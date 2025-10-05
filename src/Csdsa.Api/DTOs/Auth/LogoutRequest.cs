namespace Csdsa.Application.DTOs.Auth;

public class LogoutRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
