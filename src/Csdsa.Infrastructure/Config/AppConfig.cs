using Microsoft.Extensions.Configuration;

namespace Csdsa.Infrastructure.Config;

public class AppConfig : IAppConfig
{
    private readonly IConfiguration _config;

    public AppConfig(IConfiguration config)
    {
        _config = config;
    }

    public string JwtIssuer => _config["Jwt:Issuer"]!;
    public string JwtKey => _config["Jwt:Key"]!;
    public string JwtAudience => _config["Jwt:Audience"]!;
    public string RedisHost => _config["Redis:Host"]!;
    public string ConnectionString => _config.GetConnectionString("Default")!;
}
