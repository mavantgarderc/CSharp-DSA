using Modules.DataStructures;

namespace Tests
{
    public class ArrayListTests
    {
        private static readonly int[] expectedArray_123 = [1, 2, 3];
        private static readonly int[] expectedArray_13 = [1, 3];
        private static readonly int[] expectedArray_234 = [2, 3, 4];
        private static readonly int[] collection_34 = [3, 4];
        private static readonly int[] expectedArray_1234 = [1, 2, 3, 4];
        private static readonly int[] expectedArray_15 = [1, 5];
        private static readonly int[] expectedArray_0980 = [0, 9, 8, 0];
        private static readonly int[] expectedArray_321 = [3, 2, 1];
        private static readonly string[] expectedArray_abc = ["a", "b", "c"];
        private static readonly int[] expectedArray_1020 = [10, 20];

        [Fact]
        public void Constructor_Default_InitializesWithDefaultCapacity() {
            var list = new ArrayList<int>();
            Assert.Empty(list);
            Assert.True(list.Capacity >= 4);
        }
        [Fact]
        public void Constructor_WithCapacity_InitializesCorrectly() {
            var list = new ArrayList<string>(10);
            Assert.Empty(list);
            Assert.Equal(10, list.Capacity);
        }
        [Fact]
        public void Constructor_WithNegativeCapacity_Throws() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList<int>(-1));
        }
        [Fact]
        public void Constructor_FromCollection_CopiesElements() {
            var source = new List<int> { 1, 2, 3 };
            var list = new ArrayList<int>(source);
            Assert.Equal(3, list.Count);
            Assert.Equal(source[1], list[1]);
        }
        [Fact]
        public void Add_AppendsElement() {
            var list = new ArrayList<int> { 42 };
            Assert.Single(list);
            Assert.Equal(42, list[0]);
        }
        [Fact]
        public void Indexer_GetSet_Works() {
            var list = new ArrayList<string> { "first" };
            list[0] = "updated";
            Assert.Equal("updated", list[0]);
        }
        [Fact]
        public void Insert_InsertsAtCorrectIndex() {
            var list = new ArrayList<int> { 1, 3 };
            list.Insert(1, 2);
            Assert.Equal(expectedArray_123, list.ToArray());
        }
        [Fact]
        public void Remove_RemovesFirstOccurrence() {
            var list = new ArrayList<int> { 1, 2, 3 };
            bool removed = list.Remove(2);
            Assert.True(removed);
            Assert.Equal(expectedArray_13, list.ToArray());
        }
        [Fact]
        public void RemoveAt_RemovesCorrectIndex() {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.RemoveAt(1);
            Assert.Equal(expectedArray_13, list.ToArray());
        }
        [Fact]
        public void Clear_EmptiesTheList() {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.Clear();
            Assert.Empty(list);
        }
        [Fact]
        public void GetRange_ReturnsCorrectSublist() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            var range = list.GetRange(1, 3);
            Assert.Equal(expectedArray_234, range.ToArray());
        }
        [Fact]
        public void BinarySearch_FindsElement() {
            var list = new ArrayList<int> { 1, 3, 5, 7 };
            int index = list.BinarySearch(5);
            Assert.Equal(2, index);
        }
        [Fact]
        public void AddRange_AppendsAllElements() {
            var list = new ArrayList<int> { 1, 2 };
            list.AddRange(collection_34);
            Assert.Equal(expectedArray_1234, list.ToArray());
        }
        [Fact]
        public void InsertRange_InsertsAllAtCorrectIndex() {
            var list = new ArrayList<int> { 1, 4 };
            list.InsertRange(1, [2, 3]);
            Assert.Equal(expectedArray_1234, list.ToArray());
        }
        [Fact]
        public void RemoveRange_RemovesCorrectSubset() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            list.RemoveRange(1, 3);
            Assert.Equal(expectedArray_15, list.ToArray());
        }
        [Fact]
        public void SetRange_ReplacesValues() {
            var list = new ArrayList<int> { 0, 0, 0, 0 };
            list.SetRange(1, new List<int> { 9, 8 });
            Assert.Equal(expectedArray_0980, list.ToArray());
        }
        [Fact]
        public void Sort_SortsCorrectly() {
            var list = new ArrayList<int> { 3, 1, 2 };
            list.Sort();
            Assert.Equal(expectedArray_123, list.ToArray());
        }
        [Fact]
        public void Enumerator_IteratesCorrectly() {
            var list = new ArrayList<string> { "a", "b", "c" };
            var result = new List<string>();
            foreach (var item in list)
                result.Add(item);

            Assert.Equal(expectedArray_abc, result);
        }
        [Fact]
        public void EnsureCapacity_ExpandsStorage() {
            var list = new ArrayList<int>(1);
            list.Add(1);
            list.EnsureCapacity(10);
            Assert.True(list.Capacity >= 10);
        }
        [Fact]
        public void ToArray_CopiesContents() {
            var list = new ArrayList<int> { 10, 20 };
            var arr = list.ToArray();
            Assert.Equal(expectedArray_1020, arr);
        }
        [Fact]
        public void TrimToSize_ShrinksCapacity() {
            var list = new ArrayList<int>(16) { 1, 2, 3 };
            list.TrimToSize();
            Assert.Equal(3, list.Capacity);
        }
        [Fact]
        public void Reverse_ReversesList() {
            var list = new ArrayList<int> { 1, 2, 3 };
            list.Reverse();
            Assert.Equal(expectedArray_321, list.ToArray());
        }
        [Fact]
        public void Capacity_SetLessThanCount_Throws() {
            var list = new ArrayList<int> { 1, 2, 3 };
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Capacity = 2);
        }
        [Fact]
        public void Enumerator_DetectsModification_Throws() {
            var list = new ArrayList<int> { 1, 2, 3 };
            var enumerator = list.GetEnumerator();
            list.Add(4); // change version

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        // New tests for added functionality
        [Fact]
        public void LastIndexOf_FindsLastOccurrence() {
            var list = new ArrayList<int> { 1, 2, 3, 2, 4 };
            int index = list.LastIndexOf(2);
            Assert.Equal(3, index);
        }

        [Fact]
        public void LastIndexOf_WithStartIndex_FindsCorrectOccurrence() {
            var list = new ArrayList<int> { 1, 2, 3, 2, 4 };
            int index = list.LastIndexOf(2, 2);
            Assert.Equal(1, index);
        }

        [Fact]
        public void LastIndexOf_WithStartIndexAndCount_FindsCorrectOccurrence() {
            var list = new ArrayList<int> { 1, 2, 3, 2, 4 };
            int index = list.LastIndexOf(2, 3, 3);
            Assert.Equal(3, index);
        }

        [Fact]
        public void Reverse_WithRange_ReversesPartialList() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            list.Reverse(1, 3);
            Assert.Equal([1, 4, 3, 2, 5], list.ToArray());
        }

        [Fact]
        public void BinarySearch_WithRange_FindsElementInRange() {
            var list = new ArrayList<int> { 1, 3, 5, 7, 9 };
            int index = list.BinarySearch(1, 3, 5, Comparer<int>.Default);
            Assert.Equal(2, index);
        }

        [Fact]
        public void Sort_WithRange_SortsPartialList() {
            var list = new ArrayList<int> { 5, 3, 1, 4, 2 };
            list.Sort(1, 3, Comparer<int>.Default);
            Assert.Equal([5, 1, 3, 4, 2], list.ToArray());
        }

        [Fact]
        public void FindIndex_FindsFirstMatchingElement() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            int index = list.FindIndex(x => x > 3);
            Assert.Equal(3, index);
        }

        [Fact]
        public void FindIndex_WithStartIndex_FindsFromPosition() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            int index = list.FindIndex(2, x => x > 3);
            Assert.Equal(3, index);
        }

        [Fact]
        public void FindIndex_WithStartIndexAndCount_FindsInRange() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5 };
            int index = list.FindIndex(0, 3, x => x > 2);
            Assert.Equal(2, index);
        }

        [Fact]
        public void Find_ReturnsFirstMatchingElement() {
            var list = new ArrayList<string> { "apple", "banana", "cherry" };
            string? result = list.Find(x => x.StartsWith("b"));
            Assert.Equal("banana", result);
        }

        [Fact]
        public void Find_ReturnsDefaultWhenNotFound() {
            var list = new ArrayList<int> { 1, 2, 3 };
            int result = list.Find(x => x > 5);
            Assert.Equal(0, result);
        }

        [Fact]
        public void Find_EmptyList_ReturnsDefault() {
            var list = new ArrayList<int>();
            int result = list.Find(x => x > 0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void FindAll_ReturnsAllMatchingElements() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5, 6 };
            var result = list.FindAll(x => x % 2 == 0);
            Assert.Equal([2, 4, 6], result.ToArray());
        }

        [Fact]
        public void FindAll_NoMatches_ReturnsEmptyList() {
            var list = new ArrayList<int> { 1, 3, 5 };
            var result = list.FindAll(x => x % 2 == 0);
            Assert.Empty(result);
        }

        [Fact]
        public void Exists_ReturnsTrueWhenElementExists() {
            var list = new ArrayList<int> { 1, 2, 3 };
            bool exists = list.Exists(x => x == 2);
            Assert.True(exists);
        }

        [Fact]
        public void Exists_ReturnsFalseWhenElementDoesNotExist() {
            var list = new ArrayList<int> { 1, 2, 3 };
            bool exists = list.Exists(x => x == 5);
            Assert.False(exists);
        }

        [Fact]
        public void TrueForAll_ReturnsTrueWhenAllMatch() {
            var list = new ArrayList<int> { 2, 4, 6 };
            bool allEven = list.TrueForAll(x => x % 2 == 0);
            Assert.True(allEven);
        }

        [Fact]
        public void TrueForAll_ReturnsFalseWhenNotAllMatch() {
            var list = new ArrayList<int> { 2, 3, 6 };
            bool allEven = list.TrueForAll(x => x % 2 == 0);
            Assert.False(allEven);
        }

        [Fact]
        public void TrueForAll_EmptyList_ReturnsTrue() {
            var list = new ArrayList<int>();
            bool result = list.TrueForAll(x => x > 0);
            Assert.True(result);
        }

        [Fact]
        public void ForEach_ExecutesActionOnAllElements() {
            var list = new ArrayList<int> { 1, 2, 3 };
            var sum = 0;
            list.ForEach(x => sum += x);
            Assert.Equal(6, sum);
        }

        [Fact]
        public void ForEach_EmptyList_DoesNothing() {
            var list = new ArrayList<int>();
            var executed = false;
            list.ForEach(x => executed = true);
            Assert.False(executed);
        }

        [Fact]
        public void ForEach_DetectsModification_Throws() {
            var list = new ArrayList<int> { 1, 2, 3 };
            Assert.Throws<InvalidOperationException>(() => 
                list.ForEach(x => { if (x == 2) list.Add(4); }));
        }

        [Fact]
        public void RemoveAll_RemovesMatchingElements() {
            var list = new ArrayList<int> { 1, 2, 3, 4, 5, 6 };
            int removed = list.RemoveAll(x => x % 2 == 0);
            Assert.Equal(3, removed);
            Assert.Equal([1, 3, 5], list.ToArray());
        }

        [Fact]
        public void RemoveAll_NoMatches_ReturnsZero() {
            var list = new ArrayList<int> { 1, 3, 5 };
            int removed = list.RemoveAll(x => x % 2 == 0);
            Assert.Equal(0, removed);
            Assert.Equal([1, 3, 5], list.ToArray());
        }

        [Fact]
        public void RemoveAll_RemoveAll_EmptiesList() {
            var list = new ArrayList<int> { 1, 2, 3 };
            int removed = list.RemoveAll(x => true);
            Assert.Equal(3, removed);
            Assert.Empty(list);
        }

        [Fact]
        public void NonGenericEnumerator_Works() {
            var list = new ArrayList<int> { 1, 2, 3 };
            System.Collections.IEnumerable nonGeneric = list;
            var enumerator = nonGeneric.GetEnumerator();
            
            var results = new List<object>();
            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current);
            }
            
            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(3, results[2]);
        }

        [Fact]
        public void GetRange_EmptyRange_ReturnsEmptyList() {
            var list = new ArrayList<int> { 1, 2, 3 };
            var range = list.GetRange(1, 0);
            Assert.Empty(range);
        }

        [Fact]
        public void ToArray_EmptyList_ReturnsEmptyArray() {
            var list = new ArrayList<int>();
            var array = list.ToArray();
            Assert.Empty(array);
        }

        [Fact]
        public void Constructor_FromEmptyCollection_CreatesEmptyList() {
            var list = new ArrayList<int>(new List<int>());
            Assert.Empty(list);
            Assert.Equal(0, list.Capacity);
        }
    }
}
