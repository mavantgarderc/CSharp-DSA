using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.RefreshToken;

public class RefreshTokenCommand : IRequest<OperationResult<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}
