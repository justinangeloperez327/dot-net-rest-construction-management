# Release Process

The repository uses stable semantic versions in the form:

```text
vMAJOR.MINOR.PATCH
```

The current release baseline is `0.1.0`.

## Required quality gates

Before tagging a release, the commit on `main` must have successful runs for:

- `CI / build-and-test`;
- `Docker / build-and-smoke`;
- `Security / packages`.

The primary CI job verifies formatting, performs a Release build, runs the full automated test suite, and collects coverage.

Docker CI verifies the real PostgreSQL migration/startup sequence and runtime container health.

Security CI checks direct and transitive NuGet dependencies for known vulnerabilities.

## Version source

`Directory.Build.props` contains:

```xml
<VersionPrefix>0.1.0</VersionPrefix>
```

A release tag must match this value exactly after removing the leading `v`.

Example:

```text
VersionPrefix = 0.1.0
Tag           = v0.1.0
```

The release workflow rejects mismatches.

## Create a release

After all required checks are green:

```bash
git checkout main
git pull --ff-only
git tag -a v0.1.0 -m "Construction Management REST API v0.1.0"
git push origin v0.1.0
```

Pushing the tag starts `.github/workflows/release.yml`.

## Release workflow

The release workflow independently re-verifies:

- semantic release tag;
- repository version;
- restore;
- formatting;
- Release build;
- tests;
- package vulnerability scan.

It then publishes the runtime image to GitHub Container Registry.

For `v0.1.0`, image tags include:

```text
ghcr.io/justinangeloperez327/dot-net-rest-construction-management:0.1.0
ghcr.io/justinangeloperez327/dot-net-rest-construction-management:0.1
ghcr.io/justinangeloperez327/dot-net-rest-construction-management:0
ghcr.io/justinangeloperez327/dot-net-rest-construction-management:latest
ghcr.io/justinangeloperez327/dot-net-rest-construction-management:sha-<commit>
```

The image build includes SBOM and provenance metadata, and the application assembly is stamped with the release version and source revision.

Finally, GitHub Release notes are generated and the production Compose/environment templates are attached.

## Deployment

Set:

```text
CONSTRUCTION_API_IMAGE=ghcr.io/justinangeloperez327/dot-net-rest-construction-management:0.1.0
```

in the production environment and follow `docs/deployment.md`.

Deploy by immutable version tag or digest for production. Do not deploy from a mutable branch tag.

## Version changes

For a future release:

1. update `VersionPrefix`;
2. update `CHANGELOG.md`;
3. merge and verify all quality gates;
4. create the matching `vX.Y.Z` tag;
5. let the release workflow publish the image and GitHub Release.

Do not reuse or move an existing release tag.
