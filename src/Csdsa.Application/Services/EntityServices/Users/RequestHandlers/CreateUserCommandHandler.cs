using AutoMapper;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Models.Enums;
using Csdsa.Domain.Models.UserEntities;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            IPasswordHasher passwordHasher
        )
        {
            _uow = uow;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PasswordHash = _passwordHasher.HashPassword(request.Password),
                    Role = UserRole.User,
                };

                await _uow.Users.AddAsync(user);
                await _uow.CommitTransactionAsync();

                return _mapper.Map<UserDto>(user);
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
