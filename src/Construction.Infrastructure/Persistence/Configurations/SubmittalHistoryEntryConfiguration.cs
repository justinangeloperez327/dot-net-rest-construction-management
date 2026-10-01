using Construction.Domain.Submittals;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class SubmittalHistoryEntryConfiguration
    : IEntityTypeConfiguration<SubmittalHistoryEntry>
{
    public void Configure(EntityTypeBuilder<SubmittalHistoryEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("submittal_history");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Action)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(entry => entry.Note)
            .HasMaxLength(2000);

        builder.HasIndex(entry => new
        {
            entry.SubmittalId,
            entry.OccurredAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
