using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Models.Package;

namespace Warehouse.Infrastructure.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName(nameof(PackageId))
            .ValueGeneratedNever()
            .HasConversion(
                goodsId => goodsId.Value,
                goods => PackageId.Create(goods)
            );

        // builder.OwnsOne(g => g.Name);
        builder.OwnsOne(g => g.Weight);
        // builder.OwnsOne(g => g.Destination);

        // perhaps missong this? idk ¯\_(ツ)_/¯
        // builder.OwnsOne(g => g.Name);
        // builder.OwnsOne(g => g.Amount);
    }

    // public void Configure(EntityTypeBuilder<Weight> builder)
    // {
    //
    //     builder.HasNoKey();
    // }
}