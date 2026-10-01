using Construction.Domain.Submittals;

namespace Construction.Api.Contracts.Submittals;

public sealed record ChangeSubmittalStatusRequest(
    SubmittalStatus Status,
    string? Reason);
