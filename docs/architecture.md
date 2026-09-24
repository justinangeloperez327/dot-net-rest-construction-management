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
