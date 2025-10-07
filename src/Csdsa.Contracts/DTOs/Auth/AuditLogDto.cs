namespace Csdsa.Contracts.Dtos.Auth;

public record AuditLogDto(
    Guid Id,
    string Action,
    string PerformedBy,
    DateTime PerformedAt
);
