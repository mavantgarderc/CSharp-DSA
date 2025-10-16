using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class Priority : ValueObject
{
    public int Value { get; private set; }

    public Priority(int value)
    {
        if (value < 0) throw new ArgumentException("Priority cannot be negative.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
