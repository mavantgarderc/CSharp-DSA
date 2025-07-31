using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Models.UserEntities;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers;

public class ActivateUserAccountCommandHandler : IRequestHandler<ActivateUserAccountCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public ActivateUserAccountCommandHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    async Task<bool> IRequestHandler<ActivateUserAccountCommand, bool>.Handle(
        ActivateUserAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        await _uow.BeginTransactionAsync();
        try
        {
            var userRepo = _uow.Repository<User>();
            var user = await userRepo.GetByIdAsync(request.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (user.IsActive)
                return true;

            user.IsActive = true;

            await userRepo.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }
}
