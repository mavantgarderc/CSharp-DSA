using Csdsa.Application.DTOs.Entities.Role;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record GetAllRolesQuery() : IRequest<List<RoleDto>>;
