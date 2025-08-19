using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.GetAllRoles;

public class GetAllRolesQuery : IRequest<OperationResult<List<RoleDto>>>
{
    public bool IncludeInactive { get; set; } = false;
    public bool IncludePermissions { get; set; } = false;
}
