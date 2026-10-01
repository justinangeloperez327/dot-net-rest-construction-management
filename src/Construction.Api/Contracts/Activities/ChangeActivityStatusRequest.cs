using Construction.Domain.Activities;

namespace Construction.Api.Contracts.Activities;

public sealed record ChangeActivityStatusRequest(
    ActivityStatus Status,
    DateOnly? EffectiveDate);
