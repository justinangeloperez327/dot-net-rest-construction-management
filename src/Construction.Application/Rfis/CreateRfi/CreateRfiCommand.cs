using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.CreateRfi;

public sealed record CreateRfiCommand(
    Guid ProjectId,
    string Number,
    string Subject,
    string Question,
    DateOnly? DueDate,
    Guid? ResponsibleUserId) : ICommand<RfiResponse>;
