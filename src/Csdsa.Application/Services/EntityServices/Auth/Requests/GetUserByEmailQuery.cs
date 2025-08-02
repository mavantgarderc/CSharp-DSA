using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Queries;

public class GetUserByEmailQuery : IRequest<UserDto?>
{
    public string Email { get; set; } = string.Empty;
}
