using ErrorOr;
using SharedKernel;
using Warehouse.Domain.Events.Courier;

namespace Warehouse.Domain.Models.Courier;

public class Courier : AggregateRoot<CourierId>
{
    public string Name { get; private set; }

    public Email Email { get; private set; }

    public CourierStatusEnum Status { get; set; }

    public static ErrorOr<Courier> Create(string name, string email)
    {
        var emailOrError = Email.Create(email);
        if (emailOrError.IsError) return emailOrError.Errors;

        return new Courier
        {
            Id = CourierId.CreateUnique(),
            Name = name,
            Email = emailOrError.Value,
            Status = CourierStatusEnum.NotWorking
        };
    }
    
    public ErrorOr<Success> Update(string? name, string? email, string? status)
    {
        Name = name ?? Name;
        
        if (email != null)
        {
            var emailOrError = Email.Create(email);
            if (emailOrError.IsError) return emailOrError.Errors;
            Email = emailOrError.Value;
        }
        
        if (status != null)
        {
            if (!Enum.TryParse<CourierStatusEnum>(status, true, out var newStatus))
                return Error.Validation("Invalid status");
            Status = newStatus;
        }
        
        return Result.Success;
    }

    public void CourierAdded()
    {
        RaiseEvent(new CourierCreatedDomainEvent{ CourierId = Id.Value });
    }
    
    public void CourierUpdated()
    {
        RaiseEvent(new CourierUpdatedDomainEvent{ CourierId = Id.Value });
    }
    
    public void CourierDeleted()
    {
        RaiseEvent(new CourierDeletedDomainEvent{ CourierId = Id.Value });
    }
}