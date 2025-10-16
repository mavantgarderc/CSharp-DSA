using Csdsa.Domain.Common;

namespace Csdsa.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    public Email(string value)
    {
        if (string.IsNullOrEmpty(value) || !value.Contains("@"))
            throw new ArgumentException("Invalid email format.", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
