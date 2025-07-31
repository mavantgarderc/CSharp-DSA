using MediatR;
using Csdsa.Application.DTOs.Entities.User;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record GetCurrentUserQuery() : IRequest<UserDto>;
