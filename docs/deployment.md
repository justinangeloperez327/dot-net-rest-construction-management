# Deployment

## Container deployment model

The API uses a multi-stage .NET 10 container build.

Runtime characteristics:

- Ubuntu 24.04 chiseled ASP.NET Core runtime with ICU/tzdata support;
- non-root `app` user;
- read-only root filesystem in Compose;
- all Linux capabilities dropped for the API/migration containers;
- `no-new-privileges`;
- persistent volume only for uploaded files;
- built-in HTTP container health probe;
- SIGTERM stop signal;
- 30-second ASP.NET Core graceful-shutdown timeout;
- 35-second Compose stop grace period.

The SDK exists only in the build stage and is not present in the runtime image.

## Build the image

```bash
docker build --target runtime -t construction-api:latest .
```

The Docker build restores only the production project graph before copying source code, which improves Docker layer reuse when source files change without package changes.

## Local development stack

```bash
docker compose up --build -d
```

The local stack starts in this order:

```text
PostgreSQL healthy
        ↓
migration container completes successfully
        ↓
API starts
        ↓
/health/ready becomes healthy
```

The migration container runs:

```text
dotnet Construction.Api.dll --migrate
```

The normal API process never applies migrations automatically.

Stop and remove local containers:

```bash
docker compose down
```

Remove the local database/file volumes as well:

```bash
docker compose down --volumes
```

## Production Compose

Copy the environment template:

```bash
cp .env.example .env
```

On PowerShell:

```powershell
Copy-Item .env.example .env
```

Replace every placeholder secret and production hostname in `.env`.

Then deploy:

```bash
docker compose -f docker-compose.production.yml up -d
```

Production Compose intentionally does not build the application. It runs the immutable image identified by:

```text
CONSTRUCTION_API_IMAGE
```

A release pipeline or registry should publish this image before deployment.

The production API binds to `127.0.0.1:8080` by default. Put a TLS-terminating reverse proxy, ingress, or managed load balancer in front of it.

## Database migration strategy

Database migrations are an explicit deployment phase.

The production sequence is:

1. start PostgreSQL or verify the managed database is reachable;
2. run one migration container with `--migrate`;
3. require successful completion;
4. start or roll out API instances.

Do not run migrations independently from every API replica during startup. This avoids multiple application instances racing to modify the schema.

The migration command also runs the existing Identity seed process after migrations.

For source-based local administration, the existing scripts remain available:

```bash
./scripts/migrate.sh
```

```powershell
./scripts/migrate.ps1
```

## PostgreSQL

PostgreSQL is the relational persistence store.

Do not use `EnsureCreated` in production.

The Compose PostgreSQL service uses a persistent named volume:

```text
construction-postgres
```

For managed production PostgreSQL, replace the Compose database service/connection string with the managed database endpoint and retain the explicit migration phase.

## File storage

Document revisions and construction-record attachments store metadata/storage keys in PostgreSQL. File bytes are persisted through `IFileStorage`.

The current implementation is `LocalFileStorage`.

Configuration:

```text
FileStorage__RootPath=/app/data/uploads
FileStorage__MaximumFileSizeBytes=104857600
```

The Compose deployment mounts:

```text
construction-files:/app/data/uploads
```

The runtime root filesystem is read-only; this volume is the intended writable application location.

For multiple API replicas, local/host storage is not sufficient unless it is shared storage. Replace `IFileStorage` with Azure Blob, Amazon S3, or another shared object store before horizontally scaling file-serving instances.

## Container health

The image defines a Docker health check using the application binary itself:

```text
dotnet Construction.Api.dll --healthcheck http://127.0.0.1:8080/health/live
```

This avoids installing `curl`, a shell, or other diagnostic packages into the chiseled runtime image.

Application endpoints:

```text
GET /health/live
GET /health/ready
```

Use liveness to restart a failed process.

Use readiness to decide whether new traffic should be routed to the instance. Readiness includes PostgreSQL connectivity.

## Graceful shutdown

ASP.NET Core's hosting lifetime handles SIGTERM. The application configures a 30-second shutdown timeout, and Compose allows 35 seconds before forced termination.

Deployment platforms should send SIGTERM first and configure a termination grace period of at least 35 seconds.

## Authentication secrets

Production must provide:

```text
Authentication__Jwt__SigningKey=<high-entropy-secret>
```

Do not deploy with the development signing key. Production startup rejects it.

Database credentials, JWT signing keys, and telemetry credentials must come from the deployment platform or secret store rather than committed configuration.

## CORS and host filtering

Production CORS origins must use HTTPS:

```text
Api__AllowedOrigins__0=https://construction.example.com
```

Set `AllowedHosts` to the public API hostname:

```text
AllowedHosts=api.construction.example.com
```

## Reverse proxy

When a reverse proxy or ingress terminates TLS, configure its exact address through the deployment environment:

```text
Api__TrustedProxies__0=10.0.0.10
```

The proxy must forward:

```text
X-Forwarded-For
X-Forwarded-Proto
```

Do not trust arbitrary public addresses or all forwarded headers.

## Observability

Production logs are structured JSON.

Optional OTLP export:

```text
Observability__ServiceName=Construction.Api
Observability__TraceSamplingRatio=0.1
Observability__OtlpEndpoint=http://otel-collector:4317
```

An OpenTelemetry Collector is the preferred production destination.

See `docs/observability.md`.

## Docker CI

The Docker workflow verifies:

- the production runtime image builds;
- the configured runtime user is `app`;
- a Docker health check is present;
- PostgreSQL becomes healthy;
- migrations complete;
- the API starts afterward;
- `/health/ready` succeeds;
- Docker reports the API container as healthy.

This runs for pull requests, pushes to `main`, and manual dispatch.
