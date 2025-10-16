namespace Csdsa.Domain.Entities;

public class TrieNode
{
    public char? Character { get; private set; }
    public Dictionary<char, TrieNode> Children { get; private set; }
    public bool IsEndOfWord { get; set; }

    public TrieNode(char? character = null)
    {
        Character = character;
        Children = new Dictionary<char, TrieNode>();
    }
}
