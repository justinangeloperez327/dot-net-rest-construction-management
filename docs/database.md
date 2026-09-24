# Database

The persistence implementation uses PostgreSQL through Entity Framework Core.

## Current versions

- Entity Framework Core: 10.0.12
- Npgsql Entity Framework Core provider: 10.0.3
- PostgreSQL development container: 18

Package versions are centrally managed in `Directory.Packages.props`.

## Context boundary

`ApplicationDbContext` implements the Application-layer `IApplicationDbContext` contract. No Entity Framework Core types are exposed through the Application or Domain projects.

Entity configurations are discovered from the Infrastructure assembly. Feature-specific configuration classes will be added alongside each persistent domain module.

## Transactions

Entity Framework Core already wraps a single `SaveChanges` operation in a transaction when supported by the provider. An explicit cross-operation transaction abstraction is intentionally deferred until a real use case requires multiple persistence boundaries in one application operation.

## Auditing

`AuditableEntityInterceptor` applies UTC creation and modification timestamps using the .NET `TimeProvider` abstraction. HTTP context and user identity are not accessed by the persistence interceptor.

## Concurrency

Concurrency tokens are configured per aggregate when the aggregate is introduced. A global concurrency property is not imposed on every entity because not every entity has the same conflict semantics.

## Migrations

The repository contains a local `dotnet-ef` tool manifest pinned to the matching EF Core patch version.

No empty initial migration is committed. The first migration should be created with the first persistent business aggregate so the migration represents an actual schema.

## Database initialization

`DatabaseInitializer` applies pending migrations. Deployment environments should invoke migration initialization deliberately rather than relying on schema creation through `EnsureCreated`.

## Health checks

Infrastructure registers an Entity Framework Core database health check named `postgresql` with `ready` and `database` tags.
