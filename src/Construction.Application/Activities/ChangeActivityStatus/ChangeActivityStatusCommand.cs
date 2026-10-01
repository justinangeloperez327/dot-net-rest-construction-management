using Construction.Application.Common.Messaging;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.ChangeActivityStatus;

public sealed record ChangeActivityStatusCommand(
    Guid ProjectId,
    Guid ActivityId,
    ActivityStatus Status,
    DateOnly? EffectiveDate = null) : ICommand<ActivityResponse>;
