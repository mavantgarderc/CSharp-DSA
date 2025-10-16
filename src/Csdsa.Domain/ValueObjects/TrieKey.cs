using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class TrieKey : ValueObject
{
    public string Value { get; private set; }

    public TrieKey(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
