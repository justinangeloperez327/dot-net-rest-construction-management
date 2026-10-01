using Construction.Domain.Activities;
using Construction.Domain.Inspections;
using Construction.Domain.Locations;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class InspectionConfiguration
    : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("inspections");
        builder.HasKey(inspection => inspection.Id);

        builder.Property(inspection => inspection.Number)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(inspection => inspection.NormalizedNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(inspection => inspection.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(inspection => inspection.Description)
            .HasMaxLength(5000);

        builder.Property(inspection => inspection.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(inspection => inspection.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(inspection => inspection.ResultNotes)
            .HasMaxLength(5000);

        builder.Property(inspection => inspection.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(inspection => new
        {
            inspection.ProjectId,
            inspection.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(inspection => new
        {
            inspection.ProjectId,
            inspection.Status,
            inspection.RequestedForDate
        });

        builder.HasIndex(inspection => inspection.InspectorUserId);
        builder.HasIndex(inspection => inspection.LocationId);
        builder.HasIndex(inspection => inspection.ActivityId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(inspection => inspection.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProjectLocation>()
            .WithMany()
            .HasForeignKey(inspection => inspection.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(inspection => inspection.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(inspection => inspection.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(inspection => inspection.InspectorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(inspection => inspection.InspectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(inspection => inspection.History)
            .WithOne()
            .HasForeignKey(entry => entry.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(inspection => inspection.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
