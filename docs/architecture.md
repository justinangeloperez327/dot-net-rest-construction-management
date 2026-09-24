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

The Domain layer now provides the shared primitives required by later construction-management modules:

- identity-based entities;
- aggregate roots;
- domain events;
- value objects;
- domain exceptions;
- auditable entities.

Business entities such as Project, RFI, Submittal, Inspection, Equipment, Purchase Request, and Purchase Order will be implemented vertically in later groups rather than added as speculative placeholders.
