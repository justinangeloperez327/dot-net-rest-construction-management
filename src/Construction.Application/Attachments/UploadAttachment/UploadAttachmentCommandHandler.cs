using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Attachments;

namespace Construction.Application.Attachments.UploadAttachment;

public sealed class UploadAttachmentCommandHandler(
    IAttachmentRepository attachments,
    IAttachmentTargetValidator targetValidator,
    IFileStorage fileStorage,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<UploadAttachmentCommand, AttachmentResponse>
{
    public async Task<Result<AttachmentResponse>> HandleAsync(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.File);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Documents.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<AttachmentResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<AttachmentResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        if (!await targetValidator.ExistsAsync(
            command.ProjectId,
            command.TargetType,
            command.TargetId,
            cancellationToken))
        {
            return Result.Failure<AttachmentResponse>(
                ApplicationError.Validation(
                    "Attachments.InvalidTarget",
                    "The attachment target does not exist in this project."));
        }

        StoredFile storedFile = await fileStorage.SaveAsync(
            command.File,
            cancellationToken);

        try
        {
            Attachment attachment = Attachment.Create(
                command.ProjectId,
                command.TargetType,
                command.TargetId,
                storedFile.FileName,
                storedFile.ContentType,
                storedFile.Length,
                storedFile.StorageKey,
                command.Description,
                userId,
                timeProvider.GetUtcNow());

            attachments.Add(attachment);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(
                AttachmentResponse.FromDomain(attachment));
        }
        catch
        {
            await fileStorage.DeleteAsync(
                storedFile.StorageKey,
                CancellationToken.None);

            throw;
        }
    }
}
