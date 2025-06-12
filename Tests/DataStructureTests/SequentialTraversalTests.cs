using Modules.DataStructures;

namespace Tests
{
    public class SequentialTraversalTests
    {
        private string CaptureOutput(Action action)
        {
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            action();

            Console.SetOut(originalOut);
            return sw.ToString().Trim();
        }

        [Fact]
        public void TestTraverseArray()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseArray(new[] { 1, 2, 3 }));
            Assert.Equal("1\n2\n3", Normalize(output));
        }

        [Fact]
        public void TestTraverseList()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseList(new List<int> { 4, 5 }));
            Assert.Equal("4\n5", Normalize(output));
        }

        [Fact]
        public void TestTraverseLinkedList()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseLinkedList(new LinkedList<int>(new[] { 7, 8 })));
            Assert.Equal("7\n8", Normalize(output));
        }

        [Fact]
        public void TestTraverse2DArray()
        {
            var st = new SequentialTraversal();
            var array = new int[,] { { 1, 2 }, { 3, 4 } };
            var output = CaptureOutput(() => st.Traverse2DArray(array));
            Assert.Equal("1\n2\n3\n4", Normalize(output));
        }

        [Fact]
        public void TestTraverseJaggedArray()
        {
            var st = new SequentialTraversal();
            var jagged = new[] {
                new[] { 1, 2 },
                new[] { 3 }
            };
            var output = CaptureOutput(() => st.TraverseJaggedArray(jagged));
            Assert.Equal("1\n2\n3", Normalize(output));
        }

        [Fact]
        public void TestTraverseWithIndex()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseWithIndex(new[] { "a", "b" }));
            Assert.Equal("[0] = a\n[1] = b", Normalize(output));
        }

        [Fact]
        public void TestTraverseReverse()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TravereReverse(new List<string> { "x", "y", "z" }));
            Assert.Equal("z\ny\nx", Normalize(output));
        }

        [Fact]
        public void TestTraverseStack()
        {
            var st = new SequentialTraversal();
            var stack = new Stack<int>(new[] { 1, 2 }); // Top: 2
            var output = CaptureOutput(() => st.TraverseStack(stack));
            Assert.Equal("2\n1", Normalize(output));
        }

        [Fact]
        public void TestTraverseQueue()
        {
            var st = new SequentialTraversal();
            var queue = new Queue<int>(new[] { 9, 10 });
            var output = CaptureOutput(() => st.TraverseQueue(queue));
            Assert.Equal("9\n10", Normalize(output));
        }

        [Fact]
        public void TestTraverseDictionary()
        {
            var st = new SequentialTraversal();
            var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var output = CaptureOutput(() => st.TraverseDictionary(dict));
            Assert.Contains("a: 1", output);
            Assert.Contains("b: 2", output);
        }

        [Fact]
        public void TestTraverseSet()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseSet(new HashSet<string> { "foo", "bar" }));
            Assert.Contains("foo", output);
            Assert.Contains("bar", output);
        }

        [Fact]
        public void TestTraverseSpan()
        {
            var st = new SequentialTraversal();
            var array = new[] { 5, 6, 7 };
            var output = CaptureOutput(() => st.TraverseSpan(array));
            Assert.Equal("5\n6\n7", Normalize(output));
        }

        [Fact]
        public void TestTraverseString()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseString("hi"));
            Assert.Equal("h\ni", Normalize(output));
        }

        [Fact]
        public void TestTraverseEnumerator()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseEnumerator(new[] { "alpha", "beta" }));
            Assert.Equal("alpha\nbeta", Normalize(output));
        }

        [Fact]
        public void TestTraverseParallel()
        {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => st.TraverseParallel(new[] { "a", "b", "c" }));
            // Cannot guarantee order; only verify contents
            Assert.Contains("a", output);
            Assert.Contains("b", output);
            Assert.Contains("c", output);
        }

        [Fact]
        public void TestTraverseCircularBuffer()
        {
            var buffer = new[] { "A", "B", "C", "D" };
            var output = CaptureOutput(() => SequentialTraversal.TraversCircularBuffer(buffer, 2, 3));
            // Assumes (2+1)%4 = 3, then 4%4 = 0, then 5%4 = 1 → D, A, B
            Assert.Equal("D\nA\nB", Normalize(output));
        }

        private string Normalize(string s) => s.Replace("\r", "").Trim();
    }
}
