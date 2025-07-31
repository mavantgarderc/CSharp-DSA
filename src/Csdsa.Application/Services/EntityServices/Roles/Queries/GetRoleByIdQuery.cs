using Csdsa.Application.DTOs.Entities.Role;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Queries;

public record GetRoleByIdQuery(int Id) : IRequest<RoleDto>;
