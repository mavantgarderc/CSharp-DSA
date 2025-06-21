using System.Collections;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Multiple linked list variants with specialized implementations
    ///     - Interface-absed abstraction (ILinkedList<T> for API std)
    ///     - Strict separation of factors (node, core list, iterators, extensions)
    ///     - Functional programming excercises (map, filter, reduce, foreach, partition)
    ///     - Thread ssafety via LSP-compliant synchronized wrappers
    ///     - Cycle detection using Floyd's Tortoise & Hare algo
    ///     - structural diagnostics (node depth, mid-index computations)
    /// Key Practices:
    ///     - Descriptive error handling
    ///     - diagnostic toolings
    ///     - readonly collections wrappers for immutable access
    ///     - Snapshot-based thread-safe iteration
    ///     - Positioonal analysis (GetNodeDepth)
    ///     - Patitioning for categorial operations
    /// </summary>

    #region Interface
    public interface ILinkedList<T> : IEnumerable<T>
    {
        void AddFirst(T item);  // add to beginning
        void AddLast(T item);   // add to end 
        void RemoveFirst();     // remove from beginning
        void RemoveLast();      // remove from end
        bool Remove(T item);    // remove item
        bool Contains(T item);  // search for item
        int Count { get; }      // current element count
        T PeekFirst();          // view first, no removal
        T PeekLast();           // view last, no removal
        void Clear();           // remove all
        void Reverse();         // reverse the list... duh...
        ILinkedList<T> Clone(); // deept copy creator
        void AddRange(IEnumerable<T> items); // add multiple elements
        T[] ToArray();     // copy elements to an array
        bool HasCycle();   // detects cycles
        IEnumerable<T> ReverseIterator();    // iterates backward...
        (int Length, int MidIndex) GetStructuralInfo();
        int GetNodeDepth(T item);   // finds item position from head
    }
    #endregion

    #region Nodes
    // node for singly linked lists
    public class SinglyLinkedListNode<T>(T value)
    {
        public T Value { get; set; } = value;
        public SinglyLinkedListNode<T>? Next { get; set; }
        // public SinglyLinkedListNode(T value) => Value = value;  // initializes node
    }

    // Node for doubly linked lists
    public class LinkedListNode<T>(T value)
    {
        public T Value { get; set; } = value;
        public LinkedListNode<T>? Next { get; set; }
        public LinkedListNode<T>? Prev { get; set; }
        // public LinkedListNode(T value) => Value = value;  // initializes node
    }
    #endregion

    #region Enumerators

    #region singly linked list enumerators
    public class SinglyLinkedListEnumerator<T> : IEnumerator<T>
    {
        private readonly SinglyLinkedListNode<T>? _start;
        private SinglyLinkedListNode<T>? _current;
        private readonly Func<int> _getVersion;
        private readonly int _initialVersion;

        public SinglyLinkedListEnumerator(SinglyLinkedListNode<T>? start, Func<int>? getVersion = null)
        {
            _start = start; _getVersion = getVersion ?? (() => 0); _initialVersion = _getVersion();
        }

        public T Current => _current == null ? throw new InvalidOperationException() : _current.Value;
        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            if (_initialVersion != _getVersion()) 
            {
                throw new InvalidOperationException("collection modified");
            }

            if (_current == null) 
            { 
                _current = _start; return _current != null; 
            }
            _current = _current.Next; 

            return _current != null;
        }

        public void Reset()
        {
            if (_initialVersion != _getVersion()) 
            {
                throw new InvalidOperationException("collection modified");
            }

            _current = null;
        }

        public void Dispose() { }
    }

    #endregion 

    #region doubly linked list enumerators
    public class DoublyLinkedListEnumerator<T> : IEnumerator<T>
    {
        private readonly LinkedListNode<T>? _start;
        private LinkedListNode<T>? _current;
        private readonly Func<int> _getVersion;
        private readonly int _initialVersion;

        public DoublyLinkedListEnumerator(LinkedListNode<T>? start, Func<int>? getVersion = null)  // Initializes enumerator
        {
            _start = start; _getVersion = getVersion ?? (() => 0); _initialVersion = _getVersion();
        }

        public T Current => _current == null ? throw new InvalidOperationException() : _current.Value;  // Current value
        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            if (_initialVersion != _getVersion()) 
            {
                throw new InvalidOperationException("Collection modified");
            }

            if (_current == null) 
            {
                _current = _start; return _current != null; 
            }
            _current = _current.Next; 
            return _current != null;
        }

        public void Reset()
        {
            if (_initialVersion != _getVersion()) 
            {
                throw new InvalidOperationException("Collection modified");
            }
            _current = null;
        }

        public void Dispose() { }
    }

    #endregion

    #region extension methods
    public static class LinkedListExtensions
    {
        public static IEnumerable<TResult> Map<T, TResult>(this ILinkedList<T> list, Func<T, TResult> selector)
        {
            // if (list == null || selector == null) throw new ArgumentNullException();
            // equivalent to:
            if (selector == null) 
            {
                ArgumentNullException.ThrowIfNull(selector);
            }
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }

            foreach (var item in list) 
            {
                yield return selector(item);
            }
        }

        public static IEnumerable<T> Filter<T>(this ILinkedList<T> list, Func<T, bool> predicate)
        {
            // if (list == null || predicate == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }
            if (predicate == null) 
            {
                ArgumentNullException.ThrowIfNull(predicate);
            }

            foreach (var item in list) 
            {
                if (predicate(item)) 
                {
                    yield return item;
                }
            }
        }

        public static TResult Reduce<T, TResult>(this ILinkedList<T> list, TResult seed, Func<TResult, T, TResult> accumulator)
        {
            // if (list == null || accumulator == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }
            if (accumulator == null) 
            {
                ArgumentNullException.ThrowIfNull(accumulator);
            }

            var result = seed; 

            foreach (var item in list) 
            {
                result = accumulator(result, item);
            }

            return result;
        }

        public static void ForEach<T>(this ILinkedList<T> list, Action<T> action)
        {
            // if (list == null || action == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }
            if (action == null) 
            {
                ArgumentNullException.ThrowIfNull(action);
            }

            foreach (var item in list) 
            {
                action(item);
            }
        }

        public static (ILinkedList<T> matches, ILinkedList<T> nonMatches) Partition<T>(this ILinkedList<T> list, Func<T, bool> predicate)
        {
            // if (list == null || predicate == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null)      
            {
                ArgumentNullException.ThrowIfNull(list);
            }
            if (predicate == null) 
            {
                ArgumentNullException.ThrowIfNull(predicate);
            }

            var matches = new DoublyLinkedList<T>(); 
            var nonMatches = new DoublyLinkedList<T>();

            foreach (var item in list) 
            { 
                if (predicate(item)) 
                {
                    matches.AddLast(item);
                }
                else 
                {
                    nonMatches.AddLast(item);
                }
            }

            return (matches, nonMatches);
        }

        public static ILinkedList<T> Synchronized<T>(this ILinkedList<T> list)
        {
            // if (list == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }

            return new SynchronizedLinkedList<T>(list);
        }

        public static IReadOnlyCollection<T> AsReadOnly<T>(this ILinkedList<T> list)
        {
            // if (list == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }

            return new ReadOnlyLinkedListWrapper<T>(list);
        }

        public static void DebugPrint<T>(this ILinkedList<T> list)
        {
            // if (list == null) throw new ArgumentNullException();
            // equivalent to:
            if (list == null) 
            {
                ArgumentNullException.ThrowIfNull(list);
            }

            Console.WriteLine($"LinkedList Count: {list.Count}");
            Console.WriteLine($"Has Cycle: {list.HasCycle()}");

            var (length, mid) = list.GetStructuralInfo();
            Console.WriteLine($"Structural Info - Length: {length}, Mid Index: {mid}");

            int index = 0;

            foreach (var item in list) 
            {
                Console.WriteLine($"[{index++}]: {item}");
            }
        }

        // Thread-safe linked list wrapper
        private class SynchronizedLinkedList<T>(ILinkedList<T> inner) : ILinkedList<T>
        {
            private readonly ILinkedList<T> _inner = inner;  // Wrapped list
            private readonly Lock _lock = new();  // Sync lock

            // public SynchronizedLinkedList(ILinkedList<T> inner) => _inner = inner;  // Initializes wrapper

            // Synchronized members
            public int Count { get { lock (_lock) return _inner.Count; } }
            public void AddFirst(T item) { lock (_lock) _inner.AddFirst(item); }
            public void AddLast(T item) { lock (_lock) _inner.AddLast(item); }
            public void Clear() { lock (_lock) _inner.Clear(); }
            public bool Contains(T item) { lock (_lock) return _inner.Contains(item); }
            public T PeekFirst() { lock (_lock) return _inner.PeekFirst(); }
            public T PeekLast() { lock (_lock) return _inner.PeekLast(); }
            public void RemoveFirst() { lock (_lock) _inner.RemoveFirst(); }
            public void RemoveLast() { lock (_lock) _inner.RemoveLast(); }
            public bool Remove(T item) { lock (_lock) return _inner.Remove(item); }
            public void Reverse() { lock (_lock) _inner.Reverse(); }
            public ILinkedList<T> Clone() { lock (_lock) return _inner.Clone(); }
            public void AddRange(IEnumerable<T> items) { lock (_lock) _inner.AddRange(items); }
            public T[] ToArray() { lock (_lock) return [.. _inner]; }
            public bool HasCycle() { lock (_lock) return _inner.HasCycle(); }
            public IEnumerable<T> ReverseIterator() { lock (_lock) return _inner.ReverseIterator(); }
            public (int Length, int MidIndex) GetStructuralInfo() { lock (_lock) return _inner.GetStructuralInfo(); }
            public int GetNodeDepth(T item) { lock (_lock) return _inner.GetNodeDepth(item); }

            public IEnumerator<T> GetEnumerator()  // snapshot iterator
            {
                T[] snapshot; lock (_lock) { snapshot = [.. _inner]; }
                return ((IEnumerable<T>)snapshot).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        // Read-only linked list wrapper
        private class ReadOnlyLinkedListWrapper<T>(ILinkedList<T> inner) : IReadOnlyCollection<T>
        {
            private readonly ILinkedList<T> _inner = inner;  // wrapped list

            // public ReadOnlyLinkedListWrapper(ILinkedList<T> inner) => _inner = inner;  // initializes wrapper

            public int Count => _inner.Count;
            public IEnumerator<T> GetEnumerator()
            {
                return _inner.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
    #endregion

    #endregion 

    #region Implementations

    #region Singly Linked List
    public class SinglyLinkedList<T> : ILinkedList<T>
    {
        private SinglyLinkedListNode<T>? _head;
        private SinglyLinkedListNode<T>? _tail;
        private int _count;
        private int _version;

        public void AddFirst(T item)
        {
            var newNode = new SinglyLinkedListNode<T>(item);

            if (_head == null) 
            {
                _head = _tail = newNode;
            }
            else 
            { 
                newNode.Next = _head; 
                _head = newNode; 
            }

            _count++; 
            _version++;
        }

        public void AddLast(T item)
        {
            var newNode = new SinglyLinkedListNode<T>(item);

            if (_tail == null) 
            {
                _head = _tail = newNode;
            }
            else 
            {
                _tail.Next = newNode; 
                _tail = newNode; 
            }

            _count++;
            _version++;
        }

        public void RemoveFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("list is empty...");
            }

            _head = _head.Next;

            if (_head == null) _tail = null;

            _count--; 
            _version++;
        }

        public void RemoveLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("list is empty..");
            }

            if (_head == _tail) 
            {
                _head = _tail = null;
            }
            else
            {
                var current = _head;
                while (current!.Next != _tail) 
                {
                    current = current.Next;
                }
                current.Next = null; 
                _tail = current;
            }

            _count--; 
            _version++;
        }

        public bool Remove(T item)
        {
            if (_head == null) return false;

            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(_head.Value, item)) 
            {
                RemoveFirst();
                return true;
            }

            var current = _head;

            while (current.Next != null)
            {
                if (comparer.Equals(current.Next.Value, item))
                {
                    if (current.Next == _tail) 
                    {
                        _tail = current;
                    }
                    current.Next = current.Next.Next;

                    _count--; 
                    _version++;

                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public bool Contains(T item)
        {
            var current = _head;
            var comparer = EqualityComparer<T>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Value, item)) 
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public int Count => _count;

        public T PeekFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("list is empty...");
            }

            return _head.Value;
        }

        public T PeekLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("list is empty...");
            }

            return _tail.Value;
        }

        public void Clear()
        {
            _head = _tail = null;

            _count = 0;
            _version++;
        }

        public void Reverse()
        {
            if (_head == null || _head == _tail) return;

            SinglyLinkedListNode<T>? prev = null;
            var current = _head; 
            _tail = _head;

            while (current != null)
            {
                var next = current.Next;
                current.Next = prev;
                prev = current;
                current = next;
            }

            _head = prev; 
            _version++;
        }

        public ILinkedList<T> Clone()
        {
            var clone = new SinglyLinkedList<T>();
            var current = _head;

            while (current != null) 
            { 
                clone.AddLast(current.Value); 
                current = current.Next; 
            }

            return clone;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // if (items == null) throw new ArgumentNullException(nameof(items));
            // equivalent to:
            if (items == null) 
            {
                ArgumentNullException.ThrowIfNull(items);
            }

            foreach (var item in items) 
            {
                AddLast(item);
            }
        }

        public T[] ToArray()
        {
            var array = new T[_count];
            var current = _head;

            for (int i = 0; i < _count; i++) 
            { 
                array[i] = current!.Value; 
                current = current.Next; 
            }

            return array;
        }

        public bool HasCycle()
        {
            if (_head == null) return false;

            var slow = _head; 
            var fast = _head;

            while (fast?.Next != null)
            {
                slow = slow!.Next; 
                fast = fast.Next.Next;

                if (ReferenceEquals(slow, fast)) 
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SinglyLinkedListEnumerator<T>(_head, () => _version);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> ReverseIterator()
        {
            var stack = new Stack<T>();
            var current = _head;

            while (current != null) 
            { 
                stack.Push(current.Value); 
                current = current.Next; 
            }

            while (stack.Count > 0) 
            {
                yield return stack.Pop();
            }
        }

        public (int Length, int MidIndex) GetStructuralInfo()
        {
            if (_head == null) return (0, -1);

            var slow = _head; 
            var fast = _head; 
            int index = 0;

            while (fast?.Next != null) 
            { 
                slow = slow!.Next; 
                fast = fast.Next.Next; 
                index++; 
            }

            return (_count, index);
        }

        public int GetNodeDepth(T item)
        {
            int depth = 0;
            var current = _head;
            var comparer = EqualityComparer<T>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Value, item)) 
                {
                    return depth;
                }
                depth++; 
                current = current.Next;
            }

            return -1;
        }
    }
    #endregion

    #region Circular Singly LInked List
    public class DoublyLinkedList<T> : ILinkedList<T>
    {
        private LinkedListNode<T>? _head;
        private LinkedListNode<T>? _tail;
        private int _count;
        private int _version;

        public void AddFirst(T item)
        {
            var newNode = new LinkedListNode<T>(item);

            if (_head == null) 
            {
                _head = _tail = newNode;
            }
            else 
            { 
                newNode.Next = _head; _head.Prev = newNode; _head = newNode; 
            }

            _count++; 
            _version++;
        }

        public void AddLast(T item)
        {
            var newNode = new LinkedListNode<T>(item);

            if (_tail == null) 
            {
                _head = _tail = newNode;
            }
            else 
            { 
                newNode.Prev = _tail; 
                _tail.Next = newNode; 
                _tail = newNode; 
            }

            _count++; 
            _version++;
        }

        public void RemoveFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("list is empty...");
            }

            if (_head == _tail) 
            {
                _head = _tail = null;
            }
            else 
            { 
                _head = _head.Next; _head!.Prev = null; 
            }

            _count--; 
            _version++;
        }

        public void RemoveLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("lst is empty...");
            }

            if (_head == _tail) 
            {
                _head = _tail = null;
            }
            else 
            {
                _tail = _tail.Prev; 
                _tail!.Next = null; 
            }

            _count--; 
            _version++;
        }

        public bool Remove(T item)
        {
            var node = Find(item);
            if (node == null) return false;

            if (node == _head) 
            {
                RemoveFirst();
            }
            else if (node == _tail) 
            {
                RemoveLast();
            }
            else 
            { 
                node.Prev!.Next = node.Next; 
                node.Next!.Prev = node.Prev; 
                _count--; 
                _version++; 
            }

            return true;
        }

        public bool Contains(T item) => Find(item) != null;

        public int Count => _count;

        public T PeekFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("List is empty");
            }

            return _head.Value;
        }

        public T PeekLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("List is empty");
            }

            return _tail.Value;
        }

        public void Clear()
        {
            _head = _tail = null;

            _count = 0; 
            _version++;
        }

        private LinkedListNode<T>? Find(T item)
        {
            var current = _head;
            var comparer = EqualityComparer<T>.Default;

            while (current != null)
            {
                if (comparer.Equals(current.Value, item)) 
                {
                    return current;
                }

                current = current.Next;
            }

            return null;
        }

        public void Reverse()
        {
            if (_head == null || _head == _tail) return;

            var current = _head; 
            _tail = _head; 
            LinkedListNode<T>? temp = null;

            while (current != null)
            {
                temp = current.Prev;
                current.Prev = current.Next;
                current.Next = temp;
                current = current.Prev;
            }
            if (temp != null) 
            {
                _head = temp.Prev; 
                _version++;
            }
        }

        public ILinkedList<T> Clone()
        {
            var clone = new DoublyLinkedList<T>();
            var current = _head;

            while (current != null) 
            { 
                clone.AddLast(current.Value); 
                current = current.Next; 
            }

            return clone;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // if (items == null) throw new ArgumentNullException(nameof(items));
            // equivalent to:
            ArgumentNullException.ThrowIfNull(items);

            foreach (var item in items) 
            {
                AddLast(item);
            }
        }

        public T[] ToArray()
        {
            var array = new T[_count];
            var current = _head;

            for (int i = 0; i < _count; i++) 
            { 
                array[i] = current!.Value; 
                current = current.Next; 
            }

            return array;
        }

        public bool HasCycle()
        {
            if (_head == null) return false;

            var slow = _head; 
            var fast = _head;

            while (fast?.Next != null)
            {
                slow = slow!.Next; 
                fast = fast.Next.Next;
                if (ReferenceEquals(slow, fast)) return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DoublyLinkedListEnumerator<T>(_head, () => _version);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> ReverseIterator()
        {
            var current = _tail;

            while (current != null) 
            { 
                yield return current.Value; 
                current = current.Prev; 
            }
        }

        public (int Length, int MidIndex) GetStructuralInfo()
        {
            if (_head == null) return (0, -1);

            var slow = _head; 
            var fast = _head; 
            int index = 0;

            while (fast?.Next != null) 
            { 
                slow = slow!.Next; 
                fast = fast.Next.Next; 
                index++; 
            }

            return (_count, index);
        }

        public int GetNodeDepth(T item)
        {
            var node = Find(item);
            if (node == null) return -1;
            int depth = 0; 
            var current = node;

            while (current.Prev != null) 
            { 
                depth++; current = current.Prev; 
            }

            return depth;
        }
    }

    #endregion

    #region Doubly Linked List 
    public class CircularLinkedList<T> : ILinkedList<T>
    {
        private SinglyLinkedListNode<T>? _tail;
        private int _count;
        private int _version;

        private SinglyLinkedListNode<T>? Head => _tail?.Next;

        public void AddFirst(T item)
        {
            var newNode = new SinglyLinkedListNode<T>(item);

            if (_tail == null) 
            { 
                _tail = newNode; newNode.Next = newNode; 
            }
            else 
            { 
                newNode.Next = _tail.Next; _tail.Next = newNode; 
            }

            _count++; 
            _version++;
        }

        public void AddLast(T item)
        {
            AddFirst(item); 
            _tail = _tail!.Next;
        }

        public void RemoveFirst()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("list is empty bro...");
            }

            if (_tail.Next == _tail) 
            {
                _tail = null;
            }
            else 
            {
                _tail.Next = _tail.Next!.Next;
            }

            _count--; 
            _version++;
        }

        public void RemoveLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("HAHA! EMPTY...");
            }

            if (_tail.Next == _tail) 
            {
                _tail = null;
            }
            else
            {
                var current = _tail.Next;
                while (current!.Next != _tail) 
                {
                    current = current.Next;
                }

                current.Next = _tail.Next; 
                _tail = current;
            }

            _count--; 
            _version++;
        }

        public bool Remove(T item)
        {
            if (_tail == null) return false;

            var comparer = EqualityComparer<T>.Default;
            var head = Head!;
            if (comparer.Equals(head.Value, item)) 
            {
                RemoveFirst(); return true; 
            }
            var current = head;

            while (current.Next != head)
            {
                if (comparer.Equals(current.Next!.Value, item))
                {
                    if (current.Next == _tail) 
                    {
                        _tail = current;
                    }
                    current.Next = current.Next.Next;

                    _count--; 
                    _version++;

                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public bool Contains(T item)
        {
            if (_tail == null) return false;

            var current = Head; 
            var comparer = EqualityComparer<T>.Default;

            do
            {
                if (comparer.Equals(current!.Value, item)) return true;
                current = current.Next;
            } while (current != Head);

            return false;
        }

        public int Count => _count;

        public T PeekFirst()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("List is empty");
            }

            return Head!.Value;
        }

        public T PeekLast()
        {
            if (_tail == null) 
            {
                throw new InvalidOperationException("List is empty");
            }

            return _tail.Value;
        }

        public void Clear()
        {
            _tail = null; 

            _count = 0; 
            _version++;
        }

        public void Reverse()
        {
            if (_tail == null || _tail.Next == _tail) return;

            SinglyLinkedListNode<T>? prev = _tail; 
            var current = Head; 
            var head = Head;

            do
            {
                var next = current!.Next; 
                current.Next = prev; 
                prev = current; 
                current = next;
            } while (current != head);

            _tail = head; 
            _version++;
        }

        public ILinkedList<T> Clone()
        {
            var clone = new CircularLinkedList<T>();
            if (_tail == null) return clone;
            var current = Head;

            do 
            { 
                clone.AddLast(current!.Value); 
                current = current.Next; 
            } while (current != Head);

            return clone;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // if (items == null) throw new ArgumentNullException(nameof(items));
            // equivalent to:
            ArgumentNullException.ThrowIfNull(items);

            foreach (var item in items) 
            {
                AddLast(item);
            }
        }

        public T[] ToArray()
        {
            var array = new T[_count];
            if (_tail == null) return array;
            var current = Head;

            for (int i = 0; i < _count; i++) 
            {
                array[i] = current!.Value; 
                current = current.Next;
            }

            return array;
        }

        public bool HasCycle() => true;

        public IEnumerator<T> GetEnumerator()
        {
            if (_tail == null) yield break;

            var start = Head; 
            var current = start; 
            var version = _version;

            do
            {
                if (version != _version) 
                {
                    throw new InvalidOperationException("collection modified");
                }

                yield return current!.Value; 

                current = current.Next;
            } while (current != start);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> ReverseIterator()
        {
            if (_tail == null) yield break;

            var stack = new Stack<T>(); 
            var current = Head;

            do 
            {
                stack.Push(current!.Value); 
                current = current.Next; 
            } while (current != Head);

            while (stack.Count > 0) 
            {
                yield return stack.Pop();
            }
        }

        public (int Length, int MidIndex) GetStructuralInfo()
        {
            if (_tail == null) return (0, -1);

            var slow = Head; 
            var fast = Head; 
            int index = 0;

            for (int i = 0; i < _count / 2; i++) 
            { 
                slow = slow!.Next; 
                fast = fast!.Next!.Next; 
                index++; 
            }

            return (_count, index);
        }

        public int GetNodeDepth(T item)
        {
            if (_tail == null) return -1;

            int depth = 0; 
            var current = Head; 
            var comparer = EqualityComparer<T>.Default;

            do
            {
                if (comparer.Equals(current!.Value, item)) return depth;

                depth++; 
                current = current.Next;
            } while (current != Head);

            return -1;
        }
    }

    #endregion

    #region Circular Doubly Linked List
    public class CircularDoublyLinkedList<T> : ILinkedList<T>
    {
        private LinkedListNode<T>? _head;
        private int _count;
        private int _version;

        private LinkedListNode<T>? Tail => _head?.Prev;

        public void AddFirst(T item)
        {
            var newNode = new LinkedListNode<T>(item);

            if (_head == null) 
            {
                newNode.Next = newNode; newNode.Prev = newNode; _head = newNode; 
            }
            else
            {
                newNode.Next = _head; 
                newNode.Prev = _head.Prev;
                _head.Prev!.Next = newNode; 
                _head.Prev = newNode; 
                _head = newNode;
            }

            _count++; 
            _version++;
        }

        public void AddLast(T item)
        {
            if (_head == null) 
            {
                AddFirst(item);
            }
            else
            {
                var newNode = new LinkedListNode<T>(item); 
                var tail = Tail;
                newNode.Next = _head; 
                newNode.Prev = tail;
                tail!.Next = newNode; 
                _head.Prev = newNode;
            }

            _count++; 
            _version++;
        }

        public void RemoveFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("bro list is empty...");
            }

            if (_head.Next == _head) 
            {
                _head = null;
            }
            else
            {
                var newHead = _head.Next; 
                var tail = Tail;
                newHead!.Prev = tail; 
                tail!.Next = newHead; 
                _head = newHead;
            }

            _count--; 
            _version++;
        }

        public void RemoveLast()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("list is empty bro...");
            }

            if (_head.Next == _head) 
            {
                _head = null;
            }
            else
            {
                var tail = Tail; 
                var newTail = 
                    tail!.Prev;
                newTail!.Next = _head; 
                _head.Prev = newTail;
            }

            _count--; 
            _version++;
        }

        public bool Remove(T item)
        {
            if (_head == null) return false;

            var comparer = EqualityComparer<T>.Default; var current = _head;

            do
            {
                if (comparer.Equals(current!.Value, item))
                {
                    if (current == _head) 
                    {
                        RemoveFirst();
                    }
                    else if (current == Tail) 
                    {
                        RemoveLast();
                    }
                    else
                    {
                        current.Prev!.Next = current.Next; 
                        current.Next!.Prev = current.Prev; 

                        _count--; 
                        _version++; 
                    }

                    return true;
                }
                current = current.Next;
            } while (current != _head);

            return false;
        }

        public bool Contains(T item)
        {
            if (_head == null) return false;

            var current = _head; var comparer = EqualityComparer<T>.Default;

            do
            {
                if (comparer.Equals(current.Value, item)) 
                {
                    return true;
                }

                current = current.Next!;
            } while (current != _head);

            return false;
        }

        public int Count => _count;

        public T PeekFirst()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("list is empty");
            }

            return _head.Value;
        }

        public T PeekLast()
        {
            if (_head == null) 
            {
                throw new InvalidOperationException("List is empty");
            }

            return Tail!.Value;
        }

        public void Clear()
        {
            _head = null; 

            _count = 0; 
            _version++;
        }

        public void Reverse()
        {
            if (_head == null || _head.Next == _head) return;

            var current = _head;

            do
            {
                var temp = current!.Prev;

                current.Prev = current.Next;
                current.Next = temp;
                current = current.Prev;
            } while (current != _head);

            _head = _head.Next; 

            _version++;
        }

        public ILinkedList<T> Clone()
        {
            var clone = new CircularDoublyLinkedList<T>();

            if (_head == null) 
            {
                return clone;
            }

            var current = _head;

            do 
            { 
                clone.AddLast(current.Value); 
                current = current.Next!; 
            } while (current != _head);

            return clone;
        }

        public void AddRange(IEnumerable<T> items)
        {
            // if (items == null) throw new ArgumentNullException(nameof(items)); 
            // equivalent to:
            ArgumentNullException.ThrowIfNull(items);

            foreach (var item in items) 
            {
                AddLast(item);
            }
        }

        public T[] ToArray()
        {
            var array = new T[_count];

            if (_head == null) 
            {
                return array;
            }

            var current = _head;

            for (int i = 0; i < _count; i++) 
            { 
                array[i] = current!.Value; 
                current = current.Next; 
            }

            return array;
        }

        public bool HasCycle()
        {
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_head == null) yield break;

            var start = _head; 
            var current = start; 
            var version = _version;

            do
            {
                if (version != _version) 
                {
                    throw new InvalidOperationException("Collection modified");
                }

                yield return current!.Value;
                current = current.Next;
            } while (current != start);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> ReverseIterator()
        {
            if (_head == null) yield break;

            var start = Tail; var current = start;

            do 
            { 
                yield return current!.Value; 
                current = current.Prev; 
            } while (current != start);
        }

        public (int Length, int MidIndex) GetStructuralInfo()
        {
            if (_head == null) return (0, -1);

            var slow = _head; 
            var fast = _head; 
            int index = 0;

            do 
            {
                slow = slow!.Next; 
                fast = fast!.Next!.Next; 
                index++;
            } while (fast != _head && fast!.Next != _head);

            return (_count, index);
        }

        public int GetNodeDepth(T item)
        {
            if (_head == null) return -1;

            int depth = 0; 
            var current = _head; 
            var comparer = EqualityComparer<T>.Default;

            do
            {
                if (comparer.Equals(current.Value, item)) return depth;
                depth++; 
                current = current.Next!;
            } while (current != _head);

            return -1;
        }
    }

    #endregion

    #endregion
}
