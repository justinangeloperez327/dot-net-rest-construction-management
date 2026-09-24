# Application Foundation

Group 3 establishes the Application layer contracts used by future vertical features.

## Commands and queries

Use cases are represented as explicit commands and queries:

- `ICommand` and `ICommand<TResponse>` for state-changing operations;
- `IQuery<TResponse>` for read operations;
- dedicated command/query handlers;
- application-level dispatch contracts.

The interfaces are deliberately independent of a mediator library. The composition layer can implement dispatch through the built-in dependency-injection container without forcing every feature to depend on a third-party mediator package.

## Results and errors

Expected application outcomes use `Result` and `Result<T>`.

Error categories are:

- failure;
- validation;
- not found;
- conflict;
- unauthorized;
- forbidden.

Normal validation, lookup, conflict, and authorization outcomes should not require exceptions.

## Validation

Validation is expressed through `IValidator<TRequest>`. Validators return structured failures.

Pipeline behavior contracts are present, but concrete validation/logging/transaction behaviors will be wired only when the dispatcher and runtime composition are implemented.

## Queries

List/query foundations include:

- page requests;
- paged results;
- sorting;
- a base type for feature-specific typed filters.

Filtering should remain feature-specific and strongly typed. The application does not expose arbitrary property-name/value dictionaries as a generic filtering mechanism.

## External abstractions

The Application layer owns interfaces for external capabilities:

- current authenticated user;
- application database save boundary;
- file storage;
- email;
- application notifications.

Database entity sets will be added to `IApplicationDbContext` only as real domain modules are implemented. This avoids speculative persistence contracts.

For time, use the .NET `TimeProvider` abstraction directly rather than creating a redundant custom clock interface.

## Non-goals

This group intentionally does not add:

- Entity Framework Core;
- ASP.NET Core types;
- PostgreSQL;
- MediatR;
- FluentValidation;
- generic repositories;
- authentication/token implementations;
- business feature handlers.
