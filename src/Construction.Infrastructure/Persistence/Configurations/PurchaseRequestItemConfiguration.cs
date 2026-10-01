using Construction.Domain.PurchaseRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class PurchaseRequestItemConfiguration
    : IEntityTypeConfiguration<PurchaseRequestItem>
{
    public void Configure(EntityTypeBuilder<PurchaseRequestItem> builder)
    {
        builder.ToTable("purchase_request_items");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Description).HasMaxLength(500).IsRequired();
        builder.Property(item => item.Quantity).HasPrecision(18, 3);
        builder.Property(item => item.Unit).HasMaxLength(50).IsRequired();
        builder.Property(item => item.EstimatedUnitCost).HasPrecision(18, 2);

        builder.HasIndex(item => item.PurchaseRequestId);
    }
}
