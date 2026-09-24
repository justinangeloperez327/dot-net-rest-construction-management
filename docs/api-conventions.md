# API Conventions

- REST resources use `/api/v1`.
- Controllers remain thin.
- Application use cases own orchestration.
- Domain objects own business invariants.
- Errors will use RFC Problem Details.
- List endpoints will support consistent pagination, filtering, sorting, and search where appropriate.
