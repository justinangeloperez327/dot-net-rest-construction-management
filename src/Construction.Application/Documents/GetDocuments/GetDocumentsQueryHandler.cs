using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.GetDocuments;

public sealed class GetDocumentsQueryHandler(
    IDocumentRepository documents,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetDocumentsQuery, PagedResult<DocumentSummaryResponse>>
{
    public async Task<Result<PagedResult<DocumentSummaryResponse>>> HandleAsync(
        GetDocumentsQuery query,
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
            return Result.Failure<PagedResult<DocumentSummaryResponse>>(
                accessError);
        }

        var page = await documents.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        var response = new PagedResult<DocumentSummaryResponse>(
            page.Items.Select(DocumentSummaryResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount);

        return Result.Success(response);
    }
}
