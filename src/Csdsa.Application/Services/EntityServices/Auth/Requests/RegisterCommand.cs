using Csdsa.Application.DTOs;
using Csdsa.Application.DTOs.Auth;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Requests;

public class RegisterCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
