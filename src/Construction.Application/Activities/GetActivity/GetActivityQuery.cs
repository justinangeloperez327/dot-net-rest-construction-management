using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.GetActivity;

public sealed record GetActivityQuery(
    Guid ProjectId,
    Guid ActivityId) : IQuery<ActivityResponse>;
