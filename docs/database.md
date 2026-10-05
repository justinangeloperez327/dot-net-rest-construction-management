# Database

Persistence uses PostgreSQL through Entity Framework Core.

## Current versions

- Entity Framework Core: 10.0.12
- Npgsql Entity Framework Core provider: 10.0.3
- PostgreSQL container: 18

Package versions are centrally managed in `Directory.Packages.props`.

## Context boundary

`ApplicationDbContext` belongs to Infrastructure and implements the Application persistence boundary. EF Core types do not leak into Domain or Application public contracts.

Entity configurations are discovered from the Infrastructure assembly.

## Transactions

EF Core supplies transaction semantics for a normal `SaveChanges` operation.

No generic Unit of Work abstraction is layered over EF Core merely for architectural ceremony. Explicit transaction handling should be introduced only for use cases requiring multiple persistence boundaries.

## Auditing

Auditable entities/aggregate roots carry:

```text
CreatedAtUtc
LastModifiedAtUtc
Version
```

Infrastructure persistence interceptors set UTC audit timestamps and advance the numeric concurrency version.

Central audit-log persistence separately records sanitized entity changes and explicit authentication security events.

## Optimistic concurrency

`Version` is configured as an EF Core concurrency token for auditable entities.

Behavior:

```text
insert       -> Version = 1
valid update -> Version increments
stale update -> DbUpdateConcurrencyException
```

The PostgreSQL integration suite verifies stale-write rejection against the real provider.

## Migrations

The repository contains a local `dotnet-ef` tool manifest aligned with EF Core.

Persistent model changes require committed migrations.

Integration startup calls `HasPendingModelChanges()`; CI fails when the runtime model and migration snapshot diverge.

Generated migration files are treated as generated code and excluded from hand-written-code analyzer rules.

## Deployment

The normal API process does not migrate the database automatically.

Deployment runs:

```text
dotnet Construction.Api.dll --migrate
```

as an explicit one-shot phase before API replicas start.

The migration command also runs Identity seed initialization.

See `docs/deployment.md`.

## PostgreSQL 18 container storage

The official PostgreSQL 18 image uses its versioned data directory underneath:

```text
/var/lib/postgresql
```

Both Compose files mount the persistent database volume at that path.

## Health

Infrastructure registers the PostgreSQL readiness check with `ready` and `database` tags.
