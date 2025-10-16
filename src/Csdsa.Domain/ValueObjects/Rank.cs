using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class Rank : ValueObject
{
    public int Value { get; private set; }

    public Rank(int value)
    {
        if (value < 0) throw new ArgumentException("Rank cannot be negative.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
