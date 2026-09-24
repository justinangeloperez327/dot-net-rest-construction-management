# Domain Foundation

Group 2 introduces the dependency-free domain kernel used by construction-management aggregates.

## Entity identity

`Entity<TId>` defines identity-based equality. Two entities are equal only when they are the same concrete type and have the same identifier.

Identifiers are assigned when an entity is created. The domain does not depend on database-generated identity.

## Aggregate roots

`AggregateRoot<TId>` owns domain events. Derived aggregates raise events through `RaiseDomainEvent`. Infrastructure will later dequeue and dispatch those events after persistence.

## Domain events

Domain events carry:

- a version-7 GUID event identifier;
- an explicit UTC occurrence time.

The occurrence time is supplied by the caller rather than read from the system clock inside the domain. This keeps domain behavior deterministic and testable.

## Value objects

`ValueObject` provides component-based equality for value objects that are not represented as C# records. Individual domain modules remain free to use validated records when that is clearer.

## Auditing

`AuditableEntity<TId>` exposes creation and modification timestamps with private setters. Persistence interceptors will populate them in the Infrastructure layer in a later group.

User/action audit history remains a separate construction-management concern and is not embedded into the domain base class.

## Boundaries

The Domain project must remain free of:

- ASP.NET Core
- Entity Framework Core
- PostgreSQL providers
- authentication/token packages
- logging frameworks
- messaging infrastructure
- file-storage SDKs
