using Modules.DataStructures;

namespace Modules.Tests
{
    public class CustomStackTests
    {
        [Fact]
        public void PushPopPeek_StackBehavesCorrectly()
        {
            var stack = new CustomStack<int>();
            Assert.True(stack.IsEmpty);

            stack.Push(1);
            Assert.False(stack.IsEmpty);
            Assert.Equal(1, stack.Peek());
            Assert.Equal(1, stack.Pop());
            Assert.True(stack.IsEmpty);
        }

        [Fact]
        public void PopPeek_EmptyStack_Throws()
        {
            var stack = new CustomStack<string>();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }
    }

    public class CustomQueueTests
    {
        [Fact]
        public void EnqueueDequeuePeek_QueueBehavesCorrectly()
        {
            var queue = new CustomQueue<int>();
            Assert.True(queue.IsEmpty);

            queue.Enqueue(10);
            Assert.False(queue.IsEmpty);
            Assert.Equal(10, queue.Peek());
            Assert.Equal(10, queue.Dequeue());
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public void DequeuePeek_EmptyQueue_Throws()
        {
            var queue = new CustomQueue<string>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }
    }

    public class CircularQueueTests
    {
        [Fact]
        public void CircularQueue_EnqueueDequeue_WorksCorrectly()
        {
            var cq = new CircularQueue<int>(3);
            cq.Enqueue(1);
            cq.Enqueue(2);
            cq.Enqueue(3);
            Assert.True(cq.IsFull);

            Assert.Equal(1, cq.Dequeue());
            Assert.False(cq.IsFull);
            cq.Enqueue(4);
            Assert.True(cq.IsFull);
            Assert.Equal(2, cq.Peek());
        }

        [Fact]
        public void CircularQueue_OverflowsAndUnderflows_Throws()
        {
            var cq = new CircularQueue<int>(2);
            cq.Enqueue(1);
            cq.Enqueue(2);
            Assert.Throws<InvalidOperationException>(() => cq.Enqueue(3));

            cq.Dequeue();
            cq.Dequeue();
            Assert.Throws<InvalidOperationException>(() => cq.Dequeue());
        }

        [Fact]
        public void CircularQueue_InvalidCapacity_Throws()
        {
            Assert.Throws<ArgumentException>(() => new CircularQueue<int>(0));
        }
    }

    public class TwoStackQueueTests
    {
        [Fact]
        public void TwoStackQueue_EnqueueDequeue_WorksCorrectly()
        {
            var q = new TwoStackQueue<int>();
            Assert.True(q.IsEmpty);

            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);
            Assert.False(q.IsEmpty);

            Assert.Equal(1, q.Dequeue());
            Assert.Equal(2, q.Peek());
        }

        [Fact]
        public void TwoStackQueue_EmptyQueue_Throws()
        {
            var q = new TwoStackQueue<string>();
            Assert.Throws<InvalidOperationException>(() => q.Dequeue());
            Assert.Throws<InvalidOperationException>(() => q.Peek());
        }
    }

    public class ExpressionValidatorTests
    {
        [Theory]
        [InlineData("()", true)]
        [InlineData("{[()]}", true)]
        [InlineData("{[(])}", false)]
        [InlineData("({[})]", false)]
        [InlineData("", true)]
        public void EvaluateBalancedParentheses_ReturnsExpected(string expr, bool expected)
        {
            var result = ExpressionValidator.EvaluateBalancedParentheses(expr);
            Assert.Equal(expected, result);
        }
    }

    public class QueueOperationsTests
    {
        [Fact]
        public void ReverseQueue_WorksCorrectly()
        {
            var queue = new Queue<int>(new[] { 1, 2, 3 });
            var reversed = QueueOperations.ReverseQueue(queue);

            Assert.Equal(3, reversed.Dequeue());
            Assert.Equal(2, reversed.Dequeue());
            Assert.Equal(1, reversed.Dequeue());
        }
    }

    public class DequeTests
    {
        [Fact]
        public void Deque_AddRemovePeek_WorksCorrectly()
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
        public void Deque_EmptyThrows()
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
        public void PriorityQueue_EnqueueDequeuePeek_WorksCorrectly()
        {
            var pq = new PriorityQueueWrapper<int, string>();
            pq.Enqueue("low", 5);
            pq.Enqueue("high", 1);

            Assert.Equal("high", pq.Peek());
            Assert.Equal("high", pq.Dequeue());
            Assert.Equal("low", pq.Dequeue());
            Assert.True(pq.IsEmpty);
        }

        [Fact]
        public void PriorityQueue_EmptyThrows()
        {
            var pq = new PriorityQueueWrapper<int, string>();
            Assert.Throws<InvalidOperationException>(() => pq.Dequeue());
            Assert.Throws<InvalidOperationException>(() => pq.Peek());
        }
    }
}
