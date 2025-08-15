using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Exceptions;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using Csdsa.Domain.ValueObjects;
using MediatR;
using Serilog;

namespace Csdsa.Application.Services.Auth.Login;

/// <summary>
/// Handles user login command processing including authentication, account lockout management,
/// email verification checks, and JWT token generation.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger _logger = Log.ForContext<LoginCommandHandler>();

    // Hard-coded configuration values since IAppConfig doesn't expose these settings
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 7;
    private const int ACCESS_TOKEN_EXPIRY_MINUTES = 15;
    private const int MAX_FAILED_ATTEMPTS = 5;
    private const int LOCKOUT_DURATION_MINUTES = 30;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService,
        IPasswordHasher passwordHasher
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Handles the user login process including authentication and token generation.
    /// </summary>
    /// <param name="request">The login command containing user credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result containing authentication response or error</returns>
    public async Task<OperationResult<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            _logger.Information(
                "Login attempt for email: {Email} from IP: {IpAddress}",
                request.Email,
                request.IpAddress
            );

            // Get and validate user
            var user = await GetAndValidateUserAsync(request.Email);

            // Check account lockout status
            await ValidateAccountLockoutAsync(user);

            // Verify password
            await ValidatePasswordAsync(request.Password, user);

            // Check email verification
            ValidateEmailVerification(user);

            // Reset failed login attempts on successful login
            await ResetFailedLoginAttemptsAsync(user);

            // Generate tokens and create response
            var response = await GenerateAuthResponseAsync(user, request.IpAddress);

            _logger.Information(
                "Login successful for user: {UserId} ({Email})",
                user.Id,
                user.Email
            );
            return OperationResult<AuthResponse>.SuccessResult(response, "Login successful.");
        }
        catch (AuthenticationException)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.Error(ex, "Unexpected error during login for email: {Email}", request.Email);
            return OperationResult<AuthResponse>.ErrorResult(
                "An error occurred during login. Please try again."
            );
        }
    }

    /// <summary>
    /// Retrieves and performs basic validation on the user account.
    /// </summary>
    private async Task<User> GetAndValidateUserAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            _logger.Warning("Login failed - user not found or inactive: {Email}", email);
            throw new InvalidCredentialsException();
        }

        return user;
    }

    /// <summary>
    /// Validates that the user account is not locked out.
    /// </summary>
    private async Task ValidateAccountLockoutAsync(User user)
    {
        if (user.IsLockedOut)
        {
            // Check if lockout period has expired
            if (user.LockoutEnd <= DateTime.UtcNow)
            {
                // Unlock the account
                user.LockoutEnd = null;
                user.FailedLoginAttempts = 0;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                _logger.Information(
                    "Account automatically unlocked after lockout period: {Email}",
                    user.Email
                );
                return;
            }

            _logger.Warning(
                "Login attempt on locked account: {Email}, locked until: {LockoutEnd}",
                user.Email,
                user.LockoutEnd
            );
            throw new AccountLockedException(user.LockoutEnd!.Value);
        }
    }

    /// <summary>
    /// Validates the user's password and handles failed attempts.
    /// </summary>
    private async Task ValidatePasswordAsync(string password, User user)
    {
        var hashedPassword = new HashedPassword(user.PasswordHash);
        var isPasswordValid = _passwordHasher.VerifyPassword(password, hashedPassword);
        if (!isPasswordValid)
        {
            await HandleFailedLoginAttemptAsync(user);
            _logger.Warning("Login failed - invalid password for: {Email}", user.Email);
            throw new InvalidCredentialsException();
        }
    }

    /// <summary>
    /// Validates that the user's email has been verified.
    /// </summary>
    private void ValidateEmailVerification(User user)
    {
        if (!user.IsEmailVerified)
        {
            _logger.Warning("Login failed - email not verified: {Email}", user.Email);
            throw new EmailNotVerifiedException();
        }
    }

    /// <summary>
    /// Resets failed login attempts after a successful login.
    /// </summary>
    private async Task ResetFailedLoginAttemptsAsync(User user)
    {
        if (user.FailedLoginAttempts > 0)
        {
            _logger.Information("Resetting failed login attempts for user: {Email}", user.Email);
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _userRepository.UpdateAsync(user);
        }
    }

    /// <summary>
    /// Generates JWT tokens and creates the authentication response.
    /// </summary>
    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress)
    {
        // Get user with roles for token generation
        var userWithRoles = await _userRepository.GetByIdAsync(user.Id, u => u.Role);
        if (userWithRoles == null)
        {
            _logger.Error("Failed to retrieve user with roles for ID: {UserId}", user.Id);
            throw new InvalidOperationException(
                "User data could not be retrieved for token generation."
            );
        }

        // Generate tokens
        var accessToken = await _jwtService.GenerateAccessTokenAsync(userWithRoles);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync();

        // Get configuration values
        var refreshTokenExpiryDays = REFRESH_TOKEN_EXPIRY_DAYS;
        var accessTokenExpiryMinutes = ACCESS_TOKEN_EXPIRY_MINUTES;

        // Create and store refresh token
        var refreshTokenEntity = new Domain.Models.Auth.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
            CreatedByIp = ipAddress,
            IsRevoked = false,
        };

        user.RefreshTokens.Add(refreshTokenEntity);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Create user profile DTO
        var userProfile = CreateUserProfileDto(user, userWithRoles);

        // Create authentication response
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes),
            RefreshTokenExpiry = refreshTokenEntity.ExpiresAt,
            User = userProfile,
        };
    }

    /// <summary>
    /// Creates a user profile DTO from the user entity.
    /// </summary>
    private static UserProfileDto CreateUserProfileDto(User user, User userWithRoles)
    {
        return new UserProfileDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            Roles = userWithRoles.Role?.Select(ur => ur.Role.Name).ToList() ?? new List<string>(),
        };
    }

    /// <summary>
    /// Handles failed login attempts including account lockout logic.
    /// </summary>
    private async Task HandleFailedLoginAttemptAsync(User user)
    {
        user.FailedLoginAttempts++;
        var maxFailedAttempts = MAX_FAILED_ATTEMPTS;
        var lockoutDurationMinutes = LOCKOUT_DURATION_MINUTES;

        _logger.Warning(
            "Failed login attempt {AttemptNumber}/{MaxAttempts} for user: {Email}",
            user.FailedLoginAttempts,
            maxFailedAttempts,
            user.Email
        );

        if (user.FailedLoginAttempts >= maxFailedAttempts)
        {
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutDurationMinutes);
            _logger.Warning(
                "Account locked for user: {Email} until {LockoutEnd} after {FailedAttempts} failed attempts",
                user.Email,
                user.LockoutEnd.Value,
                user.FailedLoginAttempts
            );

            // Send lockout notification email (don't await to avoid blocking)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendAccountLockedAsync(user.Email, user.LockoutEnd.Value);
                }
                catch (Exception ex)
                {
                    _logger.Warning(
                        ex,
                        "Failed to send account locked email to {Email}",
                        user.Email
                    );
                }
            });
        }

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
