using Modules.DataStructures;

namespace Tests
{
    public class StringTests
    {
        [Theory]
        [InlineData("abc", "cba")]
        [InlineData("racecar", "racecar")]
        [InlineData("", "")]
        [InlineData("a", "a")]
        public void Reverse_ShouldReturnReversedString(string input, string expected)
        {
            var result = Strings.Reverse(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("RaceCar", true)]
        [InlineData("A man a plan a canal Panama", true)]
        [InlineData("NotAPalindrome", false)]
        public void IsPalindrome_ShouldValidateCorrectly(string input, bool expected)
        {
            var result = Strings.IsPalindrome(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("listen", "silent", true)]
        [InlineData("hello", "world", false)]
        [InlineData("triangle", "integral", true)]
        [InlineData("rat", "car", false)]
        public void AreAnagrams_ShouldDetectCorrectly(string a, string b, bool expected)
        {
            var result = Strings.AreAnagrams(a, b);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("hello world", "world", 6)]
        [InlineData("abcde", "f", -1)]
        [InlineData("banana", "na", 2)]
        public void IndexOfSubstring_ShouldReturnCorrectIndex(string source, string target, int expected)
        {
            var result = Strings.IndexOfSubstring(source, target);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("swiss", 'w')]
        [InlineData("redivider", 'v')]
        [InlineData("aabbcc", null)]
        [InlineData("", null)]
        public void FirstUniqueChar_ShouldReturnCorrectChar(string input, char? expected)
        {
            var result = Strings.FirstUniqueChar(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("waterbottle", "erbottlewat", true)]
        [InlineData("rotation", "tationro", true)]
        [InlineData("hello", "elloh", true)]
        [InlineData("hello", "olelh", false)]
        [InlineData("abc", "abcd", false)]
        [InlineData("", "abc", false)]
        [InlineData("abc", "", false)]
        public void IsRotation_ShouldValidateRotation(string original, string rotated, bool expected)
        {
            var result = Strings.IsRotation(original, rotated);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ZAlgorithm_ShouldFindAllOccurrences()
        {
            var result = Strings.ZAlgorithm("ababcababcababc", "abc");
            Assert.Equal(new List<int> { 2, 7, 12 }, result);
        }

        [Fact]
        public void RabinKarp_ShouldReturnCorrectMatchPositions()
        {
            var result = Strings.RabinKarp("xyzxyzxyz", "xyz");
            Assert.Equal(new List<int> { 0, 3, 6 }, result);
        }

        [Theory]
        [InlineData("ABABDABACDABABCABAB", "ABABCABAB", new[] { 10 })]
        [InlineData("aaaaa", "aa", new[] { 0, 1, 2, 3 })]
        [InlineData("abcabcabc", "abc", new[] { 0, 3, 6 })]
        [InlineData("abcdef", "gh", new int[0])]
        public void KMPSearch_ShouldReturnExpectedIndices(string text, string pattern, int[] expected)
        {
            var result = Strings.KMPSearch(pattern, text);
            Assert.Equal(expected, result);
        }
    }
}
