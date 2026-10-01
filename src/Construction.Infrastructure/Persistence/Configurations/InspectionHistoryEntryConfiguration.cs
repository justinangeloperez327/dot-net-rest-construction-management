using Construction.Domain.Inspections;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class InspectionHistoryEntryConfiguration
    : IEntityTypeConfiguration<InspectionHistoryEntry>
{
    public void Configure(EntityTypeBuilder<InspectionHistoryEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("inspection_history");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Action)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(entry => entry.Note)
            .HasMaxLength(2000);

        builder.HasIndex(entry => new
        {
            entry.InspectionId,
            entry.OccurredAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
