using AutoMapper;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Models.Common.UserEntities.User;
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

        async Task<bool> IRequestHandler<UpdateUserCommand, bool>.Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var userRepo = _uow.Repository<User>();

                var user = await userRepo.GetByIdAsync(request.userId);

                if (user == null) { throw new UnauthorizedAccessException("Invalid credentials"); }

                user.Email = request.Email;
                user.UserName = request.Username;
                user.IsActive = request.IsActive;

                await userRepo.UpdateAsync(user);
                await _uow.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
