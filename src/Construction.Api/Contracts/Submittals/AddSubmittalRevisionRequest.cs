namespace Construction.Api.Contracts.Submittals;

public sealed record AddSubmittalRevisionRequest(
    string RevisionCode,
    string? Description);
