using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Attachments.DeleteAttachment;

public sealed class DeleteAttachmentCommandHandler(
    IAttachmentRepository attachments,
    IFileStorage fileStorage,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<DeleteAttachmentCommand>
{
    public async Task<Result> HandleAsync(
        DeleteAttachmentCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Documents.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var attachment = await attachments.GetAsync(
            command.AttachmentId,
            cancellationToken);

        if (attachment is null || attachment.ProjectId != command.ProjectId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Attachments.NotFound",
                    "The attachment was not found."));
        }

        string storageKey = attachment.StorageKey;

        attachments.Remove(attachment);
        await dbContext.SaveChangesAsync(cancellationToken);

        await fileStorage.DeleteAsync(storageKey, cancellationToken);

        return Result.Success();
    }
}
