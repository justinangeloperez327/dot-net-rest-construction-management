using Construction.Domain.Documents;

namespace Construction.Application.Documents;

public sealed record DocumentSummaryResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    DocumentCategory Category,
    DocumentStatus Status,
    int CurrentVersionNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static DocumentSummaryResponse FromDomain(ProjectDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new DocumentSummaryResponse(
            document.Id,
            document.ProjectId,
            document.Number,
            document.Title,
            document.Category,
            document.Status,
            document.CurrentVersionNumber,
            document.CreatedAtUtc,
            document.LastModifiedAtUtc);
    }
}
