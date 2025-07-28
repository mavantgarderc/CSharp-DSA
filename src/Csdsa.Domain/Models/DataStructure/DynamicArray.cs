using System.Text;

namespace Csdsa.Domain.Models;

/// <summary>
/// A generic dynamic array implementation that provides resizable array functionality.
/// This is a domain model representing a dynamic collection with specific behaviors.
/// </summary>
/// <typeparam name="T">The type of elements stored in the dynamic array</typeparam>
public class DynamicArray<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;

    public DynamicArray(int capacity = 4)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _items = new T[capacity];
        _count = 0;
    }

    /// <summary>
    /// Gets or sets the element at the specified index
    /// </summary>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _items[index] = value;
        }
    }

    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min)
                newCapacity = min;
            Array.Resize(ref _items, newCapacity);
        }
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        EnsureCapacity(_count + 1);
        Array.Copy(_items, index, _items, index + 1, _count - index);
        _items[index] = item;
        _count++;
    }

    public void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _count);

        Array.Copy(_items, index + 1, _items, index, _count - index - 1);
        _items[--_count] = default!;
    }

    public void Add(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count++] = item;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
        _count = 0;
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_items[i], item))
                return i;
        }
        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public override string ToString()
    {
        if (_count == 0)
            return "[]";

        StringBuilder sb = new();
        sb.Append('[');
        for (int i = 0; i < _count; i++)
        {
            sb.Append(_items[i]);
            if (i < _count - 1)
                sb.Append(", ");
        }
        sb.Append(']');
        return sb.ToString();
    }
}
