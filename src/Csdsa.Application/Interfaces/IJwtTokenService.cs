namespace Csdsa.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, IList<string> roles);
}
