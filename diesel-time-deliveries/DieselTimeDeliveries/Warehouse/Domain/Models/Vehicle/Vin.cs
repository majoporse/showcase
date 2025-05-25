using System.Text.RegularExpressions;
using ErrorOr;
using SharedKernel;

namespace Warehouse.Domain.Models.Vehicle;

public class Vin : ValueObject
{
    private Vin(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public static ErrorOr<Vin> Create(string value)
    {
        if (!new Regex(@"^[A-Z]{2}[0-9]{3}[A-Z]{2}$").IsMatch(value))
            return Error.Validation("Slovak Vin number is not valid");
        return new Vin(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}