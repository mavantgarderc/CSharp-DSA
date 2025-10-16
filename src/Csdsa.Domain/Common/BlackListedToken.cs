namespace Csdsa.Domain.Entities;

public class BlacklistedToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime BlacklistedAt { get; private set; }
    public User User { get; private set; }

    public BlacklistedToken(User user, string token)
    {
        Id = Guid.NewGuid();
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        BlacklistedAt = DateTime.UtcNow;
    }
}
