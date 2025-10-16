using Csdsa.Domain.Common;

namespace Csdsa.Domain.Specifications;

public class StructureByTypeSpec : Specification<IStructure>
{
    private readonly Enums.StructureType _type;

    public StructureByTypeSpec(Enums.StructureType type)
    {
        _type = type;
    }

    public override bool IsSatisfiedBy(IStructure entity)
    {
        // Type check logic
        return true;
    }
}
