using Construction.Domain.Attachments;

namespace Construction.Application.Abstractions.Data;

public interface IAttachmentRepository
{
    Task<Attachment?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Attachment>> GetByTargetAsync(
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default);

    void Add(Attachment attachment);

    void Remove(Attachment attachment);
}
