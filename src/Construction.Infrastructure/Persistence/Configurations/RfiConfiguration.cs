using Construction.Domain.Projects;
using Construction.Domain.Rfis;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class RfiConfiguration : IEntityTypeConfiguration<Rfi>
{
    public void Configure(EntityTypeBuilder<Rfi> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("rfis");
        builder.HasKey(rfi => rfi.Id);

        builder.Property(rfi => rfi.Number)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rfi => rfi.NormalizedNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rfi => rfi.Subject)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(rfi => rfi.Question)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(rfi => rfi.Response)
            .HasMaxLength(10000);

        builder.Property(rfi => rfi.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(rfi => rfi.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(rfi => new
        {
            rfi.ProjectId,
            rfi.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(rfi => new
        {
            rfi.ProjectId,
            rfi.Status,
            rfi.DueDate
        });

        builder.HasIndex(rfi => rfi.ResponsibleUserId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(rfi => rfi.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rfi => rfi.RaisedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rfi => rfi.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rfi => rfi.RespondedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rfi => rfi.ClosedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(rfi => rfi.Comments)
            .WithOne()
            .HasForeignKey(comment => comment.RfiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(rfi => rfi.History)
            .WithOne()
            .HasForeignKey(entry => entry.RfiId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(rfi => rfi.Comments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(rfi => rfi.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
