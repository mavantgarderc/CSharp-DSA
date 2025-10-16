namespace Csdsa.Domain.Exceptions;

public class AccountLockedException : AuthenticationException
{
    public DateTime LockoutEnd { get; }

    public AccountLockedException(DateTime lockoutEnd)
        : base($"Account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.")
    {
        LockoutEnd = lockoutEnd;
    }
}
