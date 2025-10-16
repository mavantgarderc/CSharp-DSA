using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class KeyValuePair<TKey, TValue> : ValueObject
{
    public TKey Key { get; private set; }
    public TValue Value { get; private set; }

    public KeyValuePair(TKey key, TValue value)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key!;
        yield return Value!;
    }
}
