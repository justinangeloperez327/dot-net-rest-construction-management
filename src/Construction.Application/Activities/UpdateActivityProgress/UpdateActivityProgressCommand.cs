using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.UpdateActivityProgress;

public sealed record UpdateActivityProgressCommand(
    Guid ProjectId,
    Guid ActivityId,
    decimal ProgressPercentage) : ICommand<ActivityResponse>;
