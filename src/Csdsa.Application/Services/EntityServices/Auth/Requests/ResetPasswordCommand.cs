using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Requests;

public class ResetPasswordCommand : IRequest<ApiResponse>
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
