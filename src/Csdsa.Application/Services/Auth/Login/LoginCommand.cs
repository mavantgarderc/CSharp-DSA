using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.Login;

public class LoginCommand : IRequest<OperationResult<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
