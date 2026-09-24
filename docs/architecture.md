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
- result and error types;
- validation contracts;
- pagination and sorting primitives;
- strongly typed filtering convention;
- current-user, persistence-boundary, file-storage, email, and notification abstractions.

Application depends only on Domain and the .NET base class library. Persistence, HTTP, authentication mechanisms, and other external technology remain outside the Application layer.

Business entities and use cases are implemented vertically in later groups rather than added as speculative placeholders.
