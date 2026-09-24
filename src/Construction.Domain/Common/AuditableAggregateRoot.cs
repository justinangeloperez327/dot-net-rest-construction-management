namespace Construction.Domain.Common;

public abstract class AuditableAggregateRoot<TId> :
    AggregateRoot<TId>,
    IAuditableEntity
    where TId : notnull
{
    protected AuditableAggregateRoot(TId id)
        : base(id)
    {
    }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? LastModifiedAtUtc { get; private set; }
}
