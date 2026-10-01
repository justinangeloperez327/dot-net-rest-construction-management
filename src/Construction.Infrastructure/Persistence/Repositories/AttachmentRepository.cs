using Construction.Application.Abstractions.Data;
using Construction.Domain.Attachments;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class AttachmentRepository(ApplicationDbContext dbContext)
    : IAttachmentRepository
{
    public Task<Attachment?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Attachments.SingleOrDefaultAsync(
            attachment => attachment.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<Attachment>> GetByTargetAsync(
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Attachments
            .AsNoTracking()
            .Where(attachment =>
                attachment.ProjectId == projectId
                && attachment.TargetType == targetType
                && attachment.TargetId == targetId)
            .OrderByDescending(attachment => attachment.UploadedAtUtc)
            .ToArrayAsync(cancellationToken);

    public void Add(Attachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        dbContext.Attachments.Add(attachment);
    }

    public void Remove(Attachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        dbContext.Attachments.Remove(attachment);
    }
}
