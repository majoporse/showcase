using SharedKernel;

namespace Warehouse.Domain.Models.Package;

public class PackageStatus : ValueObject
{
    private PackageStatus(PackageStatusEnum packageStatusEnum)
    {
        PackageStatusEnum = packageStatusEnum;
    }

    public PackageStatusEnum PackageStatusEnum { get; }

    public static PackageStatus Create(PackageStatusEnum packageStatusEnum)
    {
        return new PackageStatus(packageStatusEnum);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PackageStatusEnum;
    }
}