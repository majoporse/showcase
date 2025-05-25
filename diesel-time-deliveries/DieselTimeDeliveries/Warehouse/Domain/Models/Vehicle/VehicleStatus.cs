using SharedKernel;

namespace Warehouse.Domain.Models.Vehicle;

public class VehicleStatus : ValueObject
{
    private VehicleStatus(VehicleStatusEnum vehicleStatusEnum)
    {
        VehicleStatusEnum = vehicleStatusEnum;
    }

    public VehicleStatusEnum VehicleStatusEnum { get; }

    public static VehicleStatus Create(VehicleStatusEnum vehicleStatusEnum)
    {
        return new VehicleStatus(vehicleStatusEnum);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return VehicleStatusEnum;
    }
}