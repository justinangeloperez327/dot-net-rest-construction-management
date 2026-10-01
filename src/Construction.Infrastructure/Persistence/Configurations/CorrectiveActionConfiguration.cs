using Construction.Domain.Issues;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class CorrectiveActionConfiguration
    : IEntityTypeConfiguration<CorrectiveAction>
{
    public void Configure(EntityTypeBuilder<CorrectiveAction> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("corrective_actions");
        builder.HasKey(action => action.Id);

        builder.Property(action => action.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(action => action.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(action => action.CompletionNotes)
            .HasMaxLength(4000);

        builder.Property(action => action.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(action => new
        {
            action.IssueId,
            action.Status,
            action.DueDate
        });

        builder.HasIndex(action => action.ResponsibleUserId);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(action => action.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(action => action.CompletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
