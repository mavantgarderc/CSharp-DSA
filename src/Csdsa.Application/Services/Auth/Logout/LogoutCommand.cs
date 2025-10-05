using Csdsa.Domain.Models;
using MediatR;

namespace Csdsa.Application.Services.Auth.Logout;

public class LogoutCommand : IRequest<OperationResult<Unit>>
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public LogoutCommand(string accessToken, string refreshToken, string ipAddress)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
    }
}
