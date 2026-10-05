# Changelog

All notable changes to this project are documented here.

## [Unreleased]

No unreleased changes yet.

## [0.1.0] - Release candidate

Initial production-ready construction management REST backend baseline:

- Clean Architecture on .NET 10 / C# 14;
- PostgreSQL / EF Core persistence and migrations;
- Identity, JWT authentication, refresh tokens, roles, permissions, and project-scoped authorization;
- companies, projects, project members, locations, work packages, and activities;
- daily progress reporting;
- document register, revisions, attachments, and file-storage abstraction;
- RFIs and Submittals;
- inspections, issues, corrective actions, equipment, suppliers, Purchase Requests, and Purchase Orders;
- notifications and immutable audit history;
- optimized operational reporting API;
- optimistic concurrency;
- PostgreSQL integration/API/architecture testing;
- OpenTelemetry, JSON logging, correlation IDs, security headers, trusted proxy handling, rate limiting, health checks, and configuration validation;
- hardened non-root Docker deployment with explicit migration sequencing;
- automated CI, Docker smoke testing, dependency vulnerability scanning, and tag-driven release publishing.
