using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Models.Vehicle;

namespace Warehouse.Infrastructure.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName(nameof(VehicleId))
            .ValueGeneratedNever()
            .HasConversion(
                goodsId => goodsId.Value,
                goods => VehicleId.Create(goods)
            );

        builder.OwnsOne(g => g.Vin);
        builder.OwnsOne(g => g.PackageWeightLimit);
    }
}