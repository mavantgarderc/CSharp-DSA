namespace Csdsa.Domain.Exceptions;

public class InvalidCredentialsException : AuthenticationException
{
    public InvalidCredentialsException() : base("Invalid email or password.") { }
    public InvalidCredentialsException(string message) : base(message) { }
}
