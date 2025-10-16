namespace Csdsa.Domain.Specifications;

public class ElementsByValueSpec<T> : Specification<T>
{
    private readonly T _value;

    public ElementsByValueSpec(T value)
    {
        _value = value;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return Equals(entity, _value);
    }
}
