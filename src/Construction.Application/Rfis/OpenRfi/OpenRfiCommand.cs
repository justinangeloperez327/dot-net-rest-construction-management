using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.OpenRfi;

public sealed record OpenRfiCommand(
    Guid ProjectId,
    Guid RfiId) : ICommand<RfiResponse>;
