namespace Csdsa.Infrastructure.Config;

public interface IAppConfig
{
    string JwtIssuer { get; }
    string JwtKey { get; }
    string JwtAudience { get; }
    string RedisHost { get; }
    string ConnectionString { get; }
}
