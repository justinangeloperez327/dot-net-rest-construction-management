using Construction.Domain.Rfis;

namespace Construction.Api.Contracts.Rfis;

public sealed record ChangeRfiStatusRequest(
    RfiStatus Status,
    string? Reason);
