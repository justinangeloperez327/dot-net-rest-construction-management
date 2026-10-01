using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.ArchiveDocument;

public sealed class ArchiveDocumentCommandHandler(
    IDocumentRepository documents,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ArchiveDocumentCommand>
{
    public async Task<Result> HandleAsync(
        ArchiveDocumentCommand command,
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

        var document = await documents.GetAsync(
            command.DocumentId,
            cancellationToken);

        if (document is null || document.ProjectId != command.ProjectId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Documents.NotFound",
                    "The document was not found."));
        }

        document.Archive();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
