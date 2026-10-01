using Construction.Domain.Activities;

namespace Construction.Api.Contracts.Activities;

public sealed record AddActivityAssignmentRequest(
    Guid UserId,
    ActivityAssignmentRole Role);
