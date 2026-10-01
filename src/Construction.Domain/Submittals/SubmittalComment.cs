using Construction.Domain.Common;

namespace Construction.Domain.Submittals;

public sealed class SubmittalComment : AuditableEntity<Guid>
{
    private SubmittalComment()
        : base(Guid.Empty)
    {
    }

    internal SubmittalComment(
        Guid id,
        Guid submittalId,
        Guid authorUserId,
        string body)
        : base(id)
    {
        SubmittalId = submittalId;
        AuthorUserId = authorUserId;
        Body = ValidateBody(body);
    }

    public Guid SubmittalId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Body { get; private set; } = string.Empty;

    private static string ValidateBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new DomainException(
                "Submittal comment is required.");
        }

        if (body.Trim().Length > 4000)
        {
            throw new DomainException(
                "Submittal comment cannot exceed 4000 characters.");
        }

        return body.Trim();
    }
}
