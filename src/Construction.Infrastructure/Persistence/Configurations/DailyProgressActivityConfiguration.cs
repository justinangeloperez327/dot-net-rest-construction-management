using Construction.Domain.Activities;
using Construction.Domain.DailyProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class DailyProgressActivityConfiguration
    : IEntityTypeConfiguration<DailyProgressActivity>
{
    public void Configure(EntityTypeBuilder<DailyProgressActivity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("daily_progress_activities");
        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.WorkDescription)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(activity => activity.ReportedProgressPercentage)
            .HasPrecision(5, 2);

        builder.Property(activity => activity.QuantityCompleted)
            .HasPrecision(18, 3);

        builder.Property(activity => activity.Unit)
            .HasMaxLength(50);

        builder.Property(activity => activity.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(activity => new
        {
            activity.ReportId,
            activity.ActivityId
        }).IsUnique();

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(activity => activity.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
