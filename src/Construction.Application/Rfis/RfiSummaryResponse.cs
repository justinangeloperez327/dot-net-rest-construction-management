using Construction.Domain.Rfis;

namespace Construction.Application.Rfis;

public sealed record RfiSummaryResponse(
    Guid Id,
    string Number,
    string Subject,
    RfiStatus Status,
    DateOnly? DueDate,
    Guid? ResponsibleUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static RfiSummaryResponse FromDomain(Rfi rfi) =>
        new(
            rfi.Id,
            rfi.Number,
            rfi.Subject,
            rfi.Status,
            rfi.DueDate,
            rfi.ResponsibleUserId,
            rfi.CreatedAtUtc,
            rfi.LastModifiedAtUtc);
}
