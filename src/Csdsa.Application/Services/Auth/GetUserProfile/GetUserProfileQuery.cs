using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.GetUserProfile;

public class GetUserProfileQuery : IRequest<OperationResult<AuthResponse>>
{
    public required Guid UserId { get; set; }
}
