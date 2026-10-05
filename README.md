# Construction Management REST API

Production-oriented construction project management backend built with .NET 10, C# 14, PostgreSQL, and Clean Architecture.

Repository version: **0.1.0**

This repository is REST/backend only. It does not contain Blazor, Razor, React, or another frontend.

## Implemented capabilities

- ASP.NET Core REST API under `/api/v1`
- PostgreSQL 18 with Entity Framework Core migrations
- ASP.NET Core Identity
- JWT access tokens and refresh-token rotation/reuse protection
- role, permission, and project-scoped authorization
- Companies and Projects
- Project Members and hierarchical Project Locations
- Work Packages, Activities, assignments, and dependencies
- Daily Progress reports
- document register, immutable revisions, attachments, and file-storage abstraction
- RFIs and Submittals
- Inspections, Issues, and Corrective Actions
- Equipment and maintenance
- Suppliers, Purchase Requests, and Purchase Orders
- user Notifications and preferences
- immutable Audit history and authentication security events
- project Reporting API
- optimistic concurrency
- RFC Problem Details
- OpenTelemetry tracing/metrics
- JSON production logging
- correlation/trace IDs
- health checks
- security headers, trusted proxies, CORS, and rate limiting
- PostgreSQL-backed integration/API tests
- hardened non-root Docker deployment
- CI, Docker smoke tests, vulnerability scanning, and tag-driven releases

## Architecture

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

Projects:

- `Construction.Domain` — business rules, aggregates, state machines, and domain events
- `Construction.Application` — use cases, authorization guards, results, and external abstractions
- `Construction.Infrastructure` — EF Core/PostgreSQL, Identity, JWT, storage, notifications, and reporting projections
- `Construction.Api` — HTTP delivery, middleware, authentication, OpenAPI, health checks, and configuration
- `tests/*` — Domain, Application, Integration/API, and Architecture tests

The dependency direction is enforced by architecture tests.

See [docs/architecture.md](docs/architecture.md).

## Requirements

For source development:

- .NET SDK 10.0.401 or a compatible latest patch selected by `global.json`
- Docker for PostgreSQL-backed integration tests

For the containerized local stack:

- Docker with Compose

## Quick start

The simplest complete local environment is:

```bash
docker compose up --build -d
```

This starts PostgreSQL, runs EF Core migrations in a one-shot migration container, and then starts the API.

API:

```text
http://localhost:8080
```

Health:

```text
GET /health/live
GET /health/ready
```

Development OpenAPI:

```text
GET /openapi/v1.json
```

Stop:

```bash
docker compose down
```

## Source build and tests

```bash
dotnet restore Construction.sln
dotnet format Construction.sln --verify-no-changes --no-restore
dotnet build Construction.sln --configuration Release --no-restore
dotnet test --solution Construction.sln --configuration Release --no-build
```

Integration tests use a disposable PostgreSQL 18 container. EF Core InMemory is intentionally not used for provider-sensitive behavior.

See [docs/testing.md](docs/testing.md).

## Configuration

Primary production settings are supplied through environment variables or a secret/configuration store.

Examples:

```text
ConnectionStrings__Database
Authentication__Jwt__SigningKey
Api__AllowedOrigins__0
Api__TrustedProxies__0
AllowedHosts
FileStorage__RootPath
Observability__OtlpEndpoint
Observability__TraceSamplingRatio
```

Production startup rejects insecure JWT/CORS configuration.

See:

- [docs/deployment.md](docs/deployment.md)
- [docs/observability.md](docs/observability.md)

## API conventions

Public routes use:

```text
/api/v1/...
```

Expected failures use RFC Problem Details. Project-scoped resources require both permission possession and project access unless the caller has explicit global project access.

See:

- [docs/api-conventions.md](docs/api-conventions.md)
- [docs/authentication.md](docs/authentication.md)
- [docs/authorization.md](docs/authorization.md)
- [docs/modules.md](docs/modules.md)

## Database

The application uses PostgreSQL through EF Core.

Migrations are explicit deployment operations. Normal API replicas do not apply migrations during startup.

See [docs/database.md](docs/database.md).

## Containers

Runtime containers use a pinned .NET 10 chiseled ASP.NET Core image and execute as the non-root `app` user.

Local:

```bash
docker compose up --build -d
```

Production template:

```text
docker-compose.production.yml
.env.example
```

See [docs/deployment.md](docs/deployment.md).

## CI and release

Required release-quality workflows are:

- `CI / build-and-test`
- `Docker / build-and-smoke`
- `Security / packages`

Stable releases use tags such as:

```text
v0.1.0
```

A matching tag triggers the release workflow, which re-runs release checks, publishes the versioned runtime image to GitHub Container Registry, and creates the GitHub Release.

See:

- [docs/release.md](docs/release.md)
- [CHANGELOG.md](CHANGELOG.md)
