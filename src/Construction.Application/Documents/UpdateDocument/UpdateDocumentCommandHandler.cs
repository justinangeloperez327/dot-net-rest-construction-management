using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.UpdateDocument;

public sealed class UpdateDocumentCommandHandler(
    IDocumentRepository documents,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateDocumentCommand, DocumentResponse>
{
    public async Task<Result<DocumentResponse>> HandleAsync(
        UpdateDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Documents.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<DocumentResponse>(accessError);
        }

        var document = await documents.GetAsync(
            command.DocumentId,
            cancellationToken);

        if (document is null || document.ProjectId != command.ProjectId)
        {
            return Result.Failure<DocumentResponse>(
                ApplicationError.NotFound(
                    "Documents.NotFound",
                    "The document was not found."));
        }

        document.Update(
            command.Title,
            command.Category,
            command.Description);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(DocumentResponse.FromDomain(document));
    }
}
