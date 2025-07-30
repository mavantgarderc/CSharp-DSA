using Modules.Algorithms;

namespace Tests.AlgorithmTests
{
    public class LinearSearchTests
    {
        [Fact]
        public void IndexOf_FindsFirstMatch() {
            var collection = new[] { 1, 2, 3, 4, 5 };
            int index = collection.IndexOf(x => x == 4);

            Assert.Equal(3, index);
        }
        [Fact]
        public void IndexOf_ReturnsMinusOne_IfNotFound() {
            var collection = new[] { 1, 2, 3 };
            int index = collection.IndexOf(x => x == 10);

            Assert.Equal(-1, index);
        }
        [Fact]
        public void Contains_ReturnsTrue_IfMatch() {
            var collection = new[] { "foo", "bar" };

            Assert.True(collection.Contains(s => s == "foo"));
        }
        [Fact]
        public void AllMatches_FindsAll() {
            var collection = new[] { 1, 2, 3, 4, 5 };
            var matches = collection.AllMatches(x => x % 2 == 0);

            Assert.Equal(2, matches.Count);
            Assert.Contains(2, matches);
            Assert.Contains(4, matches);
        }
        [Fact]
        public void CountMatches_CountsCorrect() {
            var collection = new[] { 1, 2, 2, 3 };
            int count = collection.CountMatches(x => x == 2);

            Assert.Equal(2, count);
        }
        [Fact]
        public void FirstOrDefaultCustom_FindsFirst() {
            var collection = new[] { 1, 2, 3 };
            int first = collection.FirstOrDefaultCustom(x => x > 1);

            Assert.Equal(2, first);
        }
        [Fact]
        public void LastIndexOf_FindsLast() {
            var collection = new[] { 1, 2, 2, 4 };
            int lastIdx = collection.LastIndexOf(x => x == 2);

            Assert.Equal(2, lastIdx);
        }
        [Fact]
        public void FindAll_ReturnsAll() {
            var collection = new[] { 1, 2, 2, 3 };
            var all = collection.FindAll(x => x == 2);

            Assert.Equal(2, all.Count);
            Assert.All(all, item => Assert.Equal(2, item));
        }
        [Fact]
        public void AllMatchesCondition_ReturnsTrue_IfAllMatch() {
            var collection = new[] { 2, 4, 6 };

            Assert.True(collection.AllMatchesCondition(x => x % 2 == 0));
        }
        [Fact]
        public void AnyMatch_ReturnsTrue_IfAnyMatch() {
            var collection = new[] { 1, 2, 3 };

            Assert.True(collection.AnyMatch(x => x == 2));
        }
        [Fact]
        public void RemoveAll_RemovesCorrect() {
            var collection = new List<int> { 1, 2, 3, 2 };
            int removed = collection.RemoveAll(x => x == 2);

            Assert.Equal(2, removed);
            Assert.DoesNotContain(2, collection);
        }
        [Fact]
        public void SingleOrDefaultCustom_ReturnsSingle() {
            var collection = new[] { 1, 2, 3 };
            int match = collection.SingleOrDefaultCustom(x => x == 2);

            Assert.Equal(2, match);
        }
        [Fact]
        public void SingleOrDefaultCustom_ReturnsDefault_IfMultiple() {
            var collection = new[] { 1, 2, 2 };
            int match = collection.SingleOrDefaultCustom(x => x == 2);

            Assert.Equal(0, match);
        }
    }
}
