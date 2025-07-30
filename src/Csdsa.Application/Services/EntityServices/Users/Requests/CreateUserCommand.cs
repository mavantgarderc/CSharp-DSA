using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Commands.Users.CreateUser;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<UserDto>;
