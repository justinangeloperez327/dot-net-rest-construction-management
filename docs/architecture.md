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

The Domain layer provides identity-based entities, aggregate roots, domain events, value objects, domain exceptions, and auditable entities.

## Application kernel

The Application layer provides commands, queries, handlers, dispatch contracts, result/application-error types, validation contracts, pagination/sorting/filtering primitives, and abstractions for persistence and external capabilities.

Application depends only on Domain and the .NET base class library.

## Infrastructure persistence

Infrastructure owns Entity Framework Core, PostgreSQL, `ApplicationDbContext`, migrations, persistence interceptors, connection resiliency, and database health checks.

## API delivery layer

The API project owns HTTP concerns:

- `/api/v1` routing;
- controllers;
- RFC Problem Details;
- exception-to-HTTP translation;
- Application Result-to-HTTP translation;
- correlation IDs;
- request completion logging;
- CORS;
- rate limiting;
- health endpoints;
- OpenAPI generation.

No HTTP type is exposed through Application or Domain.

Business entities and use cases continue to be implemented vertically in later groups.
