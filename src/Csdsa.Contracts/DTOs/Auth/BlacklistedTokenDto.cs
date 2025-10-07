namespace Csdsa.Contracts.Dtos.Auth;

public record BlacklistedTokenDto(
    Guid Id,
    string TokenId,
    DateTime ExpiresAt,
    DateTime BlackListedAt,
    string Reason,
    Guid UserId,
    UserDto User
);
