# Development

## Prerequisites

- .NET 10 SDK selected through `global.json`
- Docker

## Recommended local stack

Run the complete API/database environment with:

```bash
docker compose up --build -d
```

Startup order is:

```text
PostgreSQL
  ↓
EF Core migration container
  ↓
API
```

API:

```text
http://localhost:8080
```

Health:

```text
/health/live
/health/ready
```

Stop:

```bash
docker compose down
```

## Source workflow

Restore:

```bash
dotnet restore Construction.sln
```

Check formatting:

```bash
dotnet format Construction.sln --verify-no-changes --no-restore
```

Build:

```bash
dotnet build Construction.sln --configuration Release --no-restore
```

Test:

```bash
dotnet test --solution Construction.sln --configuration Release --no-build
```

Integration tests require Docker because they start PostgreSQL through Testcontainers.

## Migrations

Create migrations from the repository root using the local EF tool after restoring tools:

```bash
dotnet tool restore
dotnet ef migrations add <MigrationName> \
  --project src/Construction.Infrastructure/Construction.Infrastructure.csproj \
  --startup-project src/Construction.Api/Construction.Api.csproj \
  --output-dir Persistence/Migrations
```

Apply migrations from source:

```bash
./scripts/migrate.sh
```

PowerShell:

```powershell
./scripts/migrate.ps1
```

Do not use `EnsureCreated` for application databases.

## Configuration

Development defaults are in `src/Construction.Api/appsettings.Development.json`.

Production secrets must not be copied into committed settings files.

See `docs/deployment.md`.
