using Csdsa.Application.DTOs.Auth;
using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.Register;

public class RegisterCommand : IRequest<OperationResult<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
