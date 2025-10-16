using Csdsa.Domain.Common;

namespace Csdsa.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime Expires { get; private set; }
    public bool IsRevoked { get; private set; }
    public User User { get; private set; }

    public RefreshToken(User user, string token, DateTime expires)
    {
        Id = Guid.NewGuid();
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Expires = expires;
        IsRevoked = false;
    }

    public OperationResult<RefreshToken> Revoke()
    {
        if (IsRevoked) return OperationResult<RefreshToken>.ErrorResult("Token already revoked.");
        IsRevoked = true;
        return OperationResult<RefreshToken>.SuccessResult(this);
    }
}
