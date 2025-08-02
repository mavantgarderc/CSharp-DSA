using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Auth;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Requests;

public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}
