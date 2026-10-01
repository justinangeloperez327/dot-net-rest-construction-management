using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.UpdateRfi;

public sealed record UpdateRfiCommand(
    Guid ProjectId,
    Guid RfiId,
    string Subject,
    string Question,
    DateOnly? DueDate,
    Guid? ResponsibleUserId) : ICommand<RfiResponse>;
