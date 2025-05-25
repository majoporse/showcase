using SharedKernel;

namespace Warehouse.Domain.Models.Courier;

public class CourierId : ValueObject
{
    private CourierId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static CourierId CreateUnique()
    {
        return new CourierId(Guid.NewGuid());
    }

    public static CourierId Create(Guid value)
    {
        return new CourierId(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}