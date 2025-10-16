using Csdsa.Domain.Common;

namespace Csdsa.Domain.Aggregates.TrieAggregate;

public class Trie : IAggregateRoot
{
    public Entities.TrieNode Root { get; private set; }

    public Trie()
    {
        Root = new Entities.TrieNode();
    }

    public void Insert(string word)
    {
        // Insert logic
        // Raise NodeInsertedEvent
    }
}
