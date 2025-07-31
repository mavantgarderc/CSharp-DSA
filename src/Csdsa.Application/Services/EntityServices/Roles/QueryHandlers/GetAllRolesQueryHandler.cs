using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Services.EntityServices.Roles.Queries;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.QueryHandlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IUnitOfWork uow;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = new List<RoleDto>();

            foreach (UserRole role in Enum.GetValues<UserRole>())
            {
                var userCount = await uow.Users.CountAsync(u => u.Role == role);

                roles.Add(new RoleDto
                {
                    Id = (int)role,
                    Name = role.ToString(),
                    Description = GetRoleDescription(role),
                    RoleType = role,
                    UserCount = userCount,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return roles.OrderBy(r => r.Id).ToList();
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
