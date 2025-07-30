namespace Csdsa.Domain.Common.ValueObjects;

public sealed class HashedPassword : IEquatable<HashedPassword>
{
    public string Value { get; }

    public HashedPassword(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Hashed password cannot be null or empty", nameof(value));
        }

        Value = value;
    }

    public bool Equals(HashedPassword? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is HashedPassword other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(HashedPassword hashedPassword) => hashedPassword.Value;

    public static explicit operator HashedPassword(string value) => new(value);
}
