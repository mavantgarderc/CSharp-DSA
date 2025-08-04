using Csdsa.Application.Interfaces;
using Csdsa.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Csdsa.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly int _workFactor;
    private readonly ILogger<PasswordHasher>? _logger;

    public PasswordHasher(ILogger<PasswordHasher>? logger = null, int workFactor = 12)
    {
        _logger = logger;
        _workFactor = workFactor;

        if (_workFactor < 10 || _workFactor > 18)
        {
            _logger?.LogWarning(
                "Work factor {WorkFactor} is outside recommended range (10-18)",
                _workFactor
            );
        }
    }

    public HashedPassword HashPassword(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
        {
            _logger?.LogError("Attempted to hash null or empty password");
            throw new ArgumentException("Password cannot be null or empty", nameof(plainPassword));
        }

        try
        {
            var hashedValue = BCrypt.Net.BCrypt.HashPassword(plainPassword, _workFactor);
            _logger?.LogDebug(
                "Password hashed successfully with work factor {WorkFactor}",
                _workFactor
            );
            return new HashedPassword(hashedValue);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to hash password");
            throw new InvalidOperationException("Password hashing failed", ex);
        }
    }

    public bool VerifyPassword(string plainPassword, HashedPassword hashedPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
        {
            _logger?.LogWarning("Password verification attempted with null or empty password");
            return false;
        }

        try
        {
            var result = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword.Value);
            _logger?.LogDebug("Password verification result: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Password verification failed due to exception");
            return false;
        }
    }
}
