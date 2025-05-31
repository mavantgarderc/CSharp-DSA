using System.Collections;

namespace Modules.DataStructures
{
    public class ArrayList : ICollection, IEnumerable
    {
        private object[] _items;
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
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity can't be less than current Count");

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        object[] newItems = new object[value];
                        if (_size > 0)
                            Array.Copy(_items, 0, newItems, 0, _size);
                        _items = newItems;
                    }
                    else
                    {
                        _items = [];
                    }
                }
            }
        }

        public ArrayList() => _items = [];

        public ArrayList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "bro... negative capacity? :/");
            }

            _items = capacity == 0 ? [] : new object[capacity];
        }

        public ArrayList(ICollection collection) : this(collection?.Count ?? 0)
        {
            ArgumentNullException.ThrowIfNull(collection);

            collection.CopyTo(_items, 0);
            _size = collection.Count;
        }

        public object this[int index]
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

        public void Add(object item)
        {
            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }

            _items[_size++] = item;
            _version++;
        }

        public void Insert(int index, object item)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }

            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }

            _items[index] = item;
            _size++;
            _version++;
        }

        public bool Remove(object item)
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
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }

            _items[_size] = null!;
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

        public bool Contains(object item) => IndexOf(item) >= 0;

        public int IndexOf(object item) => Array.IndexOf(_items, item, 0, _size);

        public void TrimToSize() => Capacity = _size;

        public void Reverse() => Array.Reverse(_items, 0, _size);

        public object[] ToArray()
        {
            object[] array = new object[_size];
            Array.Copy(_items, array, _size);
            return array;
        }

        public ArrayList GetRange(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(startIndex < 0 ? nameof(startIndex) : nameof(count));
            }
            if (_size - startIndex < count)
            {
                throw new IndexOutOfRangeException();
            }

            var list = new ArrayList(count);
            Array.Copy(_items, startIndex, list._items, 0, count);
            list._size = count;

            return list;
        }

        public int BinarySearch(object item) => BinarySearch(item, Comparer.Default);

        public int BinarySearch(object item, IComparer comparer)
        {
            return Array.BinarySearch(_items, 0, _size, item, comparer);
        }

        public void AddRange(ICollection collection)
        {
            InsertRange(_size, collection);
        }

        public void InsertRange(int index, ICollection collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            int count = collection.Count;

            if (count > 0)
            {
                EnsureCapacity(_size + count);

                if (index < _size)
                {
                    Array.Copy(_items, index, _items, index + count, _size - index);
                }

                if (collection is Array array && array.Rank == 1)
                {
                    Array.Copy(array, 0, _items, index, count);
                }
                else
                {
                    int i = index;
                    foreach (object item in collection)
                    {
                        _items[i++] = item;
                    }
                }

                _size += count;
                _version++;
            }
        }

        public void RemoveRange(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(startIndex < 0 ? nameof(startIndex) : nameof(count));
            }
            if (_size - startIndex < count)
            {
                throw new IndexOutOfRangeException();
            }

            if (count > 0)
            {
                int newSize = _size - count;
                if (startIndex < newSize)
                {
                    Array.Copy(_items, startIndex + count, _items, startIndex, newSize - startIndex);
                }

                Array.Clear(_items, newSize, count);

                _size = newSize;
                _version++;
            }
        }

        public void SetRange(int startIndex, ICollection collection)
        {
            ArgumentNullException.ThrowIfNull(collection);
            
            if (startIndex < 0 || startIndex > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            int required = startIndex + collection.Count;

            if (required > Capacity)
            {
                EnsureCapacity(required);
            }

            if (collection is Array array && array.Rank == 1)
            {
                Array.Copy(array, 0, _items, startIndex, collection.Count);
            }
            else
            {
                int i = startIndex;
                foreach (object item in collection)
                {
                    _items[i++] = item;
                }
            }

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
                int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
                if (newCapacity < minCapacity) 
                {
                    newCapacity = minCapacity;
                }
                Capacity = newCapacity;
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public void Sort() => Sort(Comparer.Default);

        public void Sort(IComparer comparer)
        {
            comparer ??= Comparer.Default;

            Array.Sort(_items, 0, _size, comparer);

            _version++;
        }

        public IEnumerator GetEnumerator() => new Enumerator(this);

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;

        private sealed class Enumerator : IEnumerator
        {
            private readonly ArrayList _list;
            private int _index;
            private readonly int _version;
            private object? _current;

            internal Enumerator(ArrayList list)
            {
                _list = list;
                _index = -1;
                _version = list._version;
            }

            public bool MoveNext()
            {
                CheckVersion();
                if (_index < _list._size - 1)
                {
                    _current = _list._items[++_index];

                    return true;
                }
                _index = _list._size;
                _current = null;

                return false;
            }

            public object Current
            {
                get
                {
                    if (_index < 0)
                    {
                        throw new InvalidOperationException("enumeration failed");
                    }
                    if (_index >= _list._size)
                    {
                        throw new InvalidOperationException("enumeration ended");
                    }

                    return _current!;
                }
            }

            public void Reset()
            {
                CheckVersion();

                _index = -1;
                _current = null;
            }

            private void CheckVersion()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException("collection modified during enumeration");
                }
            }
        }
    }
}