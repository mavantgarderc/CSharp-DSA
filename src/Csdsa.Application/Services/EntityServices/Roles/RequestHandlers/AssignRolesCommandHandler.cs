using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Roles.RequestHandlers
{
    public record AssignRolesCommand(List<Guid> UserIds, UserRole Role)
        : IRequest<AssignRolesResult>;

    public class AssignRolesCommandHandler : IRequestHandler<AssignRolesCommand, AssignRolesResult>
    {
        private readonly IUnitOfWork _uow;

        public AssignRolesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AssignRolesResult> Handle(
            AssignRolesCommand request,
            CancellationToken cancellationToken
        )
        {
            int successfulAssignments = 0;
            int failedAssignments = 0;
            var errors = new List<string>();

            var users = await _uow.Users.GetAllAsync(filter: u => request.UserIds.Contains(u.Id));

            var foundUserIds = users.Select(u => u.Id).ToHashSet();
            var notFoundUserIds = request.UserIds.Where(id => !foundUserIds.Contains(id)).ToList();

            foreach (var userId in notFoundUserIds)
            {
                errors.Add($"User with ID {userId} not found.");
                failedAssignments++;
            }

            if (request.Role != UserRole.SuperAdmin)
            {
                var superAdminsToModify = users.Where(u => u.Role == UserRole.SuperAdmin).ToList();
                if (superAdminsToModify.Any())
                {
                    var totalSuperAdmins = await _uow.Users.CountAsync(u =>
                        u.Role == UserRole.SuperAdmin
                    );
                    if (totalSuperAdmins - superAdminsToModify.Count < 1)
                    {
                        errors.Add(
                            "Cannot modify SuperAdmin roles as it would leave no SuperAdmins in the system."
                        );
                        failedAssignments += superAdminsToModify.Count;
                        users = users.Where(u => u.Role != UserRole.SuperAdmin).ToList();
                    }
                }
            }

            foreach (var user in users)
            {
                try
                {
                    user.Role = request.Role;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _uow.Users.UpdateAsync(user);
                    successfulAssignments++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to update user {user.UserName}: {ex.Message}");
                    failedAssignments++;
                }
            }

            if (successfulAssignments > 0)
            {
                await _uow.SaveChangesAsync();
            }

            return new AssignRolesResult(
                TotalUsers: request.UserIds.Count,
                SuccessfulAssignments: successfulAssignments,
                FailedAssignments: failedAssignments,
                Errors: errors,
                IsSuccess: failedAssignments == 0
            );
        }
    }
}
