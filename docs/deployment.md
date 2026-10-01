# Deployment

## Database

PostgreSQL is the relational persistence store. Apply Entity Framework Core migrations as an explicit deployment step using the repository migration scripts.

Do not use `EnsureCreated` in production.

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

Do not deploy with the development signing key.

## Health checks

Use:

```text
/health/live
/health/ready
```

Readiness includes PostgreSQL connectivity.
