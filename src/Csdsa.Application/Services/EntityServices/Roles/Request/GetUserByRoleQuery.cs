using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.Request;

public record GetUsersByRoleQuery(UserRole Role) : IRequest<List<UserDto>>;
