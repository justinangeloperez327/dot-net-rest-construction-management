using Construction.Domain.Projects;
using Construction.Domain.PurchaseOrders;
using Construction.Domain.PurchaseRequests;
using Construction.Domain.Suppliers;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Number).HasMaxLength(100).IsRequired();
        builder.Property(order => order.NormalizedNumber).HasMaxLength(100).IsRequired();
        builder.Property(order => order.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(order => new
        {
            order.ProjectId,
            order.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(order => new
        {
            order.ProjectId,
            order.Status,
            order.ExpectedDeliveryDate
        });

        builder.HasIndex(order => order.SupplierId);
        builder.HasIndex(order => order.PurchaseRequestId).IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(order => order.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(order => order.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PurchaseRequest>()
            .WithMany()
            .HasForeignKey(order => order.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(order => order.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(order => order.IssuedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
