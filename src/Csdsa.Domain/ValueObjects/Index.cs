using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class Index : ValueObject
{
    public int Value { get; private set; }

    public Index(int value)
    {
        if (value < 0) throw new ArgumentException("Index cannot be negative.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
