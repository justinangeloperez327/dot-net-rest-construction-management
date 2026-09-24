using Construction.Domain.Locations;
using Construction.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ProjectLocationConfiguration
    : IEntityTypeConfiguration<ProjectLocation>
{
    public void Configure(EntityTypeBuilder<ProjectLocation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("project_locations");
        builder.HasKey(location => location.Id);

        builder.Property(location => location.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(location => location.NormalizedName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(location => location.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(location => location.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(location => location.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProjectLocation>()
            .WithMany()
            .HasForeignKey(location => location.ParentLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(location => new
        {
            location.ProjectId,
            location.ParentLocationId,
            location.NormalizedName
        }).IsUnique();

        builder.HasIndex(location => new
        {
            location.ProjectId,
            location.IsActive
        });
    }
}
