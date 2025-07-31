using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Queries;

public record GetUsersByRoleQuery(UserRole Role) : IRequest<List<UserDto>>;
