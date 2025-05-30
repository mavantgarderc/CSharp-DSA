namespace Modules.DataStructures.Tests
{
    public class LinkedListTests
    {
        private DoublyLinkedList<int> NewList(params int[] items)
        {
            var list = new DoublyLinkedList<int>();
            list.AddRange(items);
            return list;
        }

        [Fact]
        public void AddFirst_AddLast_ToArray_And_Count_And_Contains_Work()
        {
            var list = new DoublyLinkedList<int>();
            Assert.Equal(0, list.Count);

            list.AddFirst(2);
            list.AddFirst(1);
            list.AddLast(3);
            Assert.Equal(3, list.Count);

            var arr = list.ToArray();
            Assert.Equal(new[] { 1, 2, 3 }, arr);
            Assert.True(list.Contains(2));
            Assert.False(list.Contains(42));
        }

        [Fact]
        public void PeekFirst_PeekLast_And_RemoveMethods_Work_And_Throw_OnEmpty()
        {
            var list = NewList(10, 20, 30);
            Assert.Equal(10, list.PeekFirst());
            Assert.Equal(30, list.PeekLast());

            list.RemoveFirst();
            Assert.Equal(2, list.Count);
            Assert.Equal(20, list.PeekFirst());

            list.RemoveLast();
            Assert.Single(list);
            Assert.Equal(20, list.PeekLast());

            bool removed = list.Remove(20);
            Assert.True(removed);
            Assert.Empty(list);
            Assert.False(list.Remove(999));  // not found

            // removing from empty
            Assert.Throws<InvalidOperationException>(() => list.RemoveFirst());
            Assert.Throws<InvalidOperationException>(() => list.RemoveLast());
            Assert.Throws<InvalidOperationException>(() => list.PeekFirst());
            Assert.Throws<InvalidOperationException>(() => list.PeekLast());
        }

        [Fact]
        public void Clear_Empties_List()
        {
            var list = NewList(1,2,3);
            list.Clear();
            Assert.Empty(list);
            Assert.False(list.Contains(1));
        }

        [Fact]
        public void Reverse_And_ReverseIterator_Work()
        {
            var list = NewList(1,2,3,4);
            list.Reverse();
            Assert.Equal(new[] {4,3,2,1}, list.ToArray());

            var back = list.ReverseIterator().Cast<int>().ToArray();
            Assert.Equal(new[] {1,2,3,4}, back);
        }

        [Fact]
        public void Clone_Is_DeepCopy()
        {
            var original = NewList(5,6,7);
            var clone = (DoublyLinkedList<int>)original.Clone();

            // same sequence, different instance
            Assert.Equal(original.ToArray(), clone.ToArray());
            clone.AddLast(8);
            Assert.NotEqual(original.Count, clone.Count);
        }

        [Fact]
        public void AddRange_Appends_All_Items()
        {
            var list = new DoublyLinkedList<string>();
            list.AddRange(new[] { "a","b","c" });
            Assert.Equal(new[] { "a","b","c" }, list.ToArray());
        }

        [Theory]
        [InlineData(new[] {1,2,3}, 3, 1)]
        [InlineData(new[] {1,2,3,4}, 4, 2)]
        public void GetStructuralInfo_Returns_Correct_Length_And_Mid(int[] data, int expLen, int expMid)
        {
            var list = NewList(data);
            var (length, mid) = list.GetStructuralInfo();
            Assert.Equal(expLen, length);
            Assert.Equal(expMid, mid);
        }

        [Fact]
        public void GetNodeDepth_Finds_Correct_Index_Or_NegativeOne()
        {
            var list = NewList(10,20,30);
            Assert.Equal(0, list.GetNodeDepth(10));
            Assert.Equal(2, list.GetNodeDepth(30));
            Assert.Equal(-1, list.GetNodeDepth(99));
        }

        [Fact]
        public void Extensions_Map_Filter_Reduce_ForEach_Partition_Work_As_Expected()
        {
            var list = NewList(1,2,3,4,5);

            var squares = list.Map(x => x * x).Cast<int>().ToArray();
            Assert.Equal(new[] {1,4,9,16,25}, squares);

            var evens = list.Filter(x => x % 2 == 0).Cast<int>().ToArray();
            Assert.Equal(new[] {2,4}, evens);

            int sum = list.Reduce(0, (acc,v) => acc + v);
            Assert.Equal(15, sum);

            var collected = new System.Collections.Generic.List<int>();
            list.ForEach(collected.Add);
            Assert.Equal(list.ToArray(), collected.ToArray());

            var (left, right) = list.Partition(x => x <= 3);
            Assert.Equal(new[] {1,2,3}, left.ToArray());
            Assert.Equal(new[] {4,5}, right.ToArray());
        }
    }
}