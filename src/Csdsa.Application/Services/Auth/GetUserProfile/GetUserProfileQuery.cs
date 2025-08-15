using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.GetUserProfile;

public class GetUserProfileQuery : IRequest<OperationResult<UserProfileDto>>
{
    public required Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
