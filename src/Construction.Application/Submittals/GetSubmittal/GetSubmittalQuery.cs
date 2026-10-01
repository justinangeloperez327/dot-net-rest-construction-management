using Construction.Application.Common.Messaging;

namespace Construction.Application.Submittals.GetSubmittal;

public sealed record GetSubmittalQuery(
    Guid ProjectId,
    Guid SubmittalId) : IQuery<SubmittalResponse>;
