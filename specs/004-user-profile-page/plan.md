# Implementation Plan: User Profile Management

**Branch**: `004-user-profile-page` | **Date**: 2026-02-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/004-user-profile-page/spec.md`

## Summary

Create a user profile management page where authenticated users can view and edit their personal information (name, firstname, gender, country). The feature follows the existing CQRS vertical slice architecture and uses in-memory storage consistent with the current CitytripPlanner patterns.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: MediatR 14.0.0, Blazor Server (built-in)
**Storage**: In-memory (new InMemoryUserProfileRepository following existing patterns)
**Testing**: xUnit + bUnit 2.5.3 (Blazor component testing)
**Target Platform**: Blazor Server web application
**Project Type**: Web application (Clean Architecture: Web, Features, Infrastructure)
**Performance Goals**: Page load < 2s, save operation < 3s (per spec SC-001, SC-002)
**Constraints**: Must follow existing authentication model (ICurrentUserService), in-memory data only
**Scale/Scope**: Single-user profile management (no admin features), 4 profile fields

**Clarifications Needed**:
- [NEEDS CLARIFICATION] Should gender options be defined as constants, enum, or configuration?
- [NEEDS CLARIFICATION] Should country list be a static C# list, JSON file, or ISO library?
- [NEEDS CLARIFICATION] Should UserProfile be a new entity or extend existing User concept?
- [NEEDS CLARIFICATION] Where should shared ICurrentUserService live (currently in Citytrips domain)?

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Principle I: CQRS with Vertical Slices ✅

**Compliance**: Feature will be organized as `CitytripPlanner.Features/UserProfiles/` with vertical slices:
- `GetUserProfile/` (Query) - retrieve profile data
- `UpdateUserProfile/` (Command) - update profile data

Each slice contains: Query/Command, Handler, Validator (if needed), and DTOs.

**No violations**.

### Principle II: Test-Driven Development ✅

**Compliance**: Implementation will follow TDD with Red-Green-Refactor cycle:
1. Write failing tests for handlers first
2. Implement minimum code to pass
3. Refactor
4. Add Blazor component tests using bUnit

**No violations**.

### Principle III: Clean Architecture Layering ✅

**Compliance**:
- Features project: Domain model (UserProfile), handlers, commands/queries
- Infrastructure project: InMemoryUserProfileRepository implementation
- Web project: Profile.razor Blazor component
- Dependency direction: Web → Features ← Infrastructure (via interfaces)

**No violations**.

### Principle IV: Simplicity & YAGNI ✅

**Compliance**:
- Start with simple in-memory storage (no database)
- Single profile entity with 4 fields (no complex relationships)
- Basic validation only (length, required fields)
- No premature abstractions

**No violations**.

### Technology Constraints ✅

**Compliance**:
- .NET 10 ✅
- Blazor Server ✅
- Nullable reference types enabled ✅
- MediatR (existing dependency) ✅
- No new NuGet packages required ✅

**No violations**.

### Development Workflow ✅

**Compliance**:
- Feature branch created from main: `004-user-profile-page` ✅
- Atomic commits ✅
- Single vertical slice (user profile management) ✅

**No violations**.

**GATE RESULT**: ✅ PASS - No constitution violations. Proceed to Phase 0.

## Project Structure

### Documentation (this feature)

```text
specs/004-user-profile-page/
├── spec.md              # Feature specification
├── plan.md              # This file
├── research.md          # Phase 0 output (resolves NEEDS CLARIFICATION)
├── data-model.md        # Phase 1 output (UserProfile entity)
├── quickstart.md        # Phase 1 output (developer guide)
├── contracts/           # Phase 1 output (API contracts if needed)
├── checklists/
│   └── requirements.md  # Spec quality checklist
└── tasks.md             # Phase 2 output (created by /speckit.tasks)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Features/
│   ├── Citytrips/              # Existing feature
│   └── UserProfiles/           # NEW: User profile feature domain
│       ├── Domain/
│       │   ├── UserProfile.cs           # Profile entity
│       │   ├── IUserProfileRepository.cs # Repository interface
│       │   └── GenderOptions.cs         # Gender constants/enum
│       ├── GetUserProfile/      # Query slice
│       │   ├── GetUserProfileQuery.cs
│       │   ├── GetUserProfileHandler.cs
│       │   └── UserProfileResponse.cs
│       └── UpdateUserProfile/   # Command slice
│           ├── UpdateUserProfileCommand.cs
│           ├── UpdateUserProfileHandler.cs
│           └── UpdateUserProfileValidator.cs
│
├── CitytripPlanner.Infrastructure/
│   ├── Citytrips/              # Existing infrastructure
│   └── UserProfiles/           # NEW: User profile infrastructure
│       └── InMemoryUserProfileRepository.cs
│
├── CitytripPlanner.Web/
│   └── Components/
│       └── Pages/
│           └── Profile.razor    # NEW: Profile management page
│           └── Profile.razor.css # Scoped styles
│
└── CitytripPlanner.Tests/
    ├── Citytrips/              # Existing tests
    └── UserProfiles/           # NEW: User profile tests
        ├── GetUserProfileHandlerTests.cs
        ├── UpdateUserProfileHandlerTests.cs
        └── ProfilePageTests.cs   # bUnit component test
