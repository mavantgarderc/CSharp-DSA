using Modules.Algorithms;

namespace Tests.AlgorithmsTests
{
    public class StreamProcessingTests
    {
        private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                await Task.Yield();
                yield return item;
            }
        }

        [Fact]
        public void Map_TransformsElements() {
            var source = new[] { 1, 2, 3 };
            var result = StreamProcessing.Map(source, x => x * 2).ToList();

            Assert.Equal([2, 4, 6], result);
        }
        [Fact]
        public void Filter_FiltersElements() {
            var source = new[] { 1, 2, 3, 4 };
            var result = StreamProcessing.Filter(source, x => x % 2 == 0).ToList();

            Assert.Equal([2, 4], result);
        }
        [Fact]
        public void Window_BatchesElements() {
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = StreamProcessing.Window(source, 2).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal([1, 2], result[0]);
            Assert.Equal([3, 4], result[1]);
            Assert.Equal([5], result[2]);
        }
        [Fact]
        public void Reduce_SumsElements() {
            var source = new[] { 1, 2, 3 };
            var result = StreamProcessing.Reduce(source, 0, (acc, x) => acc + x);

            Assert.Equal(6, result);
        }
        [Fact]
        public void DistinctBy_ReturnsDistinctItems() {
            var source = new[] { "a", "aa", "b", "bb", "ccc" };
            var result = StreamProcessing.DistinctBy(source, s => s.Length).ToList();

            Assert.Contains("a", result);
            Assert.Contains("aa", result);
            Assert.Contains("ccc", result);
            Assert.Equal(3, result.Count);
        }
        [Fact]
        public void Pairwise_ReturnsConsecutivePairs() {
            var source = new[] { 1, 2, 3 };
            var result = StreamProcessing.Pairwise(source).ToList();

            Assert.Equal([(1, 2), (2, 3)], result);
        }
        [Fact]
        public void Chunk_GroupsElements() {
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = StreamProcessing.Chunk(source, 2).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal([1, 2], result[0]);
            Assert.Equal([3, 4], result[1]);
            Assert.Equal([5], result[2]);
        }
        [Fact]
        public void TakeUntil_StopsAtPredicate() {
            var source = new[] { 1, 2, 3, 4, 5 };
            var result = StreamProcessing.TakeUntil(source, x => x == 3).ToList();

            Assert.Equal([1, 2, 3], result);
        }
        [Fact]
        public void Skip_SkipsElements() {
            var source = new[] { 1, 2, 3, 4 };
            var result = StreamProcessing.Skip(source, 2).ToList();

            Assert.Equal([3, 4], result);
        }
        [Fact]
        public void Take_TakesElements() {
            var source = new[] { 1, 2, 3, 4 };
            var result = StreamProcessing.Take(source, 2).ToList();

            Assert.Equal([1, 2], result);
        }
        [Fact]
        public void ForEach_PerformsAction() {
            var source = new[] { 1, 2, 3 };
            int sum = 0;
            StreamProcessing.ForEach(source, x => sum += x);

            Assert.Equal(6, sum);
        }

        private static readonly int[] int32Array_123 = [1, 2, 3];
        private static readonly int[] int32Array_1234 = [1, 2, 3, 4];
        private static readonly string[] stringArray = ["a", "aa", "b", "bb", "ccc"];
        private static readonly int[] int32Array_12345 = [1, 2, 3, 4, 5];

        [Fact]
        public async Task MapAsync_TransformsElements() {
            var source = ToAsyncEnumerable(int32Array_123);
            var result = new List<int>();
            await foreach (var x in StreamProcessing.MapAsync(source, y => y * 2))
                result.Add(x);

            Assert.Equal([2, 4, 6], result);
        }
        [Fact]
        public async Task FilterAsync_FiltersElements() {
            var source = ToAsyncEnumerable(int32Array_1234);
            var result = new List<int>();
            await foreach (var x in StreamProcessing.FilterAsync(source, y => y % 2 == 0))
                result.Add(x);

            Assert.Equal([2, 4], result);
        }
        [Fact]
        public async Task ReduceAsync_SumsElements() {
            var source = ToAsyncEnumerable(int32Array_123);
            var sum = await StreamProcessing.ReduceAsync(source, 0, (acc, x) => acc + x);

            Assert.Equal(6, sum);
        }
        [Fact]
        public async Task DistinctByAsync_ReturnsDistinctItems() {
            var source = ToAsyncEnumerable(stringArray);
            var result = new List<string>();
            await foreach (var s in StreamProcessing.DistinctBy(source, x => x.Length))
                result.Add(s);

            Assert.Contains("a", result);
            Assert.Contains("aa", result);
            Assert.Contains("ccc", result);
            Assert.Equal(3, result.Count);
        }
        [Fact]
        public async Task PairwiseAsync_ReturnsConsecutivePairs() {
            var source = ToAsyncEnumerable(int32Array_123);
            var result = new List<(int, int)>();
            await foreach (var pair in StreamProcessing.PairwiseAsync(source))
                result.Add(pair);

            Assert.Equal([(1, 2), (2, 3)], result);
        }
        [Fact]
        public async Task ChunkAsync_GroupsElements() {
            var source = ToAsyncEnumerable(int32Array_12345);
            var result = new List<List<int>>();
            await foreach (var chunk in StreamProcessing.ChunkAsync(source, 2))
                result.Add(chunk);

            Assert.Equal(3, result.Count);
            Assert.Equal([1, 2], result[0]);
            Assert.Equal([3, 4], result[1]);
            Assert.Equal([5], result[2]);
        }
        [Fact]
        public async Task TakeUntilAsync_StopsAtPredicate() {
            var source = ToAsyncEnumerable(int32Array_12345);
            var result = new List<int>();
            await foreach (var x in StreamProcessing.TakeUntilAsync(source, y => y == 3))
                result.Add(x);

            Assert.Equal([1, 2, 3], result);
        }
        [Fact]
        public async Task SkipAsync_SkipsElements() {
            var source = ToAsyncEnumerable(int32Array_1234);
            var result = new List<int>();
            await foreach (var x in StreamProcessing.SkipAsync(source, 2))
                result.Add(x);

            Assert.Equal([3, 4], result);
        }
        [Fact]
        public async Task TakeAsync_TakesElements() {
            var source = ToAsyncEnumerable(int32Array_1234);
            var result = new List<int>();
            await foreach (var x in StreamProcessing.TakeAsync(source, 2))
                result.Add(x);

            Assert.Equal([1, 2], result);
        }
        [Fact]
        public async Task ForEachAsync_PerformsAction() {
            var source = ToAsyncEnumerable(int32Array_123);
            int sum = 0;
            await StreamProcessing.ForEachAsync(source, x => sum += x);

            Assert.Equal(6, sum);
        }
    }
}
