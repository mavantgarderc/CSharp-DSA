namespace Csdsa.Domain.Exceptions;

public class InvalidTokenException : AuthenticationException
{
    public InvalidTokenException() : base("Invalid or expired token.") { }
    public InvalidTokenException(string message) : base(message) { }
}
