# Implementation Plan: Citytrip Detail View

**Branch**: `005-citytrip-details` | **Date**: 2026-02-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/005-citytrip-details/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Implement a read-only detail view for citytrips that displays comprehensive information including basic trip details (city, dates, budget, photos) and a day-by-day itinerary with timeframes, attractions to visit (with optional website links), and transportation options. Users navigate to this view by clicking a citytrip card in the main list.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: Blazor Server (built-in), MediatR 14.x (existing)
**Storage**: In-memory (extending existing InMemoryCitytripRepository pattern)
**Testing**: bUnit 2.5.3, xUnit (existing test infrastructure)
**Target Platform**: Web (Blazor Server)
**Project Type**: Web application with Clean Architecture (Web → Features → Infrastructure)
**Performance Goals**: Page navigation and render <1 second, support itineraries up to 30 days without degradation
**Constraints**: Read-only view, no editing functionality
**Scale/Scope**: Single detail page with nested components for day plans and attractions

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### ✅ Principle I: CQRS with Vertical Slices
- **Compliance**: Feature will be implemented as a new vertical slice `GetCitytripDetail` under `CitytripPlanner.Features/Citytrips/GetCitytripDetail/`
- **Structure**: Will contain `GetCitytripDetailQuery.cs`, `GetCitytripDetailHandler.cs`, and response DTOs
- **Dependencies**: No cross-slice dependencies; will use shared domain models from `Citytrips/Domain/`

### ✅ Principle II: Test-Driven Development
- **Compliance**: TDD workflow will be strictly followed:
  1. Write failing tests for query handler
  2. Implement minimum code to pass
  3. Refactor
- **Test Coverage**: Unit tests for handler logic, integration tests for Blazor component navigation

### ✅ Principle III: Clean Architecture Layering
- **Compliance**: Strict layering maintained:
  - **Web**: Blazor page/component for detail view (e.g., `Pages/CitytripDetail.razor`)
  - **Features**: Query handler, domain models (DayPlan, Attraction), response DTOs
  - **Infrastructure**: In-memory repository implementation for extended Citytrip data
- **Interfaces**: ICitytripRepository (existing) will be extended with new method for fetching detail data

### ✅ Principle IV: Simplicity & YAGNI
- **Compliance**:
  - Start with simplest in-memory data structure for itinerary
  - No abstractions beyond required layer boundaries (ICitytripRepository interface)
  - Read-only view (no update/delete operations)
  - Simple text-based transportation info (no complex routing integration)

### Gate Assessment
**Status**: ✅ PASS - All constitutional principles satisfied. No violations requiring justification.

## Project Structure

### Documentation (this feature)

```text
specs/005-citytrip-details/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
│   └── GetCitytripDetailContract.md
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Web/
│   ├── Pages/
│   │   └── CitytripDetail.razor         # New: detail page component
│   │   └── CitytripDetail.razor.cs      # New: code-behind
│   │   └── CitytripDetail.razor.css     # New: scoped styles
│   └── Components/
│       └── Citytrips/
│           ├── DayPlanCard.razor        # New: day plan display component
│           ├── AttractionItem.razor     # New: attraction display component
│           └── TransportationInfo.razor # New: transportation display component
│
├── CitytripPlanner.Features/
│   └── Citytrips/
│       ├── Domain/
│       │   ├── Citytrip.cs             # Existing: may need extension
│       │   ├── DayPlan.cs              # New: domain model
│       │   ├── Attraction.cs           # New: domain model
│       │   └── ICitytripRepository.cs  # Existing: extend with GetByIdWithDetails
│       └── GetCitytripDetail/
│           ├── GetCitytripDetailQuery.cs    # New: MediatR query
│           ├── GetCitytripDetailHandler.cs  # New: query handler
│           └── CitytripDetailResponse.cs    # New: response DTO
│
├── CitytripPlanner.Infrastructure/
│   └── Persistence/
│       └── InMemoryCitytripRepository.cs  # Existing: extend with itinerary seed data
│
└── CitytripPlanner.Tests/
    ├── Features/
    │   └── Citytrips/
    │       └── GetCitytripDetailTests.cs  # New: handler unit tests
    └── Web/
        └── Pages/
            └── CitytripDetailPageTests.cs # New: Blazor component tests
```

**Structure Decision**: Using Option 2 (Web application) pattern. The CitytripPlanner solution follows Clean Architecture with three core projects (Web, Features, Infrastructure) plus Tests. This feature adds a new vertical slice under `Features/Citytrips/` and corresponding UI components in `Web/`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

N/A - No constitutional violations. All principles are satisfied.
