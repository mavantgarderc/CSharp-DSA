namespace Csdsa.Domain.Exceptions;

public class EmptyStructureException : DomainException
{
    public EmptyStructureException(string structureType)
        : base($"The {structureType} is empty.") { }
}
