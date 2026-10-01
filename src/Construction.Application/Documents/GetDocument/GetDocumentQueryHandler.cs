using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.GetDocument;

public sealed class GetDocumentQueryHandler(
    IDocumentRepository documents,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetDocumentQuery, DocumentResponse>
{
    public async Task<Result<DocumentResponse>> HandleAsync(
        GetDocumentQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Documents.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<DocumentResponse>(accessError);
        }

        var document = await documents.GetAsync(
            query.DocumentId,
            cancellationToken);

        return document is null || document.ProjectId != query.ProjectId
            ? Result.Failure<DocumentResponse>(
                ApplicationError.NotFound(
                    "Documents.NotFound",
                    "The document was not found."))
            : Result.Success(DocumentResponse.FromDomain(document));
    }
}
