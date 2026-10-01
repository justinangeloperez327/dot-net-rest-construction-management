using Construction.Domain.PurchaseOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderItemConfiguration
    : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("purchase_order_items");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Description).HasMaxLength(500).IsRequired();
        builder.Property(item => item.OrderedQuantity).HasPrecision(18, 3);
        builder.Property(item => item.ReceivedQuantity).HasPrecision(18, 3);
        builder.Property(item => item.Unit).HasMaxLength(50).IsRequired();
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);

        builder.HasIndex(item => item.PurchaseOrderId);
    }
}
