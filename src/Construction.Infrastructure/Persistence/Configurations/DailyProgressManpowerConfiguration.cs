using Construction.Domain.Companies;
using Construction.Domain.DailyProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class DailyProgressManpowerConfiguration
    : IEntityTypeConfiguration<DailyProgressManpower>
{
    public void Configure(EntityTypeBuilder<DailyProgressManpower> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("daily_progress_manpower");
        builder.HasKey(manpower => manpower.Id);

        builder.Property(manpower => manpower.Trade)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(manpower => manpower.TotalHours)
            .HasPrecision(10, 2);

        builder.Property(manpower => manpower.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(manpower => manpower.ReportId);
        builder.HasIndex(manpower => manpower.CompanyId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(manpower => manpower.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
