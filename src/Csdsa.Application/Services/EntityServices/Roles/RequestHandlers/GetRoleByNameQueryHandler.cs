using Csdsa.Application.DTOs.Entities;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Roles.Request;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers
{
    public class GetRoleByNameQueryHandler : IRequestHandler<GetRoleByNameQuery, RoleDto?>
    {
        private readonly IUnitOfWork _uow;

        public GetRoleByNameQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RoleDto?> Handle(
            GetRoleByNameQuery request,
            CancellationToken cancellationToken
        )
        {
            if (!Enum.TryParse<UserRole>(request.RoleName, true, out var role))
                return null;

            var userCount = await _uow.Users.CountAsync(u => u.Role == role);

            return new RoleDto
            {
                Id = (int)role,
                Name = role.ToString(),
                Description = GetRoleDescription(role),
                RoleType = role,
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
}
