namespace Csdsa.Domain.Exceptions;

public class InvalidOperationException : DomainException
{
    public InvalidOperationException(string operation)
        : base($"Invalid operation: {operation}.") { }
}
