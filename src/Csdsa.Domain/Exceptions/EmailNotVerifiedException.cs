namespace Csdsa.Domain.Exceptions;

public class EmailNotVerifiedException : AuthenticationException
{
    public EmailNotVerifiedException() : base("Email address is not verified.") { }
}
