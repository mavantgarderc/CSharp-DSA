namespace Csdsa.Application.DTOs.Entities.Role;

public record AssignRolesResult(
    int TotalUsers,
    int SuccessfulAssignments,
    int FailedAssignments,
    List<string> Errors,
    bool IsSuccess
);
