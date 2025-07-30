using AutoMapper;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Models.Common.UserEntities.User;
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
            try
            {
                // Find the user by email instead of trying to map email to User
                var user = await _uow.Users.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    return false; // User not found
                }

                // Perform soft delete using user ID
                await _uow.Users.SoftDeleteAsync(user.Email);
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
