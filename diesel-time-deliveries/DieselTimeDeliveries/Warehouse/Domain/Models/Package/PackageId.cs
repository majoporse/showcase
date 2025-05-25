using SharedKernel;

namespace Warehouse.Domain.Models.Package;

public class PackageId : ValueObject
{
    private PackageId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PackageId CreateUnique()
    {
        return new PackageId(Guid.NewGuid());
    }

    public static PackageId Create(Guid value)
    {
        return new PackageId(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}