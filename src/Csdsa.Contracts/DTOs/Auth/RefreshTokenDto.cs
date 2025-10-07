namespace Csdsa.Contracts.Dtos.Auth;

public record RefreshTokenDto(
    string Token,
    DateTime ExpiresAt,
    bool IsRevoked,
    string CreatedByIp,
    DateTime? RevokedAt,
    string? RevokedByIp,
    string? RevokedReason,
    string? ReplacedByToken,
    Guid UserId,
    UserDto User,
    bool IsExpired,
    bool IsActive,
    bool IsValidForRefresh,
    TimeSpan GetRemainingTime
);
