namespace Csdsa.Contracts.Repositories;

public interface IPasswordHasher
{
    HashedPassword HashPassword(string plainPassword);

    bool VerifyPassword(string plainPassword, HashedPassword hashedPassword);
}
