# Authentication

The REST API uses ASP.NET Core Identity for account/password storage and JWT bearer tokens for API authentication.

## Account boundary

Identity is an Infrastructure concern. `ApplicationUser`, roles, password hashes, lockout state, refresh tokens, and EF Core Identity tables do not leak into Domain or Application.

Public self-registration is intentionally not exposed. Construction-management accounts should be provisioned through controlled administration workflows.

## Access tokens

Access tokens are JWTs signed with HMAC SHA-256 and contain user identifier, email, display name, token identifier, and role claims.

The default access-token lifetime is 15 minutes.

The signing key must be at least 32 bytes. Production must supply `Authentication__Jwt__SigningKey` through secure configuration or a secrets provider.

## Refresh tokens

Refresh tokens are cryptographically random 64-byte values.

Only SHA-256 hashes of refresh tokens are stored in PostgreSQL. Raw refresh tokens are returned once to the client and are never persisted.

A refresh operation rotates the token. Reuse of a refresh token that has already been rotated triggers revocation of remaining active refresh tokens for that user.

Logout revokes the submitted refresh token and is idempotent.

## Password and lockout defaults

- minimum password length: 12 characters;
- uppercase required;
- lowercase required;
- digit required;
- non-alphanumeric character required;
- unique email required;
- lockout after 5 failed attempts;
- lockout duration: 15 minutes.

## Endpoints

```
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
```

Login and refresh failures intentionally use generic error messages to avoid account enumeration.

## Development key

The development configuration contains an explicitly non-production signing key for local execution. Production configuration leaves the signing key empty and startup validation rejects a missing or short key.
