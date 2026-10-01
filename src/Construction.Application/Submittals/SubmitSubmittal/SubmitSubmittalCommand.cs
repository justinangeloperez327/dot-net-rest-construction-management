using Construction.Application.Common.Messaging;

namespace Construction.Application.Submittals.SubmitSubmittal;

public sealed record SubmitSubmittalCommand(
    Guid ProjectId,
    Guid SubmittalId,
    DateOnly? ReviewDueDate) : ICommand<SubmittalResponse>;
