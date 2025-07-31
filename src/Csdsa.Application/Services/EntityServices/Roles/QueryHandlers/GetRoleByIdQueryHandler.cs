using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Services.EntityServices.Roles.Queries;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Roles.QueryHandler
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto?>
    {
        private readonly IUnitOfWork _uow;

        public GetRoleByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RoleDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(typeof(UserRole), request.Id))
                return null;

            var role = (UserRole)request.Id;
            var userCount = await _uow.Users.CountAsync(u => u.Role == role);

            return new RoleDto
            {
                Id = request.Id,
                Name = role.ToString(),
                Description = GetRoleDescription(role),
                RoleType = role,
                UserCount = userCount,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static string GetRoleDescription(UserRole role)
        {
            return role switch
            {
                UserRole.User => "Standard user with basic permissions",
                UserRole.Admin => "Administrator with elevated permissions",
                UserRole.SuperAdmin => "Super administrator with full system access",
                _ => "Unknown role"
            };
        }
    }
}
