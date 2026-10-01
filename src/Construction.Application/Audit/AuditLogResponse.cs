using Construction.Domain.Audit;

namespace Construction.Application.Audit;

public sealed record AuditLogResponse(
    Guid Id,
    AuditCategory Category,
    string Action,
    Guid? UserId,
    Guid? ProjectId,
    string? EntityType,
    string? EntityId,
    string? ChangesJson,
    string? Description,
    DateTimeOffset OccurredAtUtc)
{
    public static AuditLogResponse FromDomain(AuditLog auditLog) =>
        new(
            auditLog.Id,
            auditLog.Category,
            auditLog.Action,
            auditLog.UserId,
            auditLog.ProjectId,
            auditLog.EntityType,
            auditLog.EntityId,
            auditLog.ChangesJson,
            auditLog.Description,
            auditLog.OccurredAtUtc);
}
