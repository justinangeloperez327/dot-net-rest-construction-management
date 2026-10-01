using Construction.Domain.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ActivityDependencyConfiguration
    : IEntityTypeConfiguration<ActivityDependency>
{
    public void Configure(EntityTypeBuilder<ActivityDependency> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("activity_dependencies");
        builder.HasKey(dependency => dependency.Id);

        builder.Property(dependency => dependency.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(dependency => dependency.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(dependency => new
        {
            dependency.ActivityId,
            dependency.PredecessorActivityId
        }).IsUnique();

        builder.HasIndex(dependency => dependency.PredecessorActivityId);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(dependency => dependency.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(dependency => dependency.PredecessorActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
