using ErrorOr;
using SharedKernel;
using Warehouse.Domain.Events.Vehicle;
using Warehouse.Domain.Models.Package;

namespace Warehouse.Domain.Models.Vehicle;

public class Vehicle : AggregateRoot<VehicleId>
{
    public string Make { get; private set; }
    public string Model { get; private set; }
    public Weight PackageWeightLimit { get; private set; }

    public Vin Vin { get; private set; }
    public VehicleStatusEnum Status { get; set; }

    public static ErrorOr<Vehicle> Create(string make, string model, decimal weightLimit, string vin)
    {
        if (string.IsNullOrWhiteSpace(make))
            return Error.Validation("invalid vehicle make");

        if (string.IsNullOrWhiteSpace(model))
            return Error.Validation("invalid vehicle model");


        var weightOrError = Weight.Create(weightLimit);
        if (weightOrError.IsError) return weightOrError.Errors;


        var vinOrError = Vin.Create(vin);
        if (vinOrError.IsError) return vinOrError.Errors;

        return new Vehicle
        {
            Id = VehicleId.CreateUnique(),
            Make = make,
            Model = model,
            PackageWeightLimit = weightOrError.Value,
            Vin = vinOrError.Value,
            Status = VehicleStatusEnum.Available
        };
    }
    
    public ErrorOr<Success> Update(string? make, string? model, decimal? weight, string? vin, string? status)
    {
        Make = make ?? Make;
        Model = model ?? Model;
        
        if (weight.HasValue)
        {
            var weightOrError = Weight.Create(weight.Value);
            if (weightOrError.IsError) return weightOrError.Errors;
            PackageWeightLimit = weightOrError.Value;
        }
        
        if (vin != null)
        {
            var vinOrError = Vin.Create(vin);
            if (vinOrError.IsError) return vinOrError.Errors;
            Vin = vinOrError.Value;
        }
        
        if (status != null)
        {
            if (!Enum.TryParse<VehicleStatusEnum>(status, true, out var newStatus))
                return Error.Validation("Invalid status");
            Status = newStatus;
        }
        
        return Result.Success;
    }

    public void VehicleAdded()
    {
        RaiseEvent(new VehicleCreatedDomainEvent{ VehicleId = Id.Value });
    }
    
    public void VehicleUpdated()
    {
        RaiseEvent(new VehicleUpdatedDomainEvent{ VehicleId = Id.Value });
    }
    
    public void VehicleDeleted()
    {
        RaiseEvent(new VehicleDeletedDomainEvent{ VehicleId = Id.Value });
    }
}