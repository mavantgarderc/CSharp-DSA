namespace Csdsa.Contracts.Dtos.Auth;

public record ApiClient(
    Guid Id,
    string Name,
    string ApiKey,
    bool IsActive
);
