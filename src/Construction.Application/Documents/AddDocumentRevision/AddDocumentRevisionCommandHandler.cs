using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.AddDocumentRevision;

public sealed class AddDocumentRevisionCommandHandler(
    IDocumentRepository documents,
    IFileStorage fileStorage,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<AddDocumentRevisionCommand, DocumentRevisionResponse>
{
    public async Task<Result<DocumentRevisionResponse>> HandleAsync(
        AddDocumentRevisionCommand command,
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
            return Result.Failure<DocumentRevisionResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<DocumentRevisionResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var document = await documents.GetAsync(
            command.DocumentId,
            cancellationToken);

        if (document is null || document.ProjectId != command.ProjectId)
        {
            return Result.Failure<DocumentRevisionResponse>(
                ApplicationError.NotFound(
                    "Documents.NotFound",
                    "The document was not found."));
        }

        StoredFile storedFile = await fileStorage.SaveAsync(
            command.File,
            cancellationToken);

        try
        {
            var revision = document.AddRevision(
                command.RevisionCode,
                storedFile.FileName,
                storedFile.ContentType,
                storedFile.Length,
                storedFile.StorageKey,
                userId,
                timeProvider.GetUtcNow(),
                command.Notes);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(
                DocumentRevisionResponse.FromDomain(revision));
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
