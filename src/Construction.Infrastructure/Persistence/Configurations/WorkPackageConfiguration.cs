using Construction.Domain.Projects;
using Construction.Domain.WorkPackages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class WorkPackageConfiguration
    : IEntityTypeConfiguration<WorkPackage>
{
    public void Configure(EntityTypeBuilder<WorkPackage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("work_packages");
        builder.HasKey(workPackage => workPackage.Id);

        builder.Property(workPackage => workPackage.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(workPackage => workPackage.NormalizedCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(workPackage => workPackage.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(workPackage => workPackage.Description)
            .HasMaxLength(2000);

        builder.Property(workPackage => workPackage.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(workPackage => workPackage.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(workPackage => new
        {
            workPackage.ProjectId,
            workPackage.NormalizedCode
        }).IsUnique();

        builder.HasIndex(workPackage => new
        {
            workPackage.ProjectId,
            workPackage.Status
        });

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(workPackage => workPackage.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<WorkPackage>()
            .WithMany()
            .HasForeignKey(workPackage => workPackage.ParentWorkPackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
