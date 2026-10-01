using Construction.Domain.DailyProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class DailyProgressEquipmentConfiguration
    : IEntityTypeConfiguration<DailyProgressEquipment>
{
    public void Configure(EntityTypeBuilder<DailyProgressEquipment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("daily_progress_equipment");
        builder.HasKey(equipment => equipment.Id);

        builder.Property(equipment => equipment.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(equipment => equipment.WorkingHours)
            .HasPrecision(10, 2);

        builder.Property(equipment => equipment.IdleHours)
            .HasPrecision(10, 2);

        builder.Property(equipment => equipment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(equipment => equipment.ReportId);
    }
}
