using Construction.Domain.Submittals;

namespace Construction.Api.Contracts.Submittals;

public sealed record ReviewSubmittalRequest(
    SubmittalRevisionStatus Outcome,
    string? Remarks);
