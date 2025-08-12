using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Auth;
using Csdsa.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Csdsa.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly RSA _rsa;
        private readonly ILogger _logger = Log.ForContext<JwtService>();

        public JwtService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _rsa = RSA.Create();

            var privateKey = _configuration["JWT:PrivateKey"];
            if (!string.IsNullOrEmpty(privateKey))
            {
                _rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            }
        }

        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(_rsa);

            // Get user roles and permissions
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            var roles = userRoles.Select(ur => ur.Role.Name).ToList();
            var permissions = userRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("UserId", user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (!string.IsNullOrEmpty(user.FirstName))
                claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));

            if (!string.IsNullOrEmpty(user.LastName))
                claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permissions
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["AccessTokenExpiryInMinutes"] ?? "15")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.Information("Access token generated for user {UserId} with roles: {Roles}",
                user.Id, string.Join(", ", roles));

            return tokenString;
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            // Ensure uniqueness
            while (await _context.RefreshTokens.AnyAsync(rt => rt.Token == refreshToken))
            {
                rng.GetBytes(randomBytes);
                refreshToken = Convert.ToBase64String(randomBytes);
            }

            return refreshToken;
        }

        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JWT");
                var tokenHandler = new JwtSecurityTokenHandler();

                // Create public key for validation
                var publicKeyRsa = RSA.Create();
                var publicKey = _configuration["JWT:PublicKey"];
                if (!string.IsNullOrEmpty(publicKey))
                {
                    publicKeyRsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                }

                var key = new RsaSecurityKey(publicKeyRsa);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Check if token is blacklisted
                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(jti) && await IsTokenBlacklistedAsync(jti))
                {
                    _logger.Warning("Attempted to use blacklisted token {TokenId}", jti);
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Token validation failed");
                return null;
            }
        }

        public async Task<string?> GetTokenIdAsync(string token, JwtSecurityTokenHandler tokenHandler)
        {
            try
            {
                var tokenHandle = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandle.ReadJwtToken(token);
                return await Task.FromResult(jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value);
            }
            catch
            {
                return await Task.FromResult<string?>(null);
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string tokenId)
        {
            return await _context.BlacklistedTokens
                .AnyAsync(bt => bt.TokenId == tokenId && bt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task BlacklistTokenAsync(string tokenId, Guid userId, string reason)
        {
            try
            {
                var expiryDate = DateTime.UtcNow.AddDays(1); // Default to 1 day for safety

                var blacklistedToken = new BlacklistedToken
                {
                    TokenId = tokenId,
                    UserId = userId,
                    ExpiresAt = expiryDate,
                    Reason = reason
                };

                _context.BlacklistedTokens.Add(blacklistedToken);
                await _context.SaveChangesAsync();

                _logger.Information("Token {TokenId} blacklisted for user {UserId}. Reason: {Reason}",
                    tokenId, userId, reason);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to blacklist token {TokenId}", tokenId);
                throw;
            }
        }

        public void Dispose()
        {
            _rsa?.Dispose();
        }

        public Task<string?> GetTokenIdAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
