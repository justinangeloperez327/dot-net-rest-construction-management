using Construction.Domain.Equipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class EquipmentMaintenanceRecordConfiguration
    : IEntityTypeConfiguration<EquipmentMaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<EquipmentMaintenanceRecord> builder)
    {
        builder.ToTable("equipment_maintenance");
        builder.HasKey(record => record.Id);

        builder.Property(record => record.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(record => record.ServiceProvider)
            .HasMaxLength(200);

        builder.Property(record => record.Cost)
            .HasPrecision(18, 2);

        builder.Property(record => record.CompletionNotes)
            .HasMaxLength(4000);

        builder.HasIndex(record => new
        {
            record.EquipmentId,
            record.ScheduledDate
        });
    }
}
