using Construction.Domain.Rfis;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class RfiHistoryEntryConfiguration
    : IEntityTypeConfiguration<RfiHistoryEntry>
{
    public void Configure(EntityTypeBuilder<RfiHistoryEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("rfi_history");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Action)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(entry => entry.Note)
            .HasMaxLength(2000);

        builder.HasIndex(entry => new
        {
            entry.RfiId,
            entry.OccurredAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
