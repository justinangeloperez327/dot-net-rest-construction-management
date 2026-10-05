# Construction Management REST API

A REST-first construction project management backend built with .NET 10 and Clean Architecture.

## Architecture

- `Construction.Domain` — business model and domain rules
- `Construction.Application` — use cases and application abstractions
- `Construction.Infrastructure` — persistence and external service implementations
- `Construction.Api` — HTTP/REST delivery layer
- `tests/*` — domain, application, integration, and architecture tests

Business modules are scaffolded as directories first. Their implementation will be added vertically in later development groups.


## Containers

Build and run the complete local stack:

```bash
docker compose up --build -d
```

The stack runs PostgreSQL, applies EF Core migrations in a one-shot migration container, then starts the API on:

```text
http://localhost:8080
```

Health endpoints:

```text
GET /health/live
GET /health/ready
```

For production deployment, see [docs/deployment.md](docs/deployment.md) and use `docker-compose.production.yml`.
