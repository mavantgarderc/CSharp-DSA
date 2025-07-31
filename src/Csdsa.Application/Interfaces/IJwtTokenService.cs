namespace Csdsa.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, IList<string> roles);
}
