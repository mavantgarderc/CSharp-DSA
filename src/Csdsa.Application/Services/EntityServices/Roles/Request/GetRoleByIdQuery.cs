using Csdsa.Application.DTOs.Entities;
using Csdsa.Application.DTOs.Entities.Role;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record GetRoleByIdQuery(int Id) : IRequest<RoleDto>;
