using Construction.Domain.Issues;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class IssueHistoryEntryConfiguration
    : IEntityTypeConfiguration<IssueHistoryEntry>
{
    public void Configure(EntityTypeBuilder<IssueHistoryEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("issue_history");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Action)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(entry => entry.Note)
            .HasMaxLength(2000);

        builder.HasIndex(entry => new
        {
            entry.IssueId,
            entry.OccurredAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
