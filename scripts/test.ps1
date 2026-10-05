$ErrorActionPreference = "Stop"

dotnet test Construction.sln --configuration Release

exit $LASTEXITCODE
