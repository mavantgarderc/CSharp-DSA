namespace Csdsa.Domain.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message)
        : base(message) { }

    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class InvalidCredentialsException : AuthenticationException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.") { }

    public InvalidCredentialsException(string message)
        : base(message) { }
}

public class AccountLockedException : AuthenticationException
{
    public DateTime LockoutEnd { get; }

    public AccountLockedException(DateTime lockoutEnd)
        : base($"Account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.")
    {
        LockoutEnd = lockoutEnd;
    }
}

public class EmailNotVerifiedException : AuthenticationException
{
    public EmailNotVerifiedException()
        : base("Email address is not verified.") { }
}

public class InvalidTokenException : AuthenticationException
{
    public InvalidTokenException()
        : base("Invalid or expired token.") { }

    public InvalidTokenException(string message)
        : base(message) { }
}

public class UserNotFoundException : AuthenticationException
{
    public UserNotFoundException()
        : base("User not found.") { }

    public UserNotFoundException(string message)
        : base(message) { }
}

public class DuplicateEmailException : AuthenticationException
{
    public DuplicateEmailException()
        : base("Email address is already registered.") { }

    public DuplicateEmailException(string message)
        : base(message) { }
}
