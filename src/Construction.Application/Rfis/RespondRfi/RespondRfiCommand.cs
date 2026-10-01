using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.RespondRfi;

public sealed record RespondRfiCommand(
    Guid ProjectId,
    Guid RfiId,
    string Response) : ICommand<RfiResponse>;
