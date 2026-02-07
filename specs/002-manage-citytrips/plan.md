# Implementation Plan: Manage Citytrips

**Branch**: `002-manage-citytrips` | **Date**: 2026-02-07 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-manage-citytrips/spec.md`

## Summary

Add a "My Citytrips" page where authenticated users can manage their own citytrips (create, edit, delete) and view citytrips they've enlisted in. The page uses a tab-based layout to toggle between "My Citytrips" and "Enlisted Citytrips" sections. Create/edit operations use a modal dialog overlay. This feature requires introducing a basic user identity system since the current codebase has no authentication.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: MediatR 14.0.0, Blazor Server (built-in), bUnit 2.5.3 (testing)
**Storage**: In-memory (extending existing InMemoryCitytripRepository pattern)
**Testing**: xUnit 2.9.3 + bUnit 2.5.3
**Target Platform**: Web (Blazor Server)
**Project Type**: Web application (Blazor Server monolith)
**Performance Goals**: Page load < 2s, section toggle < 1s, form submit < 1s
**Constraints**: No database yet (in-memory only), no real auth (simulated user identity)
**Scale/Scope**: Demo application, single user session

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. CQRS with Vertical Slices | PASS | New slices: CreateTrip, UpdateTrip, DeleteTrip, GetMyTrips, GetEnlistedTrips — each self-contained |
| II. Test-Driven Development | PASS | TDD workflow will be enforced in tasks. Handler tests + component tests planned |
| III. Clean Architecture Layering | PASS | Features project holds commands/queries/handlers, Infrastructure holds repository, Web holds Blazor components |
| IV. Simplicity & YAGNI | PASS | Using existing in-memory patterns, minimal user identity (hardcoded user for demo), no over-engineering |
| Technology Constraints | PASS | .NET 10, Blazor Server, MediatR — all existing. No new NuGet packages needed |
| Development Workflow | PASS | Feature branch already created from main |

No violations. Gate passes.

## Project Structure

### Documentation (this feature)

```text
specs/002-manage-citytrips/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── api-contracts.md
└── tasks.md             # Phase 2 output (via /speckit.tasks)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Features/
│   └── Citytrips/
│       ├── Domain/
│       │   ├── Citytrip.cs                  # MODIFY: Add new fields (StartDate, EndDate, MaxParticipants, CreatorId)
│       │   ├── ICitytripRepository.cs       # MODIFY: Add CRUD methods + query by creator/enlisted
│       │   └── UserTripInteraction.cs       # EXISTING (unchanged)
│       ├── CreateTrip/
│       │   ├── CreateTripCommand.cs          # NEW
│       │   ├── CreateTripHandler.cs          # NEW
│       │   └── CreateTripValidator.cs        # NEW
│       ├── UpdateTrip/
│       │   ├── UpdateTripCommand.cs          # NEW
│       │   ├── UpdateTripHandler.cs          # NEW
│       │   └── UpdateTripValidator.cs        # NEW
│       ├── DeleteTrip/
│       │   ├── DeleteTripCommand.cs          # NEW
│       │   └── DeleteTripHandler.cs          # NEW
│       ├── GetMyTrips/
│       │   ├── GetMyTripsQuery.cs            # NEW
│       │   ├── GetMyTripsHandler.cs          # NEW
│       │   └── MyTripItem.cs                 # NEW (response DTO)
│       └── GetEnlistedTrips/
│           ├── GetEnlistedTripsQuery.cs      # NEW
│           ├── GetEnlistedTripsHandler.cs    # NEW
│           └── EnlistedTripItem.cs           # NEW (response DTO)
│
├── CitytripPlanner.Infrastructure/
│   └── Citytrips/
│       └── InMemoryCitytripRepository.cs    # MODIFY: Implement new CRUD + query methods
│
├── CitytripPlanner.Web/
│   └── Components/
│       ├── Pages/
│       │   ├── MyCitytrips.razor            # NEW: My Citytrips page with tab layout
│       │   └── MyCitytrips.razor.css        # NEW: Scoped styles
│       ├── Shared/
│       │   ├── TripFormModal.razor           # NEW: Create/Edit modal dialog
│       │   ├── TripFormModal.razor.css       # NEW: Modal styles
│       │   ├── DeleteConfirmModal.razor      # NEW: Delete confirmation dialog
│       │   └── DeleteConfirmModal.razor.css  # NEW: Confirmation styles
│       └── Layout/
│           └── NavMenu.razor                # MODIFY: Add nav link to My Citytrips
│
└── CitytripPlanner.Tests/
    └── Citytrips/
        ├── CreateTrip/
        │   └── CreateTripHandlerTests.cs     # NEW
        ├── UpdateTrip/
        │   └── UpdateTripHandlerTests.cs     # NEW
        ├── DeleteTrip/
        │   └── DeleteTripHandlerTests.cs     # NEW
        ├── GetMyTrips/
        │   └── GetMyTripsHandlerTests.cs     # NEW
        └── GetEnlistedTrips/
            └── GetEnlistedTripsHandlerTests.cs # NEW
```

**Structure Decision**: Extends the existing Clean Architecture with vertical slices. Each CRUD operation gets its own slice folder under `Citytrips/`. The Blazor page and shared modal components follow existing patterns from feature 001.

## Complexity Tracking

No constitution violations to justify.
