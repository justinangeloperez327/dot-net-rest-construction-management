using Construction.Application.Common.Messaging;
using Construction.Domain.Rfis;

namespace Construction.Application.Rfis.ChangeRfiStatus;

public sealed record ChangeRfiStatusCommand(
    Guid ProjectId,
    Guid RfiId,
    RfiStatus Status,
    string? Reason) : ICommand<RfiResponse>;
