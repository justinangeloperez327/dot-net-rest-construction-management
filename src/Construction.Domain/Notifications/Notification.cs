using Construction.Domain.Common;

namespace Construction.Domain.Notifications;

public sealed class Notification : AuditableAggregateRoot<Guid>
{
    private Notification()
        : base(Guid.Empty)
    {
    }

    private Notification(
        Guid id,
        Guid recipientUserId,
        Guid? projectId,
        string type,
        string subject,
        string message,
        string? relatedEntityType,
        Guid? relatedEntityId)
        : base(id)
    {
        RecipientUserId = recipientUserId;
        ProjectId = projectId;
        Type = ValidateRequired(type, 100, "Notification type");
        Subject = ValidateRequired(subject, 300, "Notification subject");
        Message = ValidateRequired(message, 4000, "Notification message");
        RelatedEntityType = NormalizeOptional(
            relatedEntityType,
            100,
            "Related entity type");
        RelatedEntityId = relatedEntityId;
    }

    public Guid RecipientUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Subject { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public string? RelatedEntityType { get; private set; }

    public Guid? RelatedEntityId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTimeOffset? ReadAtUtc { get; private set; }

    public static Notification Create(
        Guid recipientUserId,
        Guid? projectId,
        string type,
        string subject,
        string message,
        string? relatedEntityType,
        Guid? relatedEntityId)
    {
        if (recipientUserId == Guid.Empty)
        {
            throw new DomainException(
                "Notification recipient identifier is required.");
        }

        return new Notification(
            Guid.CreateVersion7(),
            recipientUserId,
            projectId,
            type,
            subject,
            message,
            relatedEntityType,
            relatedEntityId);
    }

    public void MarkRead(DateTimeOffset readAtUtc)
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        ReadAtUtc = readAtUtc;
    }

    private static string ValidateRequired(
        string value,
        int maximumLength,
        string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{field} is required.");
        }

        if (value.Trim().Length > maximumLength)
        {
            throw new DomainException(
                $"{field} cannot exceed {maximumLength} characters.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(
        string? value,
        int maximumLength,
        string field)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new DomainException(
                $"{field} cannot exceed {maximumLength} characters.");
        }

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
