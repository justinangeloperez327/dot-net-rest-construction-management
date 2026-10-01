using Construction.Domain.Equipment;
using Construction.Domain.Locations;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class EquipmentAssignmentConfiguration
    : IEntityTypeConfiguration<EquipmentAssignment>
{
    public void Configure(EntityTypeBuilder<EquipmentAssignment> builder)
    {
        builder.ToTable("equipment_assignments");
        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(assignment => new
        {
            assignment.EquipmentId,
            assignment.AssignedAtUtc
        });

        builder.HasIndex(assignment => assignment.UserId);
        builder.HasIndex(assignment => assignment.LocationId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(assignment => assignment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProjectLocation>()
            .WithMany()
            .HasForeignKey(assignment => assignment.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
