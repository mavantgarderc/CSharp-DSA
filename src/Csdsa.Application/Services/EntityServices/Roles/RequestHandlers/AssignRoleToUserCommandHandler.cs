using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Enums;
using MediatR;

namespace Csdsa.Application.Roles.CommandHandler
{
    public record AssignRoleToUserCommand(Guid UserId, UserRole Role) : IRequest<bool>;

    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public AssignRoleToUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(
            AssignRoleToUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _uow.Users.GetByIdAsync(request.UserId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

            if (user.Role == UserRole.SuperAdmin && request.Role != UserRole.SuperAdmin)
            {
                var superAdminCount = await _uow.Users.CountAsync(u =>
                    u.Role == UserRole.SuperAdmin
                );
                if (superAdminCount <= 1)
                    throw new InvalidOperationException("Cannot remove the last SuperAdmin role.");
            }

            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            return true;
        }
    }
}
