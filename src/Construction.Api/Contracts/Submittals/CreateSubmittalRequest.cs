using Construction.Domain.Submittals;

namespace Construction.Api.Contracts.Submittals;

public sealed record CreateSubmittalRequest(
    string Number,
    string Title,
    SubmittalType Type,
    Guid? ResponsibleUserId);
