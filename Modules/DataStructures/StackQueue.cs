namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Generic data structures
    ///     - Stack and Queue implementation
    ///     - Encapsulation with private backing fields
    ///     - Indexing with C# syntax sugar
    ///     - Circular queue design
    ///     - Two-stack queue simulation
    ///     - Stack-based expression evaluation
    ///     - Queue reversal using stack
    ///     - Double-ended queue (Deque<T>) pattern
    ///     - Priority-based queueing with PriorityQueue<T>
    /// Key Practices:
    ///     - Exception handling for empty state access
    ///     - Use of List<T> as internal buffer
    ///     - Expression-bodied members for brevity
    ///     - Readonly field enforcement for immutability
    ///     - Separation of concerns for modularity
    ///     - Defensive programming with validation
    ///     - O(1) amortized design where applicable
    ///     - Minimizing allocations in queue operations
    ///     - Encapsulating .NET built-ins for predictability
    ///     - Guarding edge-case usage through contract validation
    /// </summary>
    // Provides a method to evaluate whether an expression contains balanced parentheses, brackets, and braces.
    public static class ExpressionValidator
    {
        public static bool EvaluateBalancedParentheses(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return expression == string.Empty;
            var stack = new Stack<char>();
            var bracketPairs = new Dictionary<char, char>
            {
                { ')', '(' },
                { '}', '{' },
                { ']', '[' },
            };
            foreach (var ch in expression)
            {
                if (bracketPairs.ContainsValue(ch))
                {
                    stack.Push(ch);
                }
                else if (bracketPairs.ContainsKey(ch))
                {
                    if (stack.Count == 0 || stack.Pop() != bracketPairs[ch])
                        return false;
                }
            }
            return stack.Count == 0;
        }
    }

    public class CustomStack<T>
    {
        private readonly List<T> _list = [];

        public void Push(T item) => _list.Add(item);

        public T Pop()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            var item = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            return item;
        }

        public T Peek() =>
            IsEmpty ? throw new InvalidOperationException("Stack is empty.") : _list[^1];

        public bool IsEmpty => _list.Count == 0;
    }

    public class CustomQueue<T>
    {
        private readonly List<T> _list = [];

        public void Enqueue(T item) => _list.Add(item);

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }
            var item = _list[0];
            _list.RemoveAt(0);
            return item;
        }

        public T Peek() =>
            IsEmpty ? throw new InvalidOperationException("Queue is empty.") : _list[0];

        public bool IsEmpty => _list.Count == 0;
    }

    public class LinearQueue<T>
    {
        private readonly T[] _items;
        private int _front;
        private int _rear;
        private int _count;
        public int Count => _count;
        public int Capacity => _items.Length;
        public bool IsEmpty => _count == 0;

        public LinearQueue(int capacity = 8)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be positive.", nameof(capacity));
            }

            _items = new T[capacity];
            _front = _rear = _count = 0;
        }

        public void Enqueue(T item)
        {
            if (_rear == _items.Length)
            {
                throw new InvalidOperationException("Queue is full.");
            }

            _items[_rear] = item;
            _rear++;
            _count++;
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            T item = _items[_front];
            _items![_front] = default!;
            _front++;
            _count--;
            return item;
        }

        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _items[_front];
        }
    }

    public class CircularQueue<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _tail;
        private int _size;

        public CircularQueue(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be positive.");
            }
            _buffer = new T[capacity];
        }

        public void Enqueue(T item)
        {
            if (_size == _buffer.Length)
            {
                throw new InvalidOperationException("Queue is full.");
            }

            _buffer[_tail] = item;
            _tail = (_tail + 1) % _buffer.Length;
            _size++;
        }

        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            var item = _buffer[_head];
            _head = (_head + 1) % _buffer.Length;
            _size--;
            return item;
        }

        public T Peek() =>
            IsEmpty ? throw new InvalidOperationException("Queue is empty.") : _buffer[_head];

        public bool IsEmpty => _size == 0;

        public bool IsFull => _size == _buffer.Length;
    }

    public class TwoStackQueue<T>
    {
        private readonly Stack<T> _input = new();
        private readonly Stack<T> _output = new();

        public void Enqueue(T item) => _input.Push(item);

        public T Dequeue()
        {
            Transfer();
            if (_output.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _output.Pop();
        }

        public T Peek()
        {
            Transfer();
            if (_output.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _output.Peek();
        }

        public bool IsEmpty => _input.Count == 0 && _output.Count == 0;

        private void Transfer()
        {
            if (_output.Count == 0)
                while (_input.Count > 0)
                    _output.Push(_input.Pop());
        }
    }

    public static class QueueOperations
    {
        public static Queue<T> ReverseQueue<T>(Queue<T> queue)
        {
            var stack = new Stack<T>();
            while (queue.Count > 0)
                stack.Push(queue.Dequeue());
            while (stack.Count > 0)
                queue.Enqueue(stack.Pop());
            return queue;
        }
    }

    public class Deque<T>
    {
        private readonly LinkedList<T> _list = new();

        public void AddFront(T item) => _list.AddFirst(item);

        public void AddBack(T item) => _list.AddLast(item);

        public T RemoveFront()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            var item = _list.First!.Value;
            _list.RemoveFirst();
            return item;
        }

        public T RemoveBack()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            var item = _list.Last!.Value;
            _list.RemoveLast();
            return item;
        }

        public T PeekFront() =>
            IsEmpty ? throw new InvalidOperationException("Deque is empty.") : _list.First!.Value;

        public T PeekBack() =>
            IsEmpty ? throw new InvalidOperationException("Deque is empty.") : _list.Last!.Value;

        public bool IsEmpty => _list.Count == 0;
    }

    public class PriorityQueueWrapper<TPriority, TValue>
    {
        private readonly PriorityQueue<TValue, TPriority> _priorityQueue = new();

        public void Enqueue(TValue value, TPriority priority) =>
            _priorityQueue.Enqueue(value, priority);

        public TValue Dequeue()
        {
            if (_priorityQueue.Count == 0)
            {
                throw new InvalidOperationException("Priority queue is empty.");
            }
            return _priorityQueue.Dequeue();
        }

        public TValue Peek()
        {
            if (_priorityQueue.Count == 0)
            {
                throw new InvalidOperationException("Priority queue is empty.");
            }
            return _priorityQueue.Peek();
        }

        public bool IsEmpty => _priorityQueue.Count == 0;
    }
}
