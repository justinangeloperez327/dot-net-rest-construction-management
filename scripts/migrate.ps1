$ErrorActionPreference = "Stop"

dotnet tool restore

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

dotnet ef database update `
  --project src/Construction.Infrastructure/Construction.Infrastructure.csproj `
  --startup-project src/Construction.Api/Construction.Api.csproj

exit $LASTEXITCODE
