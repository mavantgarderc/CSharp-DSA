using Modules.DataStructures;

namespace Tests
{
    public class CustomStackTests
    {
        [Fact]
        public void Stack_ShouldPushAndPopCorrectly()
        {
            var stack = new CustomStack<int>();
            stack.Push(10);
            stack.Push(20);
            Assert.False(stack.IsEmpty);
            Assert.Equal(20, stack.Pop());
            Assert.Equal(10, stack.Pop());
            Assert.True(stack.IsEmpty);
        }

        [Fact]
        public void Stack_Peek_ShouldReturnTopWithoutRemoving()
        {
            var stack = new CustomStack<string>();
            stack.Push("x");
            Assert.Equal("x", stack.Peek());
            Assert.False(stack.IsEmpty);
        }

        [Fact]
        public void Stack_EmptyOperations_ShouldThrow()
        {
            var stack = new CustomStack<double>();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }
    }

    public class CustomQueueTests
    {
        [Fact]
        public void Queue_ShouldEnqueueAndDequeueCorrectly()
        {
            var queue = new CustomQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.False(queue.IsEmpty);
            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Dequeue());
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void Queue_Peek_ShouldReturnFrontWithoutRemoving()
        {
            var queue = new CustomQueue<string>();
            queue.Enqueue("first");
            Assert.Equal("first", queue.Peek());
        }

        [Fact]
        public void Queue_EmptyOperations_ShouldThrow()
        {
            var queue = new CustomQueue<char>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }
    }

    public class LinearQueueTests
    {
        [Fact]
        public void LinearQueue_ShouldEnqueueAndDequeueCorrectly()
        {
            var queue = new LinearQueue<int>(3);
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Peek());
        }

        [Fact]
        public void LinearQueue_ExceedingCapacity_ShouldThrow()
        {
            var queue = new LinearQueue<int>(1);
            queue.Enqueue(1);
            Assert.Throws<InvalidOperationException>(() => queue.Enqueue(2));
        }

        [Fact]
        public void LinearQueue_EmptyOperations_ShouldThrow()
        {
            var queue = new LinearQueue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }
    }

    public class CircularQueueTests
    {
        [Fact]
        public void CircularQueue_ShouldEnqueueAndDequeueInOrder()
        {
            var queue = new CircularQueue<int>(2);
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.True(queue.IsFull);
            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Peek());
        }

        [Fact]
        public void CircularQueue_EmptyAndFullChecks()
        {
            var queue = new CircularQueue<string>(1);
            Assert.True(queue.IsEmpty);
            queue.Enqueue("item");
            Assert.True(queue.IsFull);
        }

        [Fact]
        public void CircularQueue_Exceptions()
        {
            var queue = new CircularQueue<int>(1);
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            queue.Enqueue(42);
            Assert.Throws<InvalidOperationException>(() => queue.Enqueue(99));
        }

        [Fact]
        public void CircularQueue_InvalidCapacity_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new CircularQueue<int>(0));
        }
    }

    public class TwoStackQueueTests
    {
        [Fact]
        public void TwoStackQueue_ShouldSimulateQueueBehavior()
        {
            var queue = new TwoStackQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.Equal(1, queue.Dequeue());
            Assert.Equal(2, queue.Peek());
        }

        [Fact]
        public void TwoStackQueue_EmptyOperations_ShouldThrow()
        {
            var queue = new TwoStackQueue<string>();
            Assert.True(queue.IsEmpty);
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }
    }

    public class ExpressionValidatorTests
    {
        [Theory]
        [InlineData("()[]{}", true)]
        [InlineData("({[]})", true)]
        [InlineData("([)]", false)]
        [InlineData("({[})]", false)]
        [InlineData("", true)]
        public void EvaluateBalancedParentheses_ShouldValidateCorrectly(string expression, bool expected)
        {
            Assert.Equal(expected, ExpressionValidator.EvaluateBalancedParentheses(expression));
        }
    }

    public class QueueOperationsTests
    {
        [Fact]
        public void ReverseQueue_ShouldReverseItems()
        {
            var queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            var reversed = QueueOperations.ReverseQueue(queue);

            Assert.Equal(3, reversed.Dequeue());
            Assert.Equal(2, reversed.Dequeue());
            Assert.Equal(1, reversed.Dequeue());
        }
    }

    public class DequeTests
    {
        [Fact]
        public void Deque_ShouldAddAndRemoveFromBothEnds()
        {
            var deque = new Deque<string>();
            deque.AddBack("b");
            deque.AddFront("a");
            deque.AddBack("c");

            Assert.Equal("a", deque.PeekFront());
            Assert.Equal("c", deque.PeekBack());
            Assert.Equal("a", deque.RemoveFront());
            Assert.Equal("c", deque.RemoveBack());
            Assert.Equal("b", deque.RemoveFront());
            Assert.True(deque.IsEmpty);
        }

        [Fact]
        public void Deque_EmptyOperations_ShouldThrow()
        {
            var deque = new Deque<int>();
            Assert.Throws<InvalidOperationException>(() => deque.PeekFront());
            Assert.Throws<InvalidOperationException>(() => deque.PeekBack());
            Assert.Throws<InvalidOperationException>(() => deque.RemoveFront());
            Assert.Throws<InvalidOperationException>(() => deque.RemoveBack());
        }
    }

    public class PriorityQueueWrapperTests
    {
        [Fact]
        public void PriorityQueue_ShouldRespectPriorityOrder()
        {
            var pq = new PriorityQueueWrapper<int, string>();
            pq.Enqueue("low", 2);
            pq.Enqueue("high", 1);
            Assert.False(pq.IsEmpty);
            Assert.Equal("high", pq.Peek());
            Assert.Equal("high", pq.Dequeue());
            Assert.Equal("low", pq.Dequeue());
            Assert.True(pq.IsEmpty);
        }

        [Fact]
        public void PriorityQueue_EmptyOperations_ShouldThrow()
        {
            var pq = new PriorityQueueWrapper<int, string>();
            Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
            Assert.Throws<InvalidOperationException>(() => pq.Peek());
        }
    }
}
