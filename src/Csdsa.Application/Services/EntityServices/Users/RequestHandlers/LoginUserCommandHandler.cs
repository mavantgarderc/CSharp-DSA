using AutoMapper;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Domain.Common.ValueObjects;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.RequestHandlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public LoginUserCommandHandler(
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
            LoginUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _uow.Users.GetByEmailAsync(request.Email);

            if (user == null) { throw new UnauthorizedAccessException("Invalid credentials"); }

            var hashedPassword = new HashedPassword(user.PasswordHash);
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, hashedPassword);

            if (!isPasswordValid) { throw new UnauthorizedAccessException("Invalid credentials"); }

            if (!user.IsActive) { throw new UnauthorizedAccessException("Account is not active"); }

            await _uow.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }
    }
}
