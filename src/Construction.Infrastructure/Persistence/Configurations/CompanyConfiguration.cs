using Construction.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("companies");
        builder.HasKey(company => company.Id);

        builder.Property(company => company.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.NormalizedName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(company => company.NormalizedName)
            .IsUnique();

        builder.Property(company => company.Type)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(company => company.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(company => company.CreatedAtUtc)
            .IsRequired();
    }
}
