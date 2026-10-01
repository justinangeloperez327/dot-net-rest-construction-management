using Construction.Application.Common.Messaging;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.ChangeSubmittalStatus;

public sealed record ChangeSubmittalStatusCommand(
    Guid ProjectId,
    Guid SubmittalId,
    SubmittalStatus Status,
    string? Reason) : ICommand<SubmittalResponse>;
