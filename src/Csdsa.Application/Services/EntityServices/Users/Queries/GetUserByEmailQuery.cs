using MediatR;
using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Entities.User;

namespace Csdsa.Application.Users.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;
