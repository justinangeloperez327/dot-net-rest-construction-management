using Construction.Application.Common.Messaging;
using Construction.Domain.Submittals;

namespace Construction.Application.Submittals.CreateSubmittal;

public sealed record CreateSubmittalCommand(
    Guid ProjectId,
    string Number,
    string Title,
    SubmittalType Type,
    Guid? ResponsibleUserId) : ICommand<SubmittalResponse>;
