# Deployment

## Database

PostgreSQL is the relational persistence store. Apply Entity Framework Core migrations as an explicit deployment step using the repository migration scripts.

Do not use `EnsureCreated` in production.

The production reporting indexes are included in the `AddReportingPerformanceIndexes` migration.

## File storage

Document revisions and construction-record attachments store only metadata and storage keys in PostgreSQL. File bytes are persisted through `IFileStorage`.

The current implementation is `LocalFileStorage`.

Configuration:

```text
FileStorage__RootPath=/persistent/path
FileStorage__MaximumFileSizeBytes=104857600
```

The default maximum upload size is 100 MiB.

The configured storage path must be persistent in production. Container-local ephemeral storage is not sufficient.

Docker Compose mounts:

```text
construction-files:/app/data/uploads
```

A future Azure Blob, Amazon S3, or S3-compatible implementation should replace the `IFileStorage` registration without changing application use cases.

## Authentication secrets

Production must provide the JWT signing key through secure configuration:

```text
Authentication__Jwt__SigningKey=<secret>
```

Do not deploy with the development signing key. Production startup rejects the development key.

Database credentials, JWT signing keys, and third-party telemetry credentials must be injected by the deployment platform or secret store rather than committed to configuration files.

## CORS

Production CORS origins must use HTTPS:

```text
Api__AllowedOrigins__0=https://construction.example.com
```

Production startup rejects configured HTTP origins.

## Reverse proxy

When a reverse proxy or ingress terminates TLS, configure its exact IP address so forwarded scheme and client address information can be trusted:

```text
Api__TrustedProxies__0=10.0.0.10
```

The proxy must forward `X-Forwarded-For` and `X-Forwarded-Proto`.

Do not configure untrusted public client addresses as trusted proxies.

Set `AllowedHosts` to the production hostnames appropriate for the deployment.

## Observability

Production logs are emitted as structured JSON.

Optional OTLP export:

```text
Observability__ServiceName=Construction.Api
Observability__TraceSamplingRatio=0.1
Observability__OtlpEndpoint=http://otel-collector:4317
```

An OpenTelemetry Collector is the preferred production destination.

See `docs/observability.md` for tracing, metrics, correlation IDs, security headers, and rate-limiting details.

## Health checks

Use:

```text
/health/live
/health/ready
```

Readiness includes PostgreSQL connectivity.
