using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Users.Queries;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserDto?>>;
