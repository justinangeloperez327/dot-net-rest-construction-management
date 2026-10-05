# API Conventions

## Versioning

Public REST resources use URL-based major versions:

```text
/api/v1/...
```

Breaking API changes must not silently mutate an existing public v1 contract.

## Authentication and authorization

Protected endpoints require JWT authentication.

Authorization combines:

- permissions;
- global roles where applicable;
- active project membership/project access for project-scoped resources;
- explicit `projects.access-all` only where global project access is intended.

## Responses

Expected application failures are mapped through RFC Problem Details:

- validation -> 422 Unprocessable Entity
- not found -> 404 Not Found
- conflict -> 409 Conflict
- unauthenticated -> 401 Unauthorized
- forbidden -> 403 Forbidden
- generic request failure -> 400 Bad Request

Unexpected exceptions return 500 without exposing internal stack traces.

Problem Details includes tracing/correlation context where available.

## Correlation and tracing

The API accepts or generates:

```text
X-Correlation-ID
```

Client values are accepted only when they:

- are 64 characters or fewer;
- contain ASCII letters, digits, `-`, `_`, or `.`.

Invalid values are replaced.

Responses also expose:

```text
X-Trace-ID
```

using the active W3C trace identifier.

## Request logging

Request completion logs contain:

- method;
- path without query-string values;
- status code;
- elapsed milliseconds;
- correlation ID;
- trace ID;
- authenticated user ID when present.

Request bodies and authentication secrets are not logged.

## Rate limiting

The global fixed-window limiter partitions by:

1. authenticated user ID for authenticated requests;
2. effective client IP for anonymous requests.

A rejected request returns HTTP 429 and Problem Details. `Retry-After` is included when the limiter provides it.

## Reverse proxies

`X-Forwarded-For` and `X-Forwarded-Proto` are processed only through configured trusted proxies. One forwarded hop is accepted.

This protects HTTPS detection and IP-based anonymous throttling from arbitrary client-controlled forwarding headers.

## Security headers

Responses include defense-oriented API headers including:

- `X-Content-Type-Options: nosniff`;
- `X-Frame-Options: DENY`;
- `Referrer-Policy: no-referrer`;
- restrictive Permissions Policy;
- restrictive Content Security Policy.

Production enables HSTS.

## CORS

Allowed origins are configuration-driven. Production requires configured origins to use HTTPS. An empty origin list does not enable wildcard CORS.

## Health

```text
GET /health/live
GET /health/ready
```

Liveness verifies the HTTP process is responsive.

Readiness includes PostgreSQL connectivity.

## OpenAPI

Development exposes the built-in OpenAPI document at:

```text
/openapi/v1.json
```

OpenAPI is not exposed automatically in production.

## Reporting

Reporting endpoints are read-only and project-scoped. Expensive exception lists are bounded, read models are projected directly from PostgreSQL, and monetary totals are kept separated by currency.
