using Construction.Domain.Documents;

namespace Construction.Application.Documents;

public sealed record DocumentRevisionResponse(
    Guid Id,
    int VersionNumber,
    string RevisionCode,
    string FileName,
    string ContentType,
    long Length,
    Guid UploadedByUserId,
    DateTimeOffset UploadedAtUtc,
    string? Notes,
    bool IsCurrent)
{
    public static DocumentRevisionResponse FromDomain(DocumentRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        return new DocumentRevisionResponse(
            revision.Id,
            revision.VersionNumber,
            revision.RevisionCode,
            revision.FileName,
            revision.ContentType,
            revision.Length,
            revision.UploadedByUserId,
            revision.UploadedAtUtc,
            revision.Notes,
            revision.IsCurrent);
    }
}
