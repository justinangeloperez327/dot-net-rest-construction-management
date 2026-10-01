using Construction.Domain.Documents;

namespace Construction.Application.Documents;

public sealed record DocumentResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    DocumentCategory Category,
    string? Description,
    DocumentStatus Status,
    int CurrentVersionNumber,
    Guid CreatedByUserId,
    IReadOnlyCollection<DocumentRevisionResponse> Revisions,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static DocumentResponse FromDomain(ProjectDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new DocumentResponse(
            document.Id,
            document.ProjectId,
            document.Number,
            document.Title,
            document.Category,
            document.Description,
            document.Status,
            document.CurrentVersionNumber,
            document.CreatedByUserId,
            document.Revisions
                .OrderByDescending(revision => revision.VersionNumber)
                .Select(DocumentRevisionResponse.FromDomain)
                .ToArray(),
            document.CreatedAtUtc,
            document.LastModifiedAtUtc);
    }
}
