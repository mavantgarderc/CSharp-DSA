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
        /// <summary>
        /// Determines whether the input string has balanced brackets: (), {}, [].
        /// </summary>
        /// <param name="expression">The input string to evaluate.</param>
        /// <returns><c>true</c> if brackets are balanced; otherwise, <c>false</c>.</returns>
        public static bool EvaluateBalancedParentheses(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return expression == string.Empty;
            var stack = new Stack<char>();
            var bracketPairs = new Dictionary<char, char>
            {
                { ')', '(' },
                { '}', '{' },
                { ']', '[' }
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

        // Adds an item to the top of the stack
        public void Push(T item) => _list.Add(item);

        // Removes and returns the item at the top of the stack
        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Stack is empty.");
            var item = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            return item;
        }

        // Returns the item at the top without removing it
        public T Peek() => IsEmpty ? throw new InvalidOperationException("Stack is empty.") : _list[^1];

        // Checks if the stack is empty
        public bool IsEmpty => _list.Count == 0;
    }

    public class CustomQueue<T>
    {
        private readonly List<T> _list = [];

        // Adds an item to the end of the queue
        public void Enqueue(T item) => _list.Add(item);

        // Removes and returns the item at the front of the queue
        public T Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException("Queue is empty.");
            var item = _list[0];
            _list.RemoveAt(0);
            return item;
        }

        // Returns the item at the front without removing it
        public T Peek() => IsEmpty ? throw new InvalidOperationException("Queue is empty.") : _list[0];

        // Checks if the queue is empty
        public bool IsEmpty => _list.Count == 0;
    }

    public class CircularQueue<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _tail;
        private int _size;

        public CircularQueue(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive.");
            _buffer = new T[capacity];
        }

        // Adds an item to the circular queue
        public void Enqueue(T item)
        {
            if (_size == _buffer.Length) throw new InvalidOperationException("Queue is full.");
            _buffer[_tail] = item;
            _tail = (_tail + 1) % _buffer.Length;
            _size++;
        }

        // Removes and returns the item at the front of the circular queue
        public T Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException("Queue is empty.");
            var item = _buffer[_head];
            _head = (_head + 1) % _buffer.Length;
            _size--;
            return item;
        }

        // Returns the item at the front of the circular queue
        public T Peek() => IsEmpty ? throw new InvalidOperationException("Queue is empty.") : _buffer[_head];

        // Checks if the circular queue is empty
        public bool IsEmpty => _size == 0;

        // Checks if the circular queue is full
        public bool IsFull => _size == _buffer.Length;
    }

    public class TwoStackQueue<T>
    {
        private readonly Stack<T> _input = new();
        private readonly Stack<T> _output = new();

        // Adds an item to the end of the queue using input stack
        public void Enqueue(T item) => _input.Push(item);

        // Removes and returns the front item from the queue
        public T Dequeue()
        {
            Transfer();
            if (_output.Count == 0) throw new InvalidOperationException("Queue is empty.");
            return _output.Pop();
        }

        // Returns the front item without removing it
        public T Peek()
        {
            Transfer();
            if (_output.Count == 0) throw new InvalidOperationException("Queue is empty.");
            return _output.Peek();
        }

        // Checks if the simulated queue is empty
        public bool IsEmpty => _input.Count == 0 && _output.Count == 0;

        // Transfers items from input stack to output stack if needed
        private void Transfer()
        {
            if (_output.Count == 0)
                while (_input.Count > 0)
                    _output.Push(_input.Pop());
        }
    }


    // Reverses the elements in a queue
    public static class QueueOperations  // New wrapper class
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

        // Adds an item to the front of the deque
        public void AddFront(T item) => _list.AddFirst(item);

        // Adds an item to the back of the deque
        public void AddBack(T item) => _list.AddLast(item);

        // Removes and returns the front item
        public T RemoveFront()
        {
            if (IsEmpty) throw new InvalidOperationException("Deque is empty.");

            // Add null-forgiving operator to tell compiler we've validated existence
            var item = _list.First!.Value;  // <-- Note the ! operator here
            _list.RemoveFirst();
            return item;
        }

        // Removes and returns the back item
        public T RemoveBack()
        {
            if (IsEmpty) throw new InvalidOperationException("Deque is empty.");
            var item = _list.Last!.Value;
            _list.RemoveLast();
            return item;
        }

        // Returns the front item without removing it
        public T PeekFront() => IsEmpty ? throw new InvalidOperationException("Deque is empty.") : _list.First!.Value;

        // Returns the back item without removing it
        public T PeekBack() => IsEmpty ? throw new InvalidOperationException("Deque is empty.") : _list.Last!.Value;

        // Checks if the deque is empty
        public bool IsEmpty => _list.Count == 0;
    }

    public class PriorityQueueWrapper<TPriority, TValue>
    {
        private readonly PriorityQueue<TValue, TPriority> _priorityQueue = new();

        // Adds a value to the priority queue with the specified priority
        public void Enqueue(TValue value, TPriority priority) => _priorityQueue.Enqueue(value, priority);

        // Removes and returns the highest priority item
        public TValue Dequeue()
        {
            if (_priorityQueue.Count == 0) throw new InvalidOperationException("Priority queue is empty.");
            return _priorityQueue.Dequeue();
        }

        // Returns the highest priority item without removing it
        public TValue Peek()
        {
            if (_priorityQueue.Count == 0) throw new InvalidOperationException("Priority queue is empty.");
            return _priorityQueue.Peek();
        }

        // Checks if the priority queue is empty
        public bool IsEmpty => _priorityQueue.Count == 0;
    }
}
