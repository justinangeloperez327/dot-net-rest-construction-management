using Construction.Application.Common.Messaging;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.UpdateSubmittal;

public sealed record UpdateSubmittalCommand(
    Guid ProjectId,
    Guid SubmittalId,
    string Title,
    SubmittalType Type,
    Guid? ResponsibleUserId) : ICommand<SubmittalResponse>;
