using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Models.UserEntities;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var userRepo = _uow.Repository<User>();

                var user = await userRepo.GetByIdAsync(request.userId);

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                user.Email = request.Email;
                user.UserName = request.UserName;
                user.IsActive = request.IsActive;

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
}
