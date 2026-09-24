# API Conventions

## Versioning

Public REST resources use URL-based major versions:

```
/api/v1/...
```

Version 1 controllers inherit from `ApiControllerBase`, which applies the `api/v1` route prefix. New incompatible versions should receive separate controller surfaces rather than changing existing v1 contracts in place.

## Responses

Expected application failures are mapped to HTTP using RFC Problem Details:

- validation -> 422 Unprocessable Entity
- not found -> 404 Not Found
- conflict -> 409 Conflict
- unauthenticated -> 401 Unauthorized
- forbidden -> 403 Forbidden
- generic request failure -> 400 Bad Request

Unexpected exceptions return 500 without exposing internal stack traces.

Problem Details includes the ASP.NET Core trace identifier and, when available, the request correlation identifier.

## Correlation

The API accepts or generates `X-Correlation-ID`. Supplied identifiers are limited to 128 characters. The identifier is returned in the response and included in request completion logs.

## Request logging

The middleware logs method, path, status code, elapsed time, and correlation identifier. Request or response bodies are deliberately not logged.

## Rate limiting

A global fixed-window rate limiter partitions by remote IP address. Limits are configuration-driven under the `Api` section.

## CORS

Allowed browser origins are configuration-driven. An empty production origin list does not implicitly enable wildcard CORS.

## Health

- `/health/live` verifies the web process is responsive.
- `/health/ready` executes readiness checks, including PostgreSQL.

## OpenAPI

Development exposes the .NET built-in OpenAPI document at:

```
/openapi/v1.json
```

OpenAPI generation uses `Microsoft.AspNetCore.OpenApi` aligned to the .NET 10 servicing version.
