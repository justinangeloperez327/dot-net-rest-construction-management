using Construction.Domain.Companies;
using Construction.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration
    : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(supplier => supplier.NormalizedCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(supplier => supplier.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(supplier => supplier.TaxRegistrationNumber).HasMaxLength(100);
        builder.Property(supplier => supplier.ContactName).HasMaxLength(200);
        builder.Property(supplier => supplier.ContactEmail).HasMaxLength(320);
        builder.Property(supplier => supplier.ContactPhone).HasMaxLength(50);
        builder.Property(supplier => supplier.PaymentTerms).HasMaxLength(500);

        builder.HasIndex(supplier => supplier.NormalizedCode).IsUnique();
        builder.HasIndex(supplier => supplier.CompanyId).IsUnique();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(supplier => supplier.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
