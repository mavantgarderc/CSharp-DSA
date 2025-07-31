using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Services.EntityServices.Roles.Queries;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers;

public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, RoleDto>
{
    private readonly IUnitOfWork _uow;

    public GetUserRoleQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<RoleDto> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        var userCount = await _uow.Users.CountAsync(u => u.Role == user.Role);

        return new RoleDto
        {
            Id = (int)user.Role,
            Name = user.Role.ToString(),
            Description = GetRoleDescription(user.Role),
            RoleType = user.Role,
            UserCount = userCount,
            CreatedAt = DateTime.UtcNow,
        };
    }

    private static string GetRoleDescription(UserRole role)
    {
        return role switch
        {
            UserRole.User => "Standard user with basic permissions",
            UserRole.Admin => "Administrator with elevated permissions",
            UserRole.SuperAdmin => "Super administrator with full system access",
            _ => "Unknown role",
        };
    }
}
