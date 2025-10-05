using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Csdsa.Infrastructure.Auth.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IBlacklistedTokenRepository? _blacklistedTokenRepository;
    private readonly ILogger<JwtService> _logger;
    private readonly RSA _privateRsa;
    private readonly Lazy<RSA> _publicRsa;
    private bool _disposed;

    public JwtService(
        IConfiguration configuration,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IBlacklistedTokenRepository blacklistedTokenRepository,
        ILogger<JwtService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _blacklistedTokenRepository = blacklistedTokenRepository; // Optional, can be null
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load private key
        var privateKeyBase64 = Environment.GetEnvironmentVariable("CSDSA_JWT_PRIVATE_KEY") ?? _configuration["JWT:PrivateKey"];
        if (string.IsNullOrEmpty(privateKeyBase64))
            throw new InvalidOperationException("JWT PrivateKey not found.");

        _privateRsa = RSA.Create();
        _privateRsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

        // Lazy-load public key
        _publicRsa = new Lazy<RSA>(() =>
        {
            var publicKeyBase64 = Environment.GetEnvironmentVariable("CSDSA_JWT_PUBLIC_KEY") ?? _configuration["JWT:PublicKey"];
            if (string.IsNullOrEmpty(publicKeyBase64))
                throw new InvalidOperationException("JWT PublicKey not found.");

            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyBase64), out _);
            return rsa;
        });
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        return await GenerateAccessTokenAsync(user, string.Empty);
    }

    public async Task<string> GenerateAccessTokenAsync(User user, string ipAddress)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(
            new RsaSecurityKey(_privateRsa),
            SecurityAlgorithms.RsaSha256);

        var claims = await CreateClaimsAsync(user);

        if (!string.IsNullOrEmpty(ipAddress))
        {
            claims = claims.Append(new Claim("ip", ipAddress)).ToArray();
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"],
            IssuedAt = DateTime.UtcNow
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(User user, string ipAddress)
    {
        var token = await GenerateRefreshTokenAsync(ipAddress);
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        return refreshToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(string ipAddress)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return await Task.FromResult(token);
    }

    public async Task<(bool IsValid, User? User)> ValidateRefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token == null || token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
        {
            return (false, null);
        }

        var user = await _userRepository.GetUserWithRolesAndPermissionsAsync(token.UserId);
        return (true, user);
    }

    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new RsaSecurityKey(_publicRsa.Value)
            };

            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti))
                return false;

            if (_blacklistedTokenRepository != null)
            {
                var isBlacklisted = await _blacklistedTokenRepository.GetByTokenIdAsync(jti) != null;
                if (isBlacklisted)
                    return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed for access token.");
            return false;
        }
    }

    public async Task<string?> GetTokenIdAsync(string accessToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            return await Task.FromResult(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value);
        }
        catch
        {
            return await Task.FromResult<string?>(null);
        }
    }

    public async Task<bool> IsTokenBlacklistedAsync(string accessToken)
    {
        if (_blacklistedTokenRepository == null)
        {
            _logger.LogWarning("BlacklistedTokenRepository not available, cannot check blacklist.");
            return false;
        }

        var jti = await GetTokenIdAsync(accessToken);
        if (string.IsNullOrEmpty(jti))
            return false;

        var blacklistedToken = await _blacklistedTokenRepository.GetByTokenIdAsync(jti);
        return blacklistedToken != null;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token);
        }
    }

    public async Task BlacklistTokenAsync(string accessToken, Guid userId, string reason)
    {
        if (_blacklistedTokenRepository == null)
        {
            _logger.LogWarning("BlacklistedTokenRepository not available, cannot blacklist token.");
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (!string.IsNullOrEmpty(jti))
        {
            var blacklistedToken = new BlacklistedToken
            {
                TokenId = jti,
                UserId = userId,
                ExpiresAt = jwtToken.ValidTo,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await _blacklistedTokenRepository.AddAsync(blacklistedToken);
        }
    }

    private async Task<Claim[]> CreateClaimsAsync(User user)
    {
        var userWithRoles = await _userRepository.GetUserWithRolesAndPermissionsAsync(user.Id);
        if (userWithRoles == null)
        {
            throw new InvalidOperationException("User with roles not found.");
        }

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        if (userWithRoles.UserRoles != null)
        {
            foreach (var userRole in userWithRoles.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                if (userRole.Role.RolePermissions != null)
                {
                    foreach (var permission in userRole.Role.RolePermissions)
                    {
                        claims.Add(new Claim("Permission", $"{permission.Permission.Resource}:{permission.Permission.Action}"));
                    }
                }
            }
        }

        return claims.ToArray();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _privateRsa?.Dispose();
            if (_publicRsa.IsValueCreated)
                _publicRsa.Value?.Dispose();
            _disposed = true;
        }
    }
}
