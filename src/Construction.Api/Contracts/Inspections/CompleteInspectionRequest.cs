namespace Construction.Api.Contracts.Inspections;

public sealed record CompleteInspectionRequest(
    bool Passed,
    string ResultNotes);
