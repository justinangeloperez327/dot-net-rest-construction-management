FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS restore
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/Construction.Domain/Construction.Domain.csproj src/Construction.Domain/
COPY src/Construction.Application/Construction.Application.csproj src/Construction.Application/
COPY src/Construction.Infrastructure/Construction.Infrastructure.csproj src/Construction.Infrastructure/
COPY src/Construction.Api/Construction.Api.csproj src/Construction.Api/

RUN dotnet restore src/Construction.Api/Construction.Api.csproj

FROM restore AS publish
COPY src ./src

RUN dotnet publish src/Construction.Api/Construction.Api.csproj     --configuration Release     --output /app/publish     --no-restore     -p:UseAppHost=false     && mkdir -p /app/publish/data/uploads

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=publish --chown=app:app /app/publish ./

USER app

EXPOSE 8080

STOPSIGNAL SIGTERM

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3     CMD ["dotnet", "Construction.Api.dll", "--healthcheck", "http://127.0.0.1:8080/health/live"]

ENTRYPOINT ["dotnet", "Construction.Api.dll"]
