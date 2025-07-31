using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Users.Requests;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;
