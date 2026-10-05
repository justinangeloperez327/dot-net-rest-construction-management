# Observability and Production Hardening

## Structured logging

Production uses the built-in JSON console logger with UTC timestamps and scopes.

Request logs include:

- HTTP method;
- request path without query-string values;
- response status;
- elapsed milliseconds;
- correlation ID;
- W3C trace ID;
- authenticated user ID when available.

Request bodies, authorization headers, JWTs, refresh tokens, database connection strings, and file contents are not logged by the request logging middleware.

## Correlation and tracing

Every response includes:

```text
X-Correlation-ID
X-Trace-ID
```

Clients may provide `X-Correlation-ID`. Supplied values are accepted only when they are 64 characters or fewer and contain ASCII letters, digits, `-`, `_`, or `.`. Invalid values are replaced with a generated UUID.

The correlation ID is also attached to the current OpenTelemetry activity.

## OpenTelemetry

The API instruments:

- ASP.NET Core requests;
- outbound `HttpClient` calls;
- .NET runtime metrics;
- Npgsql activity sources.

Configuration:

```text
Observability__ServiceName=Construction.Api
Observability__TraceSamplingRatio=0.1
Observability__OtlpEndpoint=http://otel-collector:4317
```

`TraceSamplingRatio` must be between `0` and `1`.

If `OtlpEndpoint` is empty, telemetry is instrumented but no OTLP exporter is configured. Production deployments should normally send OTLP telemetry to an OpenTelemetry Collector rather than coupling the API directly to a vendor backend.

Health-check endpoints are excluded from ASP.NET Core trace instrumentation to reduce telemetry noise.

## Health checks

```text
GET /health/live
GET /health/ready
```

Liveness verifies that the process can serve HTTP.

Readiness includes PostgreSQL connectivity and should be used by the orchestrator before routing traffic to the instance.

## Rate limiting

The global fixed-window limiter partitions traffic by:

1. authenticated user ID when a valid JWT is present;
2. otherwise, client IP address.

Rejected requests return HTTP `429`, `application/problem+json`, and `Retry-After` when the limiter can calculate the next permit window.

Configuration:

```text
Api__RateLimitPermitLimit=120
Api__RateLimitWindowSeconds=60
Api__RateLimitQueueLimit=0
```

## Reverse proxies

The API processes `X-Forwarded-For` and `X-Forwarded-Proto` only from trusted proxies. Do not configure arbitrary client-controlled forwarding headers as trusted.

Configure exact proxy addresses:

```text
Api__TrustedProxies__0=10.0.0.10
Api__TrustedProxies__1=10.0.0.11
```

Only one forwarded hop is processed.

This is important because HTTPS detection and unauthenticated rate-limit partitioning depend on the effective request scheme and client IP.

## Security headers

Responses include:

```text
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Referrer-Policy: no-referrer
Permissions-Policy: camera=(), microphone=(), geolocation=()
Content-Security-Policy: default-src 'none'; frame-ancestors 'none'; base-uri 'none'
```

Production also enables HSTS. HTTPS redirection remains enabled by the application; if a reverse proxy terminates TLS, it must forward the original scheme from a configured trusted proxy.

## Production configuration validation

Production startup fails when:

- the JWT signing key is missing;
- the development signing key is used;
- a configured CORS origin is not an absolute HTTPS URI;
- a trusted proxy value is not a valid IP address;
- the OTLP endpoint is invalid;
- the trace sampling ratio is outside `0..1`;
- rate-limit values are invalid.

Secrets should be supplied by the deployment platform or secret store, not committed to `appsettings.json`.

## Reporting query indexes

The production-readiness review added targeted indexes for high-frequency overdue reporting predicates:

- activities by project, status, and planned end date;
- issues by project, status, and due date;
- current Submittal revisions by Submittal, current flag, and review due date.

These are intentionally narrow indexes based on actual reporting predicates rather than speculative indexing of every filterable column.
