using Construction.Domain.Notifications;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("notifications");
        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(notification => notification.Subject)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(notification => notification.Message)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(notification => notification.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(notification => notification.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(notification => new
        {
            notification.RecipientUserId,
            notification.IsRead,
            notification.CreatedAtUtc
        });

        builder.HasIndex(notification => new
        {
            notification.ProjectId,
            notification.CreatedAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(notification => notification.RecipientUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(notification => notification.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
