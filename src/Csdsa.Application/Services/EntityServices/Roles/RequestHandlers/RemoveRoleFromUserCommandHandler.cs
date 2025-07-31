using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Roles.Request;
using Csdsa.Domain.Models.Enums;
using MediatR;

namespace Csdsa.Application.Roles.CommandHandler;

public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public RemoveRoleFromUserCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _uow.Users.GetByIdAsync(request.UserId);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        if (user.Role == UserRole.SuperAdmin)
        {
            var superAdminCount = await _uow.Users.CountAsync(u => u.Role == UserRole.SuperAdmin);
            if (superAdminCount <= 1)
                throw new InvalidOperationException("Cannot remove the last SuperAdmin role.");
        }

        user.Role = UserRole.User;
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        return true;
    }
}
