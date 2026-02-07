<!--
Sync Impact Report
- Version change: 0.0.0 → 1.0.0
- Modified principles: N/A (initial constitution)
- Added sections:
  - Core Principles: CQRS with Vertical Slices, Test-Driven Development,
    Clean Architecture Layering, Simplicity & YAGNI
  - Technology Constraints
  - Development Workflow
  - Governance
- Removed sections: None
- Templates requiring updates:
  - .specify/templates/plan-template.md ✅ no update needed
    (Constitution Check section is dynamic, filled at plan time)
  - .specify/templates/spec-template.md ✅ no update needed
    (template is generic, spec content adapts to principles)
  - .specify/templates/tasks-template.md ✅ no update needed
    (TDD task ordering already supported via template structure)
- Follow-up TODOs: None
-->

# CitytripPlanner Constitution

## Core Principles

### I. CQRS with Vertical Slices

- Every feature MUST be implemented using the CQRS pattern,
  separating Commands (write operations) from Queries
  (read operations).
- Each vertical slice MUST live in its own folder under the
  `CitytripPlanner.Features` project, grouped by feature domain.
- A vertical slice folder MUST contain all artifacts for that
  slice: command/query, handler, validator (if needed), and
  response DTO.
- Slices MUST NOT depend on other slices directly. Shared
  concerns go into the `CitytripPlanner.Infrastructure` project.
- Folder structure example:
  ```
  CitytripPlanner.Features/
  └── Trips/
      ├── CreateTrip/
      │   ├── CreateTripCommand.cs
      │   ├── CreateTripHandler.cs
      │   └── CreateTripValidator.cs
      ├── GetTrip/
      │   ├── GetTripQuery.cs
      │   ├── GetTripHandler.cs
      │   └── TripResponse.cs
      └── ListTrips/
          ├── ListTripsQuery.cs
          ├── ListTripsHandler.cs
          └── ListTripsResponse.cs
  ```

**Rationale**: Vertical slices keep each feature self-contained,
reduce merge conflicts, and make it straightforward to reason
about, test, and delete individual features.

### II. Test-Driven Development (NON-NEGOTIABLE)

- TDD MUST be followed for every feature: write a failing test
  first, then implement the minimum code to pass, then refactor.
- The Red-Green-Refactor cycle MUST be strictly enforced.
- No production code may be written without a corresponding
  failing test preceding it.
- Each vertical slice MUST have at least one unit test for
  its handler.
- Integration tests MUST be written for cross-cutting
  scenarios: database access, external API calls, and
  end-to-end user journeys.
- Test projects MUST mirror the feature folder structure for
  discoverability.

**Rationale**: TDD produces code that is correct by
construction, keeps test coverage high without measuring it
as a vanity metric, and forces small incremental steps.

### III. Clean Architecture Layering

- The solution MUST maintain three projects with strict
  dependency direction:
  - `CitytripPlanner.Web` → presentation, Blazor pages,
    API endpoints. Depends on Features.
  - `CitytripPlanner.Features` → application logic, CQRS
    handlers, domain models. No infrastructure dependencies.
  - `CitytripPlanner.Infrastructure` → data access, external
    services, cross-cutting concerns. Depends on Features.
- The Features project MUST NOT reference Web or
  Infrastructure directly. Use dependency inversion
  (interfaces in Features, implementations in Infrastructure).
- Domain models MUST live in the Features project alongside
  their related slices or in a shared `Domain/` folder within
  Features if used across multiple slices.

**Rationale**: Strict layering keeps the domain logic
testable in isolation and prevents accidental coupling to
infrastructure or presentation concerns.

### IV. Simplicity & YAGNI

- Start with the simplest implementation that satisfies the
  current requirement. Do not build for hypothetical futures.
- Abstractions MUST be justified by an immediate need (e.g.,
  multiple implementations, testability boundary). A single
  concrete implementation does not warrant an interface unless
  it crosses a layer boundary (see Principle III).
- Prefer deleting code over commenting it out.
- Three similar lines of code are preferable to a premature
  abstraction.

**Rationale**: Complexity is the primary enemy of
maintainability. Every abstraction has a cost; pay it only
when the benefit is proven.

## Technology Constraints

- **Runtime**: .NET 10
- **Frontend**: Blazor (server or WASM as determined per
  feature)
- **Language features**: Nullable reference types MUST be
  enabled in all projects. Implicit usings enabled.
- **Dependencies**: New NuGet packages MUST be justified in
  the implementation plan before adoption. Prefer the
  framework's built-in capabilities.

## Development Workflow

- Every feature branch MUST be created from `main`.
- Commits MUST be atomic: one logical change per commit.
- Pull requests MUST pass all tests before merge.
- Each PR MUST be scoped to a single vertical slice or a
  single cross-cutting concern — never both.

## Governance

- This constitution supersedes all other development practices
  for the CitytripPlanner project.
- Amendments require: (1) a written proposal describing the
  change, (2) rationale for the change, and (3) a migration
  plan if the change affects existing code.
- Version follows semantic versioning: MAJOR for
  principle removals/redefinitions, MINOR for new principles
  or material expansions, PATCH for clarifications and
  wording fixes.
- All PRs and code reviews MUST verify compliance with
  these principles. Non-compliance MUST be flagged and
  resolved before merge.

**Version**: 1.0.0 | **Ratified**: 2026-02-06 | **Last Amended**: 2026-02-06
