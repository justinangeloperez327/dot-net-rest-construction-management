using Construction.Domain.ProjectMembers;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ProjectMemberConfiguration
    : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("project_members");
        builder.HasKey(member => member.Id);

        builder.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(member => member.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(member => new
        {
            member.ProjectId,
            member.UserId
        }).IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(member => member.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(member => new
        {
            member.UserId,
            member.IsActive
        });
    }
}
