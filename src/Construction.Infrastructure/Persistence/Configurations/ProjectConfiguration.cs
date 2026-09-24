using Construction.Domain.Companies;
using Construction.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("projects");
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Number)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(project => project.NormalizedNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(project => project.NormalizedNumber)
            .IsUnique();

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(2000);

        builder.Property(project => project.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(project => project.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(project => project.ClientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(project => project.MainContractorCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(project => project.ConsultantCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(project => project.Status);
        builder.HasIndex(project => project.ClientCompanyId);
        builder.HasIndex(project => project.MainContractorCompanyId);
        builder.HasIndex(project => project.ConsultantCompanyId);
    }
}
