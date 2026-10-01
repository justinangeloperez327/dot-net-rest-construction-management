using Construction.Domain.Submittals;

namespace Construction.Api.Contracts.Submittals;

public sealed record UpdateSubmittalRequest(
    string Title,
    SubmittalType Type,
    Guid? ResponsibleUserId);
