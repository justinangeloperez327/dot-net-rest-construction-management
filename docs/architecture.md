# Architecture

The solution follows Clean Architecture.

Dependency direction:

```
Construction.Api
      |
      v
Construction.Application
      |
      v
Construction.Domain

Construction.Infrastructure -> Construction.Application
Construction.Infrastructure -> Construction.Domain
Construction.Api -> Construction.Infrastructure
```

The Domain project must not depend on ASP.NET Core, Entity Framework Core, database providers, authentication libraries, or external integrations.

## Domain kernel

The Domain layer provides:

- identity-based entities;
- aggregate roots;
- domain events;
- value objects;
- domain exceptions;
- auditable entities.

## Application kernel

The Application layer provides:

- commands, queries, handlers, and dispatch contracts;
- pipeline behavior contracts;
- result and application-error types;
- validation contracts;
- pagination and sorting primitives;
- strongly typed filtering convention;
- current-user, persistence-boundary, file-storage, email, and notification abstractions.

Application depends only on Domain and the .NET base class library.

## Infrastructure persistence

Infrastructure now owns:

- Entity Framework Core;
- the PostgreSQL provider;
- `ApplicationDbContext`;
- persistence configuration discovery;
- audit timestamp interception;
- migration tooling and initialization;
- connection resiliency;
- database health checks.

No database provider or Entity Framework Core type crosses into Domain or Application.

Business entities and use cases continue to be implemented vertically in later groups rather than added as speculative placeholders.
