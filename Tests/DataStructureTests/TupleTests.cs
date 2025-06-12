using Modules.DataStructures;

namespace Tests
{
    public class TupleUtilsTests
    {
        [Fact]
        public void Create_ReturnsCorrectTuple()
        {
            var tuple = TupleUtils.Create(1, "a");
            Assert.Equal(1, tuple.Item1);
            Assert.Equal("a", tuple.Item2);
        }

        [Fact]
        public void CreateValueTuple_ReturnsCorrectValueTuple()
        {
            var tuple = TupleUtils.CreateValueTuple(1, "a");
            Assert.Equal((1, "a"), tuple);
        }

        [Fact]
        public void ToValueTuple_ConvertsFromTuple()
        {
            var tuple = new Tuple<int, string>(1, "a");
            var result = TupleUtils.ToValueTuple(tuple);
            Assert.Equal((1, "a"), result);
        }

        [Fact]
        public void ToTuple_ShouldConvertValueTupleToTuple()
        {
            var input = (1, "a");
            var result = TupleUtils.ToTuple(input);

            Assert.Equal(1, result.Item1);
            Assert.Equal("a", result.Item2);
        }

        [Fact]
        public void Map_ValueTuple_TransformsBothItems()
        {
            var input = (2, 3);
            var result = input.Map(x => x + 1, y => y * 2);
            Assert.Equal((3, 6), result);
        }

        [Fact]
        public void Map_ReferenceTuple_TransformsBothItems()
        {
            var input = Tuple.Create(2, 3);
            var result = input.Map(x => x + 1, y => y * 2);
            Assert.Equal(3, result.Item1);
            Assert.Equal(6, result.Item2);
        }

        [Fact]
        public void Flatten_ValueTuple_FlattensProperly()
        {
            var nested = ((1, 2), 3);
            var flat = nested.Flatten();
            Assert.Equal((1, 2, 3), flat);
        }

        [Fact]
        public void Flatten_ReferenceTuple_FlattensProperly()
        {
            var nested = Tuple.Create(Tuple.Create(1, 2), 3);
            var flat = nested.Flatten();
            Assert.Equal(1, flat.Item1);
            Assert.Equal(2, flat.Item2);
            Assert.Equal(3, flat.Item3);
        }

        [Fact]
        public void ZipToValueTuples_ZipsCorrectly()
        {
            var first = new[] { 1, 2 };
            var second = new[] { "a", "b" };
            var result = TupleUtils.ZipToValueTuples(first, second).ToList();
            Assert.Equal((1, "a"), result[0]);
            Assert.Equal((2, "b"), result[1]);
        }

        [Fact]
        public void ZipToTuples_ZipsCorrectly()
        {
            var first = new[] { 1, 2 };
            var second = new[] { "a", "b" };
            var result = TupleUtils.ZipToTuples(first, second).ToList();
            Assert.Equal(1, result[0].Item1);
            Assert.Equal("a", result[0].Item2);
        }

        [Fact]
        public void StructuralEquals_ValueTuple_TrueForEqual()
        {
            var a = (1, "x");
            var b = (1, "x");
            Assert.True(TupleUtils.StructuralEquals(a, b));
        }

        [Fact]
        public void StructuralEquals_ReferenceTuple_FalseForMismatch()
        {
            var a = Tuple.Create(1, "x");
            var b = Tuple.Create(2, "x");
            Assert.False(TupleUtils.StructuralEquals(a, b));
        }

        [Fact]
        public void Swap_ValueTuple_SwapsElements()
        {
            var input = (1, "a");
            var swapped = input.Swap();
            Assert.Equal(("a", 1), swapped);
        }

        [Fact]
        public void Swap_ReferenceTuple_SwapsElements()
        {
            var input = Tuple.Create(1, "a");
            var swapped = input.Swap();
            Assert.Equal("a", swapped.Item1);
            Assert.Equal(1, swapped.Item2);
        }

        [Fact]
        public void Apply_ExecutesActionOnTuple()
        {
            int result = 0;
            TupleUtils.Apply((2, 3), (x, y) => result = x + y);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Select_ProjectsTupleToResult()
        {
            var result = TupleUtils.Select((2, 3), (x, y) => x * y);
            Assert.Equal(6, result);
        }

        [Fact]
        public void TransformAll_MapsBothItems()
        {
            var result = TupleUtils.TransformAll((3, 4), x => x * 10);
            Assert.Equal((30, 40), result);
        }

        [Fact]
        public void ToDictionary_ConvertsToDict_WhenNotThrowing()
        {
            var data = new List<(int, string)> { (1, "a"), (2, "b") };
            var dict = data.ToDictionary();
            Assert.Equal("a", dict[1]);
        }

        [Fact]
        public void ToDictionary_ThrowsWhenItem2IsNull_IfThrowIfNullTrue()
        {
            var data = new List<(int, string?)> { (1, null) };
            var ex = Assert.Throws<InvalidOperationException>(() => data.ToDictionary(true));
            Assert.Equal("Tupble.Item2 is null.", ex.Message);
        }

        [Fact]
        public void RotateLeft_ShiftsCorrectly()
        {
            var result = TupleUtils.RotateLeft((1, 2, 3));
            Assert.Equal((2, 3, 1), result);
        }

        [Fact]
        public void RotateRight_ShiftsCorrectly()
        {
            var result = TupleUtils.RotateRight((1, 2, 3));
            Assert.Equal((3, 1, 2), result);
        }

        [Fact]
        public void Duplicate_CreatesRepeatedTuple()
        {
            var result = TupleUtils.Duplicate((1, 2));
            Assert.Equal((1, 2, 1, 2), result);
        }

        [Fact]
        public void Contains_IdentifiesPresence()
        {
            var tuple = (1, "x");
            Assert.True(tuple.Contains("x"));
            Assert.False(tuple.Contains("y"));
        }

        [Fact]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var tuple = (1, "a");
            Assert.Equal(0, tuple.IndexOf(1));
            Assert.Equal(1, tuple.IndexOf("a"));
            Assert.Equal(-1, tuple.IndexOf("z"));
        }
    }
}
