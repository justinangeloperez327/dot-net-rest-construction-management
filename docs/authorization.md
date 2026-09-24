# Authorization

Authorization uses roles as permission bundles and permissions as the application-facing authorization vocabulary.

## Permission model

A permission is a stable string such as:

```
projects.view
projects.update
projects.members.manage
daily-progress.manage
rfis.manage
purchase-orders.issue
administration.roles.manage
```

The canonical catalog is `Construction.Application.Common.Authorization.Permissions`.

## Roles

Roles are managed by ASP.NET Core Identity. Role claims with claim type `permission` determine the permissions granted by each role.

The Infrastructure seeder guarantees that the built-in `Administrator` role exists and contains every permission in the canonical catalog.

No administrator user is automatically created. Account provisioning remains an explicit administrative operation.

## JWT claims

At login and refresh:

1. the user's Identity roles are resolved;
2. direct user permission claims are loaded;
3. permission claims from every assigned role are loaded;
4. permissions are de-duplicated;
5. role and permission claims are included in the access token.

This makes ordinary HTTP policy checks local to the authenticated request rather than querying the database on every controller action.

## Permission policies

The API can protect endpoints with:

```csharp
[HasPermission(Permissions.Projects.Update)]
```

The attribute creates a dynamic ASP.NET Core policy named:

```
Permission:projects.update
```

`PermissionPolicyProvider` creates the policy and `PermissionAuthorizationHandler` checks the JWT permission claim.

Controllers should not contain role-name checks such as:

```csharp
if (User.IsInRole("ProjectManager"))
```

Business access should be expressed through permissions.

## Application authorization

`IPermissionService` provides database-backed permission resolution for application workflows that need to authorize outside an HTTP policy evaluation.

## Project-level authorization

`IProjectAccessService` defines the boundary for resource-level project authorization.

Its implementation is intentionally deferred until the Project and Project Member aggregates exist. Permission possession alone will not automatically imply access to every project.

## Current user

`GET /api/v1/auth/me` returns the authenticated user's:

- user identifier;
- roles;
- permissions.

## Important distinction

A role answers:

> What organizational responsibility does this account have?

A permission answers:

> What operation may this account perform?

Project membership will later answer:

> On which project may this account perform that operation?
