using Csdsa.Domain.Common;
using Csdsa.Domain.Exceptions;

namespace Csdsa.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; private set; }
    public ValueObjects.Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpires { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpires { get; private set; }
    public bool IsActive { get; private set; }
    public bool SoftDelete { get; private set; }
    public List<UserRole> UserRoles { get; private set; }
    public List<RefreshToken> RefreshTokens { get; private set; }
    public List<BlacklistedToken> BlacklistedTokens { get; private set; }

    public User(string userName, ValueObjects.Email email, string passwordHash)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        IsActive = true;
        SoftDelete = false;
        UserRoles = new List<UserRole>();
        RefreshTokens = new List<RefreshToken>();
        BlacklistedTokens = new List<BlacklistedToken>();
    }

    public OperationResult<User> VerifyEmail(string token)
    {
        if (!IsActive || SoftDelete) return OperationResult<User>.ErrorResult("User is inactive or deleted.");
        if (IsEmailVerified) return OperationResult<User>.ErrorResult("Email already verified.");
        if (EmailVerificationToken != token || EmailVerificationTokenExpires < DateTime.UtcNow)
            return OperationResult<User>.ErrorResult("Invalid or expired token.", new InvalidTokenException());
        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpires = null;
        return OperationResult<User>.SuccessResult(this);
    }

    public OperationResult<User> AddFailedLoginAttempt()
    {
        if (!IsActive || SoftDelete) return OperationResult<User>.ErrorResult("User is inactive or deleted.");
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            return OperationResult<User>.ErrorResult("Account locked.", new AccountLockedException(LockoutEnd.Value));
        }
        return OperationResult<User>.SuccessResult(this);
    }
}
