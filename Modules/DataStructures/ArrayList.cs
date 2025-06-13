using System.Collections;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Dynamic array resizing and memory management
    ///     - Generic collection implementation (type-safe)
    ///     - Encapsulation of internal data with public APIs
    ///     - Versioning for safe enumeration and concurrency
    /// Key Practices:
    ///     - Capacity management and performance optimization
    ///     - Use of interfaces for extensibility (ICollection<T>, IEnumerable<T>)
    ///     - Exception handling for robust API boundaries
    ///     - Support for batch operations (AddRange, InsertRange, RemoveRange)
    /// </summary>
    public class ArrayList<T> : ICollection<T>, IEnumerable<T>
    {
        private T[] _items;
        private int _size;
        private int _version;
        private const int DefaultCapacity = 4;

        public int Count => _size;
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity can't get lesser than current count...");
                }

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];

                        if (_size > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, _size);
                        }

                        _items = newItems;
                    }
                    else
                    {
                        _items = [];
                    }
                }
            }
        }

        public bool IsReadOnly => false;

        public ArrayList() => _items = new T[DefaultCapacity];

        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "bro... negative capacity???");
            }
            _items = capacity == 0 ? [] : new T[capacity];
        }

        public ArrayList(IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (collection is ICollection<T> col)
            {
                int count = col.Count;
                _items = new T[count];
                col.CopyTo(_items, 0);
                _size = count;
            }
            else
            {
                _items = [];
                foreach (var item in collection)
                {
                    Add(item);
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                _items[index] = value;
                _version++;
            }
        }

        public void Add(T item)
        {
            if (_size == _items.Length)
            {
                EnsureCapacity(_size+1);
            }

            _items[_size++] = item;
            _version++;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (_size == _items.Length)
            {
                EnsureCapacity(_size+1);
            }
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index+1, _size-index);
            }

            _items[index] = item;
            _size++;
            _version++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);

                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _size--;

            if (index < _size)
            {
                Array.Copy(_items, index+1, _items, index, _size-index);
            }

            _items[_size] = default!;

            _version++;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }

            _version++;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _size);

        public void TrimToSize() => Capacity = _size;

        public void Reverse()  => Array.Reverse(_items, 0, _size);

        public T[] ToArray()
        {
            var array = new T[_size];

            Array.Copy(_items, array, _size);

            return array;
        }

        public ArrayList<T> GetRange(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(startIndex < 0 ? nameof(startIndex) : nameof(count));
            }
            if (_size-startIndex < count)
            {
                throw new IndexOutOfRangeException();
            }

            var list = new ArrayList<T>(count);

            Array.Copy(_items, startIndex, list._items, 0, count);
            list._size = count;

            return list;
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(item, Comparer<T>.Default);
        }
        
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            comparer ??= Comparer<T>.Default;
            return Array.BinarySearch(_items, 0, _size, item, comparer);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_size, collection);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (collection is ICollection<T> col)
            {
                int count = col.Count;

                if (count > 0)
                {
                    EnsureCapacity(_size+count);

                    if (index < _size)
                    {
                        Array.Copy(_items, index, _items, index+count, _size-index);
                    }

                    col.CopyTo(_items, index);

                    _size += count;

                    _version++;
                }
            }
            else
            {
                foreach (var item in collection)
                {
                    Insert(index++, item);
                }
            }
        }

        public void RemoveRange(uint startIndex, int count)
        {
            if (startIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(startIndex < 0 ? nameof(startIndex) : nameof(count));
            }
            if (_size-startIndex < count)
            {
                throw new IndexOutOfRangeException();
            }

            if (count > 0)
            {
                int newSize = _size-count;

                if (startIndex < newSize)
                {
                    Array.Copy(_items, startIndex+count, _items, startIndex, newSize-startIndex);
                }

                Array.Clear(_items, newSize, count);

                _size = newSize;
                _version++;
            }
        }

        public void SetRange(int startIndex, ICollection<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (startIndex < 0 || startIndex > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            int required = startIndex + collection.Count;
            EnsureCapacity(required);
            collection.CopyTo(_items, startIndex);

            if (required > _size)
            {
                _size = required;
            }

            _version++;
        }

        public void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > Capacity)
            {
                int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length*2;

                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                Capacity = newCapacity;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public void Sort() => Sort(Comparer<T>.Default);

        public void Sort(IComparer<T> comparer)
        {
            comparer ??= Comparer<T>.Default;

            Array.Sort(_items, 0, _size, comparer);

            _version++;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly ArrayList<T> _list;
            private int _index;
            private readonly int _version;
            private T? _current;

            internal Enumerator(ArrayList<T> list)
            {
                _list = list;
                _index = -1;
                _version = list._version;
            }

            public bool MoveNext()
            {
                CheckVersion();

                if (_index < _list._size-1)
                {
                    _current = _list._items[++_index];
                    return true;
                }

                _index = _list._size;
                _current = default;

                return false;
            }

            public T Current
            {
                get
                {
                    if (_index < 0 || _index >= _list._size)
                    {
                        throw new InvalidOperationException("invalid enumerator...");
                    }
                    return _current!;
                }
            }

            object IEnumerator.Current => Current!;

            public void Reset()
            {
                CheckVersion();
                _index = -1;
                _current = default;
            }

            public void Dispose() => _current = default;

            private void CheckVersion()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("colleciton modified during enumeration");
                }
            }
        }
    }
}
