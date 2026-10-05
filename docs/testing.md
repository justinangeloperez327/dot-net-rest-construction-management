# Testing

The solution uses four test projects with distinct responsibilities.

## Construction.Domain.Tests

Verifies aggregate invariants and state machines, including project/activity/equipment/issue/procurement behavior.

## Construction.Application.Tests

Verifies application orchestration such as authorization boundaries, project-access checks, validation, and reporting date logic using lightweight fakes.

## Construction.IntegrationTests

Runs against a disposable PostgreSQL 18 container and the real ASP.NET Core host.

Coverage includes:

- migrations/model-snapshot integrity;
- real database constraints;
- optimistic concurrency;
- reporting SQL projections;
- authentication success/failure;
- JWT current-user access;
- 401/403 behavior;
- Problem Details;
- security headers;
- correlation/trace behavior;
- liveness/readiness;
- rate limiting and `Retry-After`;
- production configuration validation.

EF Core InMemory is intentionally not used for provider-sensitive tests.

## Construction.ArchitectureTests

Enforces:

- Domain has no Application/Infrastructure/API dependency;
- Application has no Infrastructure/API dependency;
- Infrastructure has no API dependency;
- API controller conventions.

## Test stack

- xUnit v3
- Microsoft Testing Platform
- ASP.NET Core MVC testing
- Testcontainers for PostgreSQL
- Microsoft code coverage extension

## Local execution

Docker must be available for the integration suite.

```bash
dotnet restore Construction.sln
dotnet format Construction.sln --verify-no-changes --no-restore
dotnet build Construction.sln --configuration Release --no-restore
dotnet test --solution Construction.sln --configuration Release --no-build
```

Coverage:

```bash
dotnet test \
  --solution Construction.sln \
  --configuration Release \
  --no-build \
  --results-directory artifacts/test-results \
  --coverage \
  --coverage-output-format cobertura
```

## CI quality gate

`CI / build-and-test` performs:

```text
restore
format verification
Release build
all four test projects
coverage collection
test-result artifact upload
```

The CI command sets a minimum expected discovered-test count so a broken test-discovery configuration cannot silently pass with zero tests.

Docker and dependency-security checks are separate required release gates described in `docs/release.md`.
