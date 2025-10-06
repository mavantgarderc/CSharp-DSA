namespace Csdsa.Application.Auth;

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

public class UserNotFoundException : AuthenticationException
{
    public UserNotFoundException()
        : base("User not found.") { }

    public UserNotFoundException(string email)
        : base($"User with email '{email}' not found.") { }
}

public class EmailAlreadyExistsException : AuthenticationException
{
    public EmailAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.") { }
}

public class InvalidTokenException : AuthenticationException
{
    public InvalidTokenException()
        : base("Invalid or expired token.") { }

    public InvalidTokenException(string message)
        : base(message) { }
}

public class TokenExpiredException : AuthenticationException
{
    public TokenExpiredException()
        : base("Token has expired.") { }
}

public class InvalidRefreshTokenException : AuthenticationException
{
    public InvalidRefreshTokenException()
        : base("Invalid refresh token.") { }
}

public class InactiveUserException : AuthenticationException
{
    public InactiveUserException()
        : base("User account is inactive.") { }
}
