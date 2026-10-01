using Construction.Domain.Activities;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ActivityAssignmentConfiguration
    : IEntityTypeConfiguration<ActivityAssignment>
{
    public void Configure(EntityTypeBuilder<ActivityAssignment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("activity_assignments");
        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(assignment => assignment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(assignment => new
        {
            assignment.ActivityId,
            assignment.UserId
        }).IsUnique();

        builder.HasIndex(assignment => assignment.UserId);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(assignment => assignment.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(assignment => assignment.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
