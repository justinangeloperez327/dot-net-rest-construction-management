using Construction.Domain.Common;

namespace Construction.Domain.Rfis;

public sealed class RfiComment : AuditableEntity<Guid>
{
    private RfiComment()
        : base(Guid.Empty)
    {
    }

    internal RfiComment(
        Guid id,
        Guid rfiId,
        Guid authorUserId,
        string body)
        : base(id)
    {
        RfiId = rfiId;
        AuthorUserId = authorUserId;
        Body = ValidateBody(body);
    }

    public Guid RfiId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Body { get; private set; } = string.Empty;

    private static string ValidateBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new DomainException("RFI comment is required.");
        }

        if (body.Trim().Length > 4000)
        {
            throw new DomainException(
                "RFI comment cannot exceed 4000 characters.");
        }

        return body.Trim();
    }
}
