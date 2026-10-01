using Construction.Application.Common.Messaging;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.ReviewSubmittal;

public sealed record ReviewSubmittalCommand(
    Guid ProjectId,
    Guid SubmittalId,
    SubmittalRevisionStatus Outcome,
    string? Remarks) : ICommand<SubmittalResponse>;
