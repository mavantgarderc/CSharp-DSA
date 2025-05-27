using Modules.DataStructures;

namespace Tests
{
    public class ListTTests
    {
        [Fact]
        public void Add_ElementsToList()
        {
            var list = new List<int>();
            ListT.Add(list, 5);
            Assert.Single(list);
            Assert.Equal(5, list[0]);
        }

        [Fact]
        public void InsertAt_InsertAtCorrectPosition()
        {
            var list = new List<char> { 'a', 'b', 'd' };
            ListT.InsertAt(list, 2, 'c');
            Assert.Equal('c', list[2]);
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public void InsertAt_InvalidIndex_ThrowsException()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.Throws<ArgumentException>(() => ListT.InsertAt(list, -1, 4));
            Assert.Throws<ArgumentException>(() => ListT.InsertAt(list, 4, 4));
        }

        [Fact]
        public void RemoveAt_RemoveCorrectElement()
        {
            var list = new List<int> { 1, 2, 3 };
            ListT.RemoveAt(list, 1);
            Assert.Equal(new List<int> { 1, 3 }, list);
        }

        [Fact]
        public void RemoveAt_InvalidIndex_ThrowsException()
        {
            var list = new List<int> { 1, 2 };
            Assert.Throws<ArgumentException>(() => ListT.RemoveAt(list, 2));
        }

        [Fact]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new List<char> { 'x', 'y', 'z' };
            var index = ListT.IndexOf(list, 'y');
            Assert.Equal(1, index);
        }

        [Fact]
        public void SortAscending_SortsCorrerctly()
        {
            var list = new List<int> { 3, 1, 2 };
            ListT.SortAscending(list);
            Assert.Equal(new List<int> { 1, 2, 3 }, list);
        }

        [Fact]
        public void SortDescending_SortsCorrectly()
        {
            var list = new List<int> { 1, 2, 3 };
            ListT.SortDescending(list);
            Assert.Equal(new List<int> { 3, 2, 1 }, list);
        }

        [Fact]
        public void Reverse_ReversesList()
        {
            var list = new List<char> { 'a', 'b', 'c' };
            ListT.Reverse(list);
            Assert.Equal(new List<char> { 'c', 'b', 'a' }, list);
        }

        [Fact]
        public void Clone_ReturnsNewListWithSameElements()
        {
            var original = new List<int> { 10, 20 };
            var clone = ListT.Clone(original);
            Assert.Equal(original, clone);
            Assert.NotSame(original, clone);
        }

        [Fact]
        public void Merge_CombineBothLists()
        {
            var a = new List<int> { 1, 2 };
            var b = new List<int> { 3, 4 };
            var merged = ListT.Merge(a, b);
            Assert.Equal(new List<int> { 1, 2, 3, 4 }, merged);
        }


        [Fact]
        public void Operations_NullInput_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ListT.Add<int>(null!, 1));
            Assert.Throws<ArgumentException>(() => ListT.InsertAt<int>(null!, 0, 1));
            Assert.Throws<ArgumentException>(() => ListT.RemoveAt<int>(null!, 0));
            Assert.Throws<ArgumentException>(() => ListT.SortAscending<int>(null!));
            Assert.Throws<ArgumentException>(() => ListT.SortDescending<int>(null!));
            Assert.Throws<ArgumentException>(() => ListT.Reverse<int>(null!));
            Assert.Throws<ArgumentException>(() => ListT.Clone<int>(null!));
            Assert.Throws<ArgumentException>(() => ListT.Merge<int>(null!, new List<int>()));
            Assert.Throws<ArgumentException>(() => ListT.Merge<int>(new List<int>(), null!));
        }
    }
}
