# Document and Attachment Storage

## Document register

`ProjectDocument` is the project-scoped document aggregate.

A document has:

- a project-unique immutable document number;
- title;
- category;
- optional description;
- Active or Archived status;
- current internal version number;
- immutable revision history.

Archiving prevents further metadata edits and revision uploads but preserves history and stored files.

## Revisions

Each `DocumentRevision` records:

- sequential version number;
- user-facing revision code;
- original safe file name;
- content type;
- byte length;
- opaque storage key;
- uploader;
- upload timestamp;
- optional revision notes;
- current/superseded state.

Adding a revision marks the previous current revision as superseded. Existing revision rows and files are not overwritten.

## Attachments

`Attachment` provides generic file linkage for non-document-register records.

Current target types:

- Project
- Work Package
- Activity
- Daily Progress Report

The Infrastructure target validator confirms that a target exists and belongs to the requested project before metadata is created.

## Storage boundary

Application code depends only on `IFileStorage`.

The local implementation:

- generates opaque UUIDv7-derived storage keys;
- strips directory information from user file names;
- prevents path traversal;
- enforces the configured maximum byte length;
- verifies the copied byte length;
- uses asynchronous sequential file streams;
- treats deletes as idempotent.

The storage key is never supplied by an API client.

## Database boundary

PostgreSQL stores metadata only. Binary file data is never stored in document or attachment tables.

## Upload limits

`FileStorage:MaximumFileSizeBytes` controls both:

- ASP.NET Core multipart request size;
- storage-layer file-size validation.

The default is 104,857,600 bytes (100 MiB).

## Authorization

Document and attachment reads require:

```text
documents.view
```

Creation, metadata changes, revision uploads, archiving, and attachment deletion require:

```text
documents.manage
```

Normal project membership/resource authorization is applied in addition to the permission.
