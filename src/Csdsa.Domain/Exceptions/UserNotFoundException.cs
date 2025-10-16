namespace Csdsa.Domain.Exceptions;

public class UserNotFoundException : AuthenticationException
{
    public UserNotFoundException() : base("User not found.") { }
    public UserNotFoundException(string message) : base(message) { }
}
