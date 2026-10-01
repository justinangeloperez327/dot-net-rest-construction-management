using Construction.Application.Common.Messaging;

namespace Construction.Application.Submittals.AddSubmittalRevision;

public sealed record AddSubmittalRevisionCommand(
    Guid ProjectId,
    Guid SubmittalId,
    string RevisionCode,
    string? Description) : ICommand<SubmittalRevisionResponse>;
