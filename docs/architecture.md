# Architecture

The backend follows Clean Architecture with strict inward dependency direction.

## Projects

### Construction.Domain

Owns business behavior:

- entities and aggregate roots;
- invariants and state transitions;
- domain events;
- auditable/concurrency state;
- construction-management business concepts.

It has no dependency on ASP.NET Core, Entity Framework Core, PostgreSQL, Identity, JWT libraries, logging providers, or other external integrations.

### Construction.Application

Owns application use cases and boundaries:

- commands, queries, and handlers;
- application results/errors;
- validation and paging/filtering primitives;
- project-access guards;
- authentication/authorization abstractions;
- persistence abstractions;
- file, notification, email, reporting, and time abstractions.

Application depends on Domain but not Infrastructure or API.

### Construction.Infrastructure

Implements external concerns:

- EF Core / PostgreSQL;
- `ApplicationDbContext`;
- migrations and entity configurations;
- optimistic concurrency persistence;
- ASP.NET Core Identity stores;
- JWT implementation;
- local file storage;
- notification/email adapters;
- audit persistence;
- optimized read-only reporting projections;
- database health checks.

Infrastructure depends on Application and Domain.

### Construction.Api

Owns the HTTP boundary:

- `/api/v1` controllers;
- contracts;
- authentication/authorization middleware;
- RFC Problem Details;
- exception translation;
- correlation and W3C trace IDs;
- structured request logging;
- CORS;
- rate limiting;
- trusted forwarded headers;
- security headers/HSTS;
- OpenAPI;
- liveness/readiness endpoints;
- OpenTelemetry registration;
- production configuration validation;
- container migration and health-probe command modes.

API depends on Application and Infrastructure.

## Dependency direction

```text
Construction.Api
      |
      +------> Construction.Application
      |                 |
      |                 v
      |          Construction.Domain
      |
      +------> Construction.Infrastructure
                        |
                        +----> Construction.Application
                        +----> Construction.Domain
```

No HTTP or EF Core type is exposed through Domain or Application.

## Application structure

Application features are organized vertically by use case instead of a single broad service layer.

Examples:

```text
Projects/CreateProject
Projects/UpdateProject
Reports/GetProjectSummary
Rfis/AnswerRfi
PurchaseOrders/ReceivePurchaseOrder
```

Controllers remain thin:

```text
HTTP request
   ↓
Application handler
   ↓
Result
   ↓
HTTP response
```

## Persistence approach

EF Core is used directly behind application-specific abstractions. The architecture deliberately avoids a generic `IRepository<T>` layer over every entity.

Read/report queries favor:

- projection;
- `AsNoTracking()`;
- database-side grouping/aggregation;
- targeted indexes;
- bounded exception lists.

## Cross-cutting behavior

The system includes:

- immutable audit history;
- UTC audit timestamps;
- numeric optimistic-concurrency versions;
- project-scoped authorization;
- correlation IDs and W3C traces;
- structured JSON production logging;
- OpenTelemetry;
- health checks;
- explicit deployment migrations.

## Enforcement

`Construction.ArchitectureTests` verifies core dependency rules and API conventions as part of CI.
