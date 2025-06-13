using System.Text;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts (5 items):
    ///     - String reversal
    ///     - Palindrome detection
    ///     - Anagram verification
    ///     - Substring search
    ///     - Concatenation performance
    ///     - Z-Algorithm for pattern matching
    ///     - KMP (Knuth-Morris-Pratt) algorithm
    ///     - Rabin-Karp rolling hash technique
    /// Key Practices (5 items):
    ///     - Case/whitespace normalization
    ///     - Dictionary-based character counting
    ///     - Manual string scanning
    ///     - StringBuilder usage
    ///     - Rolling hash computation
    ///     - Prefix function construction
    ///     - Preparing for linear-time search
    ///     - Time complexity considerations
    /// </summary>
    public class Strings
    {
        public static string Reverse(string input)
        {
            StringBuilder builder = new(input.Length);
            for (int i = input.Length - 1; i >= 0; i--)
            {
                builder.Append(input[i]);
            }
            return builder.ToString();
        }
        // Checks if a string is a palindrome (ignore case & space)
        public static bool IsPalindrome(string input)
        {
            if (string.IsNullOrEmpty(input)) return true;

            var normalized = new string(input.Where(char.IsLetterOrDigit)
                    .Select(char.ToLowerInvariant)
                    .ToArray());

            int mid = normalized.Length / 2;
            for (int i = 0; i < mid; i++)
            {
                if (normalized[i] != normalized[normalized.Length - 1 - i])
                    return false;
            }

            return true;
        }

        // Verifies if two strings are anagrams
        public static bool AreAnagrams(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            var count = new Dictionary<char, int>();

            foreach (var ch in a)
            {
                if (count.TryGetValue(ch, out int value))
                    count[ch] = ++value;
                else
                    count[ch] = 1;
            }

            foreach (var ch in b)
            {
                if (!count.TryGetValue(ch, out int value)) return false;
                count[ch] = --value;
                if (value < 0) return false;
            }
            return true;
        }

        // Searches for the first occurance of a substring
        public static int IndexOfSubstring(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) return -1;
            if (target.Length > source.Length) return -1;

            for (int i = 0; i <= source.Length - target.Length; i++)
            {
                int j = 0;
                while (j < target.Length && source[i+j] == target[j]) j++;
                if (j == target.Length) return i;
            }

            return -1;
        }

        // Finds the first non-repeating character using a frequency map
        public static char? FirstUniqueChar(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var frequency = new Dictionary<char, int>();
            foreach (var ch in input)
                frequency[ch] = frequency.TryGetValue(ch, out int value) ? value + 1 : 1;

            foreach (var ch in input)
                if (frequency[ch] == 1)
                    return ch;

            return null;
        }


        // Checks if one string is a rotation of another using concatenation logic
        public static bool IsRotation(string original, string rotated)
        {
            if (original == null || rotated == null) return false;
            if (original.Length != rotated.Length) return false;

            var doubled = original + original;
            return doubled.Contains(rotated, StringComparison.Ordinal);
        }

        // Finds all pattern occurrences using Z-Algorithm
        public static List<int> ZAlgorithm(string text, string pattern)
        {
            string combined = pattern + "$" + text;
            int n = combined.Length;
            int[] z = new int[n];
            int l = 0, r = 0;

            for (int i = 1; i < n; i++)
            {
                if (i <= r)
                    z[i] = Math.Min(r-i + 1, z[i-l]);

                while (i + z[i] < n && combined[z[i]] == combined[i+z[i]])
                    z[i]++;

                if (i+z[i] - 1 > r)
                {
                    l = i;
                    r = i + z[i] - 1;
                }
            }

            var result = new List<int>();
            for (int i = pattern.Length + 1; i < n; i++)
            {
                if (z[i] == pattern.Length)
                    result.Add(i-pattern.Length - 1);
            }
            return result;
        }

        // Finds pattern occurrences using Rabin-Karp  algorithm
        public static List<int> RabinKarp(string text, string pattern, int prime = 101)
        {
            var result = new List<int>();
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text)) return result;

            int m = pattern.Length;
            int n = text.Length;
            long patternHash = 0, textHash = 0,  h = 1;
            int d = 256;

            for (int i = 0; i < m-1; i++)
                h = (h*d) % prime;

            for (int i = 0; i < m; i++)
            {
                patternHash = (d*patternHash + pattern[i]) % prime;
                textHash = (d*textHash + text[i]) % prime;
            }

            for (int i = 0; i <= n-m; i++)
            {
                if (patternHash == textHash)
                {
                    bool match = true;
                    for (int j = 0; j < m; j++)
                    {
                        if (text[i+j] != pattern[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) result.Add(i);
                }
                if (i < n-m)
                {
                    textHash = (d * (textHash - text[i]*h) + text[i+m]) % prime;
                    if (textHash < 0) textHash += prime;
                }
            }
            return result;
        }

        // Finds pattern occurrences using KMP algorithm
        private static int[] ComputeLPSArray(string pattern)
        {
            int M = pattern.Length;
            int[] lps = new int[M];
            int len = 0;
            int i = 1;
            lps[0] = 0;

            while (i < M)
            {
                if (pattern[i] == pattern[len])
                {
                    len++;
                    lps[i] = len;
                    i++;
                }
                else
                {
                    if (len != 0)
                        len = lps[len - 1];
                    else
                    {
                        lps[i] = 0;
                        i++;
                    }
                }
            }
            return lps;
        }
        public static List<int> KMPSearch(string pattern, string text)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(text))
                return [];

            int M = pattern.Length;
            int N = text.Length;
            var result = new List<int>();

            int[] lps = ComputeLPSArray(pattern);
            int i = 0, j = 0;

            while (i < N)
            {
                if (pattern[j] == text[i])
                {
                    j++;
                    i++;
                }

                if (j == M)
                {
                    result.Add(i - j);
                    j = lps[j - 1];
                }
                else if (i < N && pattern[j] != text[i])
                {
                    if (j != 0)
                        j = lps[j - 1];
                    else
                        i++;
                }
            }
            return result;
        }
    }
}
