using SharedKernel;

namespace Warehouse.Domain.Models.Courier;

public class CourierStatus : ValueObject
{
    private CourierStatus(CourierStatusEnum courierStatusEnum)
    {
        CourierStatusEnum = courierStatusEnum;
    }

    public CourierStatusEnum CourierStatusEnum { get; }

    public static CourierStatus Create(CourierStatusEnum courierStatusEnum)
    {
        return new CourierStatus(courierStatusEnum);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CourierStatusEnum;
    }
}