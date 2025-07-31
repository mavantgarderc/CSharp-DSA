using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Roles.Commands
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
            // Use local variables for tracking
            int successfulAssignments = 0;
            int failedAssignments = 0;
            var errors = new List<string>();

            var users = await _uow.Users.GetAllAsync(filter: u => request.UserIds.Contains(u.Id));

            var foundUserIds = users.Select(u => u.Id).ToHashSet();
            var notFoundUserIds = request.UserIds.Where(id => !foundUserIds.Contains(id)).ToList();

            // Add errors for not found users
            foreach (var userId in notFoundUserIds)
            {
                errors.Add($"User with ID {userId} not found.");
                failedAssignments++;
            }

            // Check SuperAdmin protection
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
                        // Remove SuperAdmins from processing
                        users = users.Where(u => u.Role != UserRole.SuperAdmin).ToList();
                    }
                }
            }

            // Update remaining users
            foreach (var user in users)
            {
                try
                {
                    user.Role = request.Role;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _uow.Users.UpdateAsync(user); // Added await
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

            // Create final result with all calculated values
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
