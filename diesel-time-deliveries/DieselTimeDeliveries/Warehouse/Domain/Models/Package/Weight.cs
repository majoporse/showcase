using ErrorOr;
using SharedKernel;

namespace Warehouse.Domain.Models.Package;

public class Weight : ValueObject
{
    private Weight(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; set; }

    public static ErrorOr<Weight> Create(decimal value)
    {
        if (value <= 0) return Error.Validation("Weight cannot be negative");

        return new Weight(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}