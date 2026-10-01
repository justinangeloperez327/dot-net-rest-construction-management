using Construction.Domain.DailyProgress;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class DailyProgressReportConfiguration
    : IEntityTypeConfiguration<DailyProgressReport>
{
    public void Configure(EntityTypeBuilder<DailyProgressReport> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("daily_progress_reports");
        builder.HasKey(report => report.Id);

        builder.Property(report => report.Weather)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(report => report.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(report => report.TemperatureCelsius)
            .HasPrecision(5, 2);

        builder.Property(report => report.WorkSummary)
            .HasMaxLength(5000);

        builder.Property(report => report.Remarks)
            .HasMaxLength(5000);

        builder.Property(report => report.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(report => report.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(report => new
        {
            report.ProjectId,
            report.ReportDate
        }).IsUnique();

        builder.HasIndex(report => new
        {
            report.ProjectId,
            report.Status,
            report.ReportDate
        });

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(report => report.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(report => report.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(report => report.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(report => report.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(report => report.Activities)
            .WithOne()
            .HasForeignKey(activity => activity.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(report => report.Manpower)
            .WithOne()
            .HasForeignKey(manpower => manpower.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(report => report.Equipment)
            .WithOne()
            .HasForeignKey(equipment => equipment.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(report => report.Activities)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(report => report.Manpower)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(report => report.Equipment)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
