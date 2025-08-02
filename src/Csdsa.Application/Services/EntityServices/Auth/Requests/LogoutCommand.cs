using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.EntityServices.Auth.Requests;

public class LogoutCommand : IRequest<ApiResponse>
{
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
