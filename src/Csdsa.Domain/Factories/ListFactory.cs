namespace Csdsa.Domain.Factories;

public class ListFactory
{
    public Aggregates.ListAggregate.LinkedList<T> CreateLinkedList<T>()
    {
        return new Aggregates.ListAggregate.LinkedList<T>();
    }
}
