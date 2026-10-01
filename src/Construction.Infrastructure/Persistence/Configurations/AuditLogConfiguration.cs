using Construction.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("audit_logs");
        builder.HasKey(audit => audit.Id);

        builder.Property(audit => audit.Category)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(audit => audit.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(audit => audit.EntityType)
            .HasMaxLength(200);

        builder.Property(audit => audit.EntityId)
            .HasMaxLength(200);

        builder.Property(audit => audit.ChangesJson)
            .HasMaxLength(20000);

        builder.Property(audit => audit.Description)
            .HasMaxLength(2000);

        builder.Property(audit => audit.OccurredAtUtc)
            .IsRequired();

        builder.HasIndex(audit => new
        {
            audit.ProjectId,
            audit.OccurredAtUtc
        });

        builder.HasIndex(audit => new
        {
            audit.Category,
            audit.OccurredAtUtc
        });

        builder.HasIndex(audit => new
        {
            audit.UserId,
            audit.OccurredAtUtc
        });

        builder.HasIndex(audit => new
        {
            audit.EntityType,
            audit.EntityId
        });
    }
}
