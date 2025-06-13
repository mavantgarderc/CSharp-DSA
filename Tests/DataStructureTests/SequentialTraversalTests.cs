using Modules.DataStructures;

namespace Tests
{
    public class SequentialTraversalTests
    {
        private static string Normalize(string s) => s.Replace("\r", "").Trim();
        private static string CaptureOutput(Action action)
        {
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            action();

            Console.SetOut(originalOut);
            return sw.ToString().Trim();
        }

        [Fact]
        public void TestTraverseArray() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseArray([1, 2, 3]));
            Assert.Equal("1\n2\n3", Normalize(output));
        }

        [Fact]
        public void TestTraverseList() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseList([4, 5]));
            Assert.Equal("4\n5", Normalize(output));
        }

        [Fact]
        public void TestTraverseLinkedList() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseLinkedList(new LinkedList<int>([7, 8])));
            Assert.Equal("7\n8", Normalize(output));
        }

        [Fact]
        public void TestTraverse2DArray() {
            var st = new SequentialTraversal();
            var array = new int[,] { { 1, 2 }, { 3, 4 } };
            var output = CaptureOutput(() => SequentialTraversal.Traverse2DArray(array));
            Assert.Equal("1\n2\n3\n4", Normalize(output));
        }

        [Fact]
        public void TestTraverseJaggedArray() {
            var st = new SequentialTraversal();
            var jagged = new[] {
                new[] { 1, 2 },
                new[] { 3 }
            };
            var output = CaptureOutput(() => SequentialTraversal.TraverseJaggedArray(jagged));
            Assert.Equal("1\n2\n3", Normalize(output));
        }

        [Fact]
        public void TestTraverseWithIndex() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseWithIndex(collection_ab));
            Assert.Equal("[0] = a\n[1] = b", Normalize(output));
        }

        [Fact]
        public void TestTraverseReverse() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TravereReverse(new List<string> { "x", "y", "z" }));
            Assert.Equal("z\ny\nx", Normalize(output));
        }

        [Fact]
        public void TestTraverseStack() {
            var st = new SequentialTraversal();
            var stack = new Stack<int>(collection_12); // Top: 2
            var output = CaptureOutput(() => SequentialTraversal.TraverseStack(stack));
            Assert.Equal("2\n1", Normalize(output));
        }

        [Fact]
        public void TestTraverseQueue() {
            var st = new SequentialTraversal();
            var queue = new Queue<int>([9, 10]);
            var output = CaptureOutput(() => SequentialTraversal.TraverseQueue(queue));
            Assert.Equal("9\n10", Normalize(output));
        }

        [Fact]
        public void TestTraverseDictionary() {
            var st = new SequentialTraversal();
            var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var output = CaptureOutput(() => SequentialTraversal.TraverseDictionary(dict));
            Assert.Contains("a: 1", output);
            Assert.Contains("b: 2", output);
        }

        [Fact]
        public void TestTraverseSet() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseSet(new HashSet<string> { "foo", "bar" }));
            Assert.Contains("foo", output);
            Assert.Contains("bar", output);
        }

        [Fact]
        public void TestTraverseSpan() {
            var st = new SequentialTraversal();
            var array = new[] { 5, 6, 7 };
            var output = CaptureOutput(() => SequentialTraversal.TraverseSpan(array));
            Assert.Equal("5\n6\n7", Normalize(output));
        }

        [Fact]
        public void TestTraverseString() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseString("hi"));
            Assert.Equal("h\ni", Normalize(output));
        }

        private static readonly string[] collection_alphabeta = ["alpha", "beta"];
        private static readonly string[] collection_abc = ["a", "b", "c"];
        private static readonly string[] collection_ab = ["a", "b"];
        private static readonly int[] collection_12 = [1, 2];

        [Fact]
        public void TestTraverseEnumerator() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseEnumerator(collection_alphabeta));
            Assert.Equal("alpha\nbeta", Normalize(output));
        }

        [Fact]
        public void TestTraverseParallel() {
            var st = new SequentialTraversal();
            var output = CaptureOutput(() => SequentialTraversal.TraverseParallel(collection_abc));

            Assert.Contains("a", output);
            Assert.Contains("b", output);
            Assert.Contains("c", output);
        }

        [Fact]
        public void TestTraverseCircularBuffer() {
            var buffer = new[] { "A", "B", "C", "D" };
            var output = CaptureOutput(() => SequentialTraversal.TraverseCircularBuffer(buffer, 2, 3));

            Assert.Equal("D\nA\nB", Normalize(output));
        }
    }
}
