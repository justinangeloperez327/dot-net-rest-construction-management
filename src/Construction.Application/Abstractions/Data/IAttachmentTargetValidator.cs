using Construction.Domain.Attachments;

namespace Construction.Application.Abstractions.Data;

public interface IAttachmentTargetValidator
{
    Task<bool> ExistsAsync(
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default);
}
