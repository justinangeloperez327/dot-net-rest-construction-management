using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.GetActivities;

public sealed class GetActivitiesQueryHandler(
    IActivityRepository activities,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetActivitiesQuery, PagedResult<ActivityResponse>>
{
    public async Task<Result<PagedResult<ActivityResponse>>> HandleAsync(
        GetActivitiesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Activities.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<ActivityResponse>>(accessError);
        }

        var page = await activities.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        var response = new PagedResult<ActivityResponse>(
            page.Items.Select(ActivityResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount);

        return Result.Success(response);
    }
}
