using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Activities.GetActivities;

public sealed record GetActivitiesQuery(
    Guid ProjectId,
    PageRequest Page) : IQuery<PagedResult<ActivityResponse>>;
