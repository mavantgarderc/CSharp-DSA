using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Models.Common.UserEntities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers;

public class ActivateUserAccountCommandHandler : IRequestHandler<ActivateUserAccountCommand, bool>
{
    private readonly IUnitOfWork uow;

    public ActivateUserAccountCommandHandler(IUnitOfWork unitOfWork)
    {
        uow = unitOfWork;
    }

    async Task<bool> IRequestHandler<ActivateUserAccountCommand, bool>.Handle(
        ActivateUserAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        var userRepo = uow.Repository<User>();
        var user = await userRepo.GetByIdAsync(request.userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.IsActive) return true;

        user.IsActive = true;

        await userRepo.UpdateAsync(user);
        await uow.SaveChangesAsync();

        return true;
    }
}
