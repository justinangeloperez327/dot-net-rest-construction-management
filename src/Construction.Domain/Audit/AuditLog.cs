using Construction.Domain.Common;

namespace Construction.Domain.Audit;

public sealed class AuditLog : Entity<Guid>
{
    private AuditLog()
        : base(Guid.Empty)
    {
    }

    private AuditLog(
        Guid id,
        AuditCategory category,
        string action,
        Guid? userId,
        Guid? projectId,
        string? entityType,
        string? entityId,
        string? changesJson,
        string? description,
        DateTimeOffset occurredAtUtc)
        : base(id)
    {
        Category = category;
        Action = ValidateRequired(action, 100, "Audit action");
        UserId = userId;
        ProjectId = projectId;
        EntityType = NormalizeOptional(entityType, 200, "Entity type");
        EntityId = NormalizeOptional(entityId, 200, "Entity identifier");
        ChangesJson = NormalizeOptional(changesJson, 20000, "Audit changes");
        Description = NormalizeOptional(description, 2000, "Audit description");
        OccurredAtUtc = occurredAtUtc;
    }

    public AuditCategory Category { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public Guid? UserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public string? EntityType { get; private set; }

    public string? EntityId { get; private set; }

    public string? ChangesJson { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public static AuditLog CreateEntityChange(
        string action,
        Guid? userId,
        Guid? projectId,
        string entityType,
        string? entityId,
        string changesJson,
        DateTimeOffset occurredAtUtc) =>
        new(
            Guid.CreateVersion7(),
            AuditCategory.EntityChange,
            action,
            userId,
            projectId,
            entityType,
            entityId,
            changesJson,
            null,
            occurredAtUtc);

    public static AuditLog CreateSecurity(
        string action,
        Guid? userId,
        string? description,
        DateTimeOffset occurredAtUtc) =>
        new(
            Guid.CreateVersion7(),
            AuditCategory.Security,
            action,
            userId,
            null,
            null,
            null,
            null,
            description,
            occurredAtUtc);

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
