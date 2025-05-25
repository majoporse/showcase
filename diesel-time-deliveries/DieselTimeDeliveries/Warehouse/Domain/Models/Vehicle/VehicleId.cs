using SharedKernel;

namespace Warehouse.Domain.Models.Vehicle;

public class VehicleId : ValueObject
{
    private VehicleId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static VehicleId CreateUnique()
    {
        return new VehicleId(Guid.NewGuid());
    }

    public static VehicleId Create(Guid value)
    {
        return new VehicleId(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}