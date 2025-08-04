using Csdsa.Domain.ValueObjects;

namespace Csdsa.Application.Interfaces;

public interface IPasswordHasher
{
    HashedPassword HashPassword(string plainPassword);

    bool VerifyPassword(string plainPassword, HashedPassword hashedPassword);
}
