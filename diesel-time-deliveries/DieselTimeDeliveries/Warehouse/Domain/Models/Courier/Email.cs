using System.ComponentModel.DataAnnotations;
using ErrorOr;
using SharedKernel;

namespace Warehouse.Domain.Models.Courier;

public class Email : ValueObject
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public static ErrorOr<Email> Create(string value)
    {
        if (!new EmailAddressAttribute().IsValid(value)) return Error.Validation("Email is not valid");
        return new Email(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}