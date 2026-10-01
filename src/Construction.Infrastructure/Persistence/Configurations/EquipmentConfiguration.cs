using Construction.Domain.Equipment;
using Construction.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class EquipmentConfiguration
    : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");
        builder.HasKey(equipment => equipment.Id);

        builder.Property(equipment => equipment.AssetCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(equipment => equipment.NormalizedAssetCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(equipment => equipment.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(equipment => equipment.Make).HasMaxLength(100);
        builder.Property(equipment => equipment.Model).HasMaxLength(100);
        builder.Property(equipment => equipment.SerialNumber).HasMaxLength(150);

        builder.Property(equipment => equipment.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(equipment => new
        {
            equipment.ProjectId,
            equipment.NormalizedAssetCode
        }).IsUnique();

        builder.HasIndex(equipment => new
        {
            equipment.ProjectId,
            equipment.Status
        });

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(equipment => equipment.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(equipment => equipment.Assignments)
            .WithOne()
            .HasForeignKey(assignment => assignment.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(equipment => equipment.MaintenanceRecords)
            .WithOne()
            .HasForeignKey(record => record.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(equipment => equipment.Assignments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(equipment => equipment.MaintenanceRecords)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
