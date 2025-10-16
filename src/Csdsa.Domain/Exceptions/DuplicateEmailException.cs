namespace Csdsa.Domain.Exceptions;

public class DuplicateEmailException : AuthenticationException
{
    public DuplicateEmailException() : base("Email address is already registered.") { }
    public DuplicateEmailException(string message) : base(message) { }
}
