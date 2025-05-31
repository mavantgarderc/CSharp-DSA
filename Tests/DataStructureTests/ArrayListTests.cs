namespace Modules.DataStructures.Tests
{
    public class ArrayListTests
    {
        [Fact]
        public void Constructor_Default_InitializesWithDefaultCapacity()
        {
            var list = new ArrayList<int>();
            Assert.Empty(list);
            Assert.True(list.Capacity >= 4);
        }

        [Fact]
        public void Constructor_WithCapacity_InitializesCorrectly()
        {
            var list = new ArrayList<string>(10);
            Assert.Empty(list);
            Assert.Equal(10, list.Capacity);
        }

        [Fact]
        public void Constructor_WithNegativeCapacity_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(-1));
        }

        [Fact]
        public void Constructor_FromCollection_CopiesElements()
        {
            var source = new List<int> { 1, 2, 3 };
            var list = new ArrayList<int>(source);
            Assert.Equal(3, list.Count);
            Assert.Equal(source[1], list[1]);
        }

        [Fact]
        public void Add_AppendsElement()
        {
            var list = new ArrayList<int>();
            list.Add(42);
            Assert.Single(list);
            Assert.Equal(42, list[0]);
        }

        [Fact]
        public void Indexer_GetSet_Works()
        {
            var list = new ArrayList<string>();
            list.Add("first");
            list[0] = "updated";
            Assert.Equal("updated", list[0]);
        }

        [Fact]
        public void Insert_InsertsAtCorrectIndex()
        {
            var list = new ArrayList<int>();
            list.Add(1);
            list.Add(3);
            list.Insert(1, 2);
            Assert.Equal(new[] { 1, 2, 3 }, list.ToArray());
        }

        [Fact]
        public void Remove_RemovesFirstOccurrence()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            bool removed = list.Remove(2);
            Assert.True(removed);
            Assert.Equal(new[] { 1, 3 }, list.ToArray());
        }

        [Fact]
        public void RemoveAt_RemovesCorrectIndex()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.RemoveAt(1);
            Assert.Equal(new[] { 1, 3 }, list.ToArray());
        }

        [Fact]
        public void Clear_EmptiesTheList()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.Clear();
            Assert.Empty(list);
        }

        [Fact]
        public void GetRange_ReturnsCorrectSublist()
        {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            var range = list.GetRange(1, 3);
            Assert.Equal(new[] { 2, 3, 4 }, range.ToArray());
        }

        [Fact]
        public void BinarySearch_FindsElement()
        {
            var list = new ArrayList<int> { 1, 3, 5, 7 };
            int index = list.BinarySearch(5);
            Assert.Equal(2, index);
        }

        [Fact]
        public void AddRange_AppendsAllElements()
        {
            var list = new ArrayList<int> { 1, 2 };
            list.AddRange(new[] { 3, 4 });
            Assert.Equal(new[] { 1, 2, 3, 4 }, list.ToArray());
        }

        [Fact]
        public void InsertRange_InsertsAllAtCorrectIndex()
        {
            var list = new ArrayList<int> { 1, 4 };
            list.InsertRange(1, new[] { 2, 3 });
            Assert.Equal(new[] { 1, 2, 3, 4 }, list.ToArray());
        }

        [Fact]
        public void RemoveRange_RemovesCorrectSubset()
        {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            list.RemoveRange(1, 3);
            Assert.Equal(new[] { 1, 5 }, list.ToArray());
        }

        [Fact]
        public void SetRange_ReplacesValues()
        {
            var list = new ArrayList<int> { 0, 0, 0, 0 };
            list.SetRange(1, new List<int> { 9, 8 });
            Assert.Equal(new[] { 0, 9, 8, 0 }, list.ToArray());
        }

        [Fact]
        public void Sort_SortsCorrectly()
        {
            var list = new ArrayList<int> { 3, 1, 2 };
            list.Sort();
            Assert.Equal(new[] { 1, 2, 3 }, list.ToArray());
        }

        [Fact]
        public void Enumerator_IteratesCorrectly()
        {
            var list = new ArrayList<string> { "a", "b", "c" };
            var result = new List<string>();
            foreach (var item in list)
                result.Add(item);

            Assert.Equal(new[] { "a", "b", "c" }, result);
        }

        [Fact]
        public void EnsureCapacity_ExpandsStorage()
        {
            var list = new ArrayList<int>(1);
            list.Add(1);
            list.EnsureCapacity(10);
            Assert.True(list.Capacity >= 10);
        }

        [Fact]
        public void ToArray_CopiesContents()
        {
            var list = new ArrayList<int> { 10, 20 };
            var arr = list.ToArray();
            Assert.Equal(new[] { 10, 20 }, arr);
        }

        [Fact]
        public void TrimToSize_ShrinksCapacity()
        {
            var list = new ArrayList<int>(16) { 1, 2, 3 };
            list.TrimToSize();
            Assert.Equal(3, list.Capacity);
        }

        [Fact]
        public void Reverse_ReversesList()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.Reverse();
            Assert.Equal(new[] { 3, 2, 1 }, list.ToArray());
        }

        [Fact]
        public void Capacity_SetLessThanCount_Throws()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Capacity = 2);
        }

        [Fact]
        public void Enumerator_DetectsModification_Throws()
        {
            var list = new ArrayList<int> { 1, 2, 3 };
            var enumerator = list.GetEnumerator();
            list.Add(4); // change version

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }
    }
}
