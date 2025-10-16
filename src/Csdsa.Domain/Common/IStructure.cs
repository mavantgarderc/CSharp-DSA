namespace Csdsa.Domain.Common;

public interface IStructure
{
    void AddElement<T>(T element);
    void RemoveElement<T>(T element);
    IEnumerable<T> Traverse<T>();
}