```

**Structure Decision**: Following existing Clean Architecture with 3 projects. New "UserProfiles" feature domain parallels the existing "Citytrips" domain, maintaining vertical slice organization per constitution Principle I.

## Complexity Tracking

> No constitution violations - this section is empty.

---

## Phase 0: Research & Technical Decisions

**Status**: Pending - see [research.md](./research.md)

Research tasks to resolve NEEDS CLARIFICATION items:

1. **Gender Options Implementation**
   - Research: Best practice for representing limited-choice fields in C#
   - Options: Constants class, enum, configuration file
   - Decision criteria: Type safety, extensibility, testability

2. **Country List Source**
   - Research: Standard approach for country selection in .NET
   - Options: Static list, ISO 3166 library, embedded JSON resource
   - Decision criteria: Simplicity (YAGNI), maintainability, data accuracy

3. **UserProfile Entity Design**
   - Research: How to model user profile separate from authentication
   - Options: New UserProfile entity, extend User, use ICurrentUserService
   - Decision criteria: Separation of concerns, existing patterns

4. **Shared Service Location**
   - Research: Where should ICurrentUserService live if shared across features?
   - Options: Keep in Citytrips, move to shared Domain folder, create Common project
   - Decision criteria: Constitution compliance (avoid 4th project), discoverability

**Output**: All decisions documented in research.md with rationale and alternatives considered.

---

## Phase 1: Design & Contracts

**Status**: Pending - awaits Phase 0 completion

### Data Model (data-model.md)

Define:
- UserProfile entity (fields, validation rules, relationships to UserId)
- Repository interface
- DTOs (UserProfileResponse, UpdateUserProfileCommand)

### Contracts (contracts/)

Since this is Blazor Server (not REST API), contracts will be:
- MediatR command/query signatures (documented, not OpenAPI)
- Component parameter contracts (Profile.razor inputs/outputs)

### Quickstart Guide (quickstart.md)

Developer onboarding:
- How to run the profile page locally
- How to test profile updates
- How to extend with new fields (for future features)

### Agent Context Update

Run `update-agent-context.ps1` to add:
- Storage: In-memory UserProfile repository
- New vertical slice pattern usage

---

## Phase 2: Task Generation

**Status**: Not started (use `/speckit.tasks` command after Phase 1)

Task ordering will follow TDD principle:
1. Tests for GetUserProfile query
2. Implementation of GetUserProfile
3. Tests for UpdateUserProfile command
4. Implementation of UpdateUserProfile
5. Blazor component tests
6. Blazor component implementation

---

## Notes

- This plan assumes the existing `ICurrentUserService` provides the authenticated user's UserId
- Profile data is scoped per-user (UserId is the key)
- No authentication/authorization changes needed (already handled by existing system per spec assumptions)
- Country list and gender options decisions in Phase 0 will inform entity design in Phase 1
