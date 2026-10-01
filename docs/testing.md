# Testing

The solution uses four test projects with separate responsibilities:

- `Construction.Domain.Tests` verifies aggregate invariants and state machines.
- `Construction.Application.Tests` verifies use-case behavior, authorization boundaries, validation, and orchestration with lightweight fakes.
- `Construction.IntegrationTests` runs against a disposable PostgreSQL 18 container and the real ASP.NET Core application host.
- `Construction.ArchitectureTests` enforces Clean Architecture dependency direction and API controller conventions.

## Test stack

- xUnit v3
- Microsoft.NET.Test.Sdk
- ASP.NET Core MVC testing
- Testcontainers for PostgreSQL
- Coverlet collector

Integration tests intentionally use PostgreSQL rather than EF Core InMemory so provider behavior, constraints, migrations, query translation, and optimistic concurrency are exercised.

## Run locally

Docker must be available for the integration suite.

```bash
dotnet restore Construction.sln
dotnet build Construction.sln --configuration Release
dotnet test Construction.sln --configuration Release --no-build
```

To collect coverage:

```bash
dotnet test Construction.sln \
  --configuration Release \
  --no-build \
  --results-directory artifacts/test-results \
  --logger "trx" \
  --collect "XPlat Code Coverage"
```

## Current quality coverage

The suite includes coverage for:

- core Domain workflow and invariant behavior;
- application authorization and reporting date validation;
- Clean Architecture project-reference rules;
- controller sealing conventions;
- real PostgreSQL unique constraints;
- optimistic concurrency/stale-write rejection;
- reporting SQL projections and aggregates;
- authentication success/failure;
- authenticated current-user access;
- authorization rejection for missing permissions;
- API ProblemDetails error payloads.

CI runs Release build plus all four test projects and stores TRX/coverage output as a workflow artifact.
