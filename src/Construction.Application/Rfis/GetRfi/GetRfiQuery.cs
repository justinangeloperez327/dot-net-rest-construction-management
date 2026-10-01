using Construction.Application.Common.Messaging;

namespace Construction.Application.Rfis.GetRfi;

public sealed record GetRfiQuery(
    Guid ProjectId,
    Guid RfiId) : IQuery<RfiResponse>;
