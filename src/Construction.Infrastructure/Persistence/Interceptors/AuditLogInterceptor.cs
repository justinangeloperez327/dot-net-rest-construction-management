using System.Text.Json;
using Construction.Application.Abstractions.Authentication;
using Construction.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Construction.Infrastructure.Persistence.Interceptors;

public sealed class AuditLogInterceptor(
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void AddAuditLogs(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        EntityEntry[] entries = dbContext.ChangeTracker
            .Entries()
            .Where(ShouldAudit)
            .ToArray();

        if (entries.Length == 0)
        {
            return;
        }

        DateTimeOffset occurredAtUtc = timeProvider.GetUtcNow();

        foreach (EntityEntry entry in entries)
        {
            string action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => "Changed"
            };

            string changesJson = JsonSerializer.Serialize(
                BuildChanges(entry));

            Guid? projectId = GetProjectId(entry);
            string? entityId = GetEntityId(entry);

            dbContext.Set<AuditLog>().Add(
                AuditLog.CreateEntityChange(
                    action,
                    currentUser.UserId,
                    projectId,
                    entry.Metadata.ClrType.Name,
                    entityId,
                    changesJson,
                    occurredAtUtc));
        }
    }

    private static bool ShouldAudit(EntityEntry entry)
    {
        if (entry.State is not EntityState.Added
            and not EntityState.Modified
            and not EntityState.Deleted)
        {
            return false;
        }

        string? entityNamespace = entry.Metadata.ClrType.Namespace;

        return entityNamespace?.StartsWith(
            "Construction.Domain.",
            StringComparison.Ordinal) == true
            && !entityNamespace.StartsWith(
                "Construction.Domain.Audit",
                StringComparison.Ordinal)
            && !entityNamespace.StartsWith(
                "Construction.Domain.Notifications",
                StringComparison.Ordinal);
    }

    private static Dictionary<string, object?> BuildChanges(
        EntityEntry entry)
    {
        var changes = new Dictionary<string, object?>(
            StringComparer.Ordinal);

        foreach (PropertyEntry property in entry.Properties)
        {
            if (entry.State == EntityState.Modified
                && !property.IsModified)
            {
                continue;
            }

            object? value = entry.State == EntityState.Deleted
                ? property.OriginalValue
                : property.CurrentValue;

            changes[property.Metadata.Name] =
                SanitizeValue(value);
        }

        return changes;
    }

    private static object? SanitizeValue(object? value) =>
        value switch
        {
            null => null,
            byte[] bytes => $"[binary:{bytes.Length}]",
            string text when text.Length > 500 =>
                text[..500] + "[truncated]",
            string text => text,
            _ => value
        };

    private static Guid? GetProjectId(EntityEntry entry)
    {
        PropertyEntry? projectProperty = entry.Properties
            .FirstOrDefault(property =>
                property.Metadata.Name == "ProjectId");

        if (projectProperty?.CurrentValue is Guid projectId
            && projectId != Guid.Empty)
        {
            return projectId;
        }

        if (entry.Metadata.ClrType.Name == "Project")
        {
            PropertyEntry? idProperty = entry.Properties
                .FirstOrDefault(property =>
                    property.Metadata.Name == "Id");

            if (idProperty?.CurrentValue is Guid id
                && id != Guid.Empty)
            {
                return id;
            }
        }

        return null;
    }

    private static string? GetEntityId(EntityEntry entry)
    {
        string[] keyValues = entry.Properties
            .Where(property => property.Metadata.IsPrimaryKey())
            .Select(property =>
            {
                object? value = entry.State == EntityState.Deleted
                    ? property.OriginalValue
                    : property.CurrentValue;

                return value?.ToString() ?? string.Empty;
            })
            .Where(value => value.Length > 0)
            .ToArray();

        return keyValues.Length == 0
            ? null
            : string.Join(",", keyValues);
    }
}
