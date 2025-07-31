using AutoMapper;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers
{
    public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SoftDeleteUserCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<bool> Handle(
            SoftDeleteUserCommand request,
            CancellationToken cancellationToken
        )
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = await _uow.Users.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    return false;
                }

                await _uow.Users.SoftDeleteAsync(user.Email);

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
}
