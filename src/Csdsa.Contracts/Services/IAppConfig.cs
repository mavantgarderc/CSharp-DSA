namespace Csdsa.Contracts.Services;

public interface IAppConfig
{
    string JwtKey { get; }
    string JwtIssuer { get; }
    string JwtAudience { get; }

    string ConnectionString { get; }
    string RedisConnection { get; }

    OAuthConfig GoogleOAuth { get; }
}

public record OAuthConfig(string ClientId, string ClientSecret);
