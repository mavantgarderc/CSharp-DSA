using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class Weight : ValueObject
{
    public double Value { get; private set; }

    public Weight(double value)
    {
        if (value < 0) throw new ArgumentException("Weight cannot be negative.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
