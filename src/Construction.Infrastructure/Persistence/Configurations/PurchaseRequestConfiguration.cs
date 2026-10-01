using Construction.Domain.Projects;
using Construction.Domain.PurchaseRequests;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class PurchaseRequestConfiguration
    : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("purchase_requests");
        builder.HasKey(request => request.Id);

        builder.Property(request => request.Number).HasMaxLength(100).IsRequired();
        builder.Property(request => request.NormalizedNumber).HasMaxLength(100).IsRequired();
        builder.Property(request => request.Title).HasMaxLength(300).IsRequired();
        builder.Property(request => request.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(request => request.ReviewRemarks).HasMaxLength(2000);

        builder.HasIndex(request => new
        {
            request.ProjectId,
            request.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(request => new
        {
            request.ProjectId,
            request.Status,
            request.CreatedAtUtc
        });

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(request => request.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(request => request.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(request => request.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(request => request.Items)
            .WithOne()
            .HasForeignKey(item => item.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(request => request.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
