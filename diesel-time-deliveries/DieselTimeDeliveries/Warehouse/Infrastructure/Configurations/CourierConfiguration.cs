using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Warehouse.Domain.Models.Courier;

namespace Warehouse.Infrastructure.Configurations;

public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName(nameof(CourierId))
            .ValueGeneratedNever()
            .HasConversion(
                goodsId => goodsId.Value,
                goods => CourierId.Create(goods)
            );

        builder.OwnsOne(g => g.Email);
    }
}