#!/usr/bin/env bash
set -euo pipefail

dotnet tool restore

dotnet ef database update \
  --project src/Construction.Infrastructure/Construction.Infrastructure.csproj \
  --startup-project src/Construction.Api/Construction.Api.csproj
