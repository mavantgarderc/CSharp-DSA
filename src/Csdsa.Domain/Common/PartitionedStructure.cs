namespace Csdsa.Domain.Common;

public abstract class PartitionedStructure<T> : IStructure
{
    public abstract void AddElement<TElement>(TElement element);
    public abstract void RemoveElement<TElement>(TElement element);
    public abstract IEnumerable<TElement> Traverse<TElement>();
}
