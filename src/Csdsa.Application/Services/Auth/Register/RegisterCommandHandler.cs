using System.Security.Cryptography;
using AutoMapper;
using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using MediatR;
using Serilog;

namespace Csdsa.Application.Services.Auth.Register;

/// <summary>
/// handles user registration command processing including:
/// validation, user creation, role assignment, email verification, and JWT token generation.
/// </summary>
public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, OperationResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<RegisterCommandHandler>();

    private const int EMAIL_VERIFICATION_EXPIRY_HOURS = 24;
    private const int ACCESS_TOKEN_EXPIRY_MINUTES = 15;
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 7;
    private const int SECURE_TOKEN_BYTES = 32;
    private const string DEFAULT_USER_ROLE = "User";

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        IMapper mapper
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Handles user registration process.
    /// </summary>
    /// <param name="request">The registration command containing user details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result containing authentication response or error</returns>
    public async Task<OperationResult<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var validationResult = await ValidateUserUniquenessAsync(request);
            if (!validationResult.Success)
                return validationResult;

            var verificationToken = GenerateSecureToken();

            var user = CreateUserEntity(request, verificationToken);

            await _userRepository.AddAsync(user);

            await AssignDefaultRoleAsync(user);

            var (accessToken, refreshToken) = await GenerateTokensAsync(user, request.IpAddress);

            AddRefreshTokenToUser(user, refreshToken, request.IpAddress);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            await SendVerificationEmailAsync(user.Email, verificationToken);

            var response = CreateAuthResponse(user, accessToken, refreshToken);

            _logger.Information(
                "User {UserId} registered successfully with email {Email}",
                user.Id,
                user.Email
            );
            return OperationResult<AuthResponse>.SuccessResult(
                response,
                "Registration successful. Please verify your email."
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.Error(
                ex,
                "Error occurred during user registration for email {Email}",
                request.Email
            );
            return OperationResult<AuthResponse>.ErrorResult(
                "An error occurred during registration. Please try again."
            );
        }
    }

    /// <summary>
    /// Validates email and username => not already taken
    /// </summary>
    private async Task<OperationResult<AuthResponse>> ValidateUserUniquenessAsync(
        RegisterCommand request
    )
    {
        var emailTask = _userRepository.IsEmailTakenAsync(request.Email);
        var usernameTask = _userRepository.IsUsernameTakenAsync(request.UserName);

        await Task.WhenAll(emailTask, usernameTask);

        if (await emailTask)
        {
            _logger.Warning("Registration failed - email already exists: {Email}", request.Email);
            return OperationResult<AuthResponse>.ErrorResult("Email is already taken.");
        }

        if (await usernameTask)
        {
            _logger.Warning(
                "Registration failed - username already exists: {Username}",
                request.UserName
            );
            return OperationResult<AuthResponse>.ErrorResult("Username is already taken.");
        }

        return OperationResult<AuthResponse>.SuccessResult(null!, string.Empty);
    }

    /// <summary>
    /// Creates new user entity using AutoMapper with custom configuration
    /// </summary>
    private User CreateUserEntity(RegisterCommand request, string verificationToken)
    {
        var user = _mapper.Map<User>(request);

        // Set properties that need custom logic
        user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        user.EmailVerificationToken = verificationToken;
        user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(
            EMAIL_VERIFICATION_EXPIRY_HOURS
        );
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        return user;
    }

    /// <summary>
    /// Sets default user role to the new users
    /// </summary>
    private async Task AssignDefaultRoleAsync(User user)
    {
        var role = await _roleRepository.FirstOrDefaultAsync(r => r.Name == DEFAULT_USER_ROLE);
        if (role == null)
        {
            _logger.Error(
                "Critical error: Default role '{RoleName}' not found in system during registration for user {UserId}",
                DEFAULT_USER_ROLE,
                user.Id
            );
            throw new InvalidOperationException(
                $"Default role '{DEFAULT_USER_ROLE}' not found in system."
            );
        }

        user.UserRoles.Add(
            new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow,
            }
        );
        _logger.Information(
            "Assigned default role '{RoleName}' to user {UserId}",
            DEFAULT_USER_ROLE,
            user.Id
        );
    }

    /// <summary>
    /// Generates access and refresh tokens for the user
    /// </summary>
    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user, string ipAddress)
    {
        var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user, ipAddress);
        return (accessToken, refreshToken.Token);
    }

    /// <summary>
    /// Adds refresh token to user's collection
    /// </summary>
    private static void AddRefreshTokenToUser(User user, string refreshToken, string ipAddress)
    {
        user.RefreshTokens.Add(
            new Csdsa.Domain.Models.Auth.RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRY_DAYS),
                CreatedByIp = ipAddress,
                IsRevoked = false,
            }
        );
    }

    /// <summary>
    /// Creates the authentication response DTO using AutoMapper
    /// </summary>
    private AuthResponse CreateAuthResponse(User user, string accessToken, string refreshToken)
    {
        var response = _mapper.Map<AuthResponse>(user);

        // Set properties that don't come from User entity
        response.AccessToken = accessToken;
        response.RefreshToken = refreshToken;
        response.AccessTokenExpiry = DateTime.UtcNow.AddMinutes(ACCESS_TOKEN_EXPIRY_MINUTES);
        response.RefreshTokenExpiry = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRY_DAYS);
        return response;
    }

    /// <summary>
    /// Sends email verification asynchronously; without blocking the registration process.
    /// </summary>
    private async Task SendVerificationEmailAsync(string email, string verificationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _emailService.SendEmailVerificationAsync(email, verificationToken);
            _logger.Information("Verification email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.Warning(
                ex,
                "Failed to send verification email to {Email}. User can request resend later.",
                email
            );
            // don't throw - email failure shouldn't prevent successful registration
        }
    }

    /// <summary>
    /// Generates a cryptographically secure token for email verification
    /// </summary>
    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[SECURE_TOKEN_BYTES];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert
            .ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
