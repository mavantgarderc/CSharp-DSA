using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Entities.User;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Queries;

public class GetCurrentUserQuery : IRequest<ApiResponse<UserDto>>
{
    public Guid UserId { get; set; }
}
