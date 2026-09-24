namespace Construction.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAtUtc { get; }

    DateTimeOffset? LastModifiedAtUtc { get; }
}
