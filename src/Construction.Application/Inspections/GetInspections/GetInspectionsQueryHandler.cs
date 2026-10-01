using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Inspections.GetInspections;

public sealed class GetInspectionsQueryHandler(
    IInspectionRepository inspections,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetInspectionsQuery, PagedResult<InspectionSummaryResponse>>
{
    public async Task<Result<PagedResult<InspectionSummaryResponse>>> HandleAsync(
        GetInspectionsQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Inspections.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<InspectionSummaryResponse>>(
                accessError);
        }

        var page = await inspections.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        return Result.Success(
            new PagedResult<InspectionSummaryResponse>(
                page.Items.Select(
                    InspectionSummaryResponse.FromDomain).ToArray(),
                Math.Max(1, query.Page.PageNumber),
                Math.Clamp(
                    query.Page.PageSize,
                    1,
                    PageRequest.MaximumPageSize),
                page.TotalCount));
    }
}
