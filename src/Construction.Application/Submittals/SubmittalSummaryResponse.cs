using Construction.Domain.Submittals;

namespace Construction.Application.Submittals;

public sealed record SubmittalSummaryResponse(
    Guid Id,
    string Number,
    string Title,
    SubmittalType Type,
    SubmittalStatus Status,
    int CurrentVersionNumber,
    Guid? ResponsibleUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static SubmittalSummaryResponse FromDomain(Submittal submittal) =>
        new(
            submittal.Id,
            submittal.Number,
            submittal.Title,
            submittal.Type,
            submittal.Status,
            submittal.CurrentVersionNumber,
            submittal.ResponsibleUserId,
            submittal.CreatedAtUtc,
            submittal.LastModifiedAtUtc);
}
