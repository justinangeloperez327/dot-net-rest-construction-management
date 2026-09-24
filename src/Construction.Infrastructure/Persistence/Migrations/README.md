# Entity Framework Core Migrations

Migrations live in this directory once the first persistent business entity is introduced.

Create a migration:

```bash
dotnet tool restore
dotnet ef migrations add <MigrationName> \
  --project src/Construction.Infrastructure/Construction.Infrastructure.csproj \
  --startup-project src/Construction.Api/Construction.Api.csproj \
  --output-dir Persistence/Migrations
```

Apply migrations:

```bash
dotnet tool restore
dotnet ef database update \
  --project src/Construction.Infrastructure/Construction.Infrastructure.csproj \
  --startup-project src/Construction.Api/Construction.Api.csproj
```

For design-time commands outside local Docker defaults, set `CONSTRUCTION_DB_CONNECTION`.
