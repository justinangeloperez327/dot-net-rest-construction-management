# Construction Management REST API

A REST-first construction project management backend built with .NET 10 and Clean Architecture.

## Architecture

- `Construction.Domain` — business model and domain rules
- `Construction.Application` — use cases and application abstractions
- `Construction.Infrastructure` — persistence and external service implementations
- `Construction.Api` — HTTP/REST delivery layer
- `tests/*` — domain, application, integration, and architecture tests

Business modules are scaffolded as directories first. Their implementation will be added vertically in later development groups.
