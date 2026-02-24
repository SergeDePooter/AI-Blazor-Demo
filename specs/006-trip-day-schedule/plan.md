# Implementation Plan: Citytrip Detail Page with Day Schedule and Map

**Branch**: `006-trip-day-schedule` | **Date**: 2026-02-24 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/006-trip-day-schedule/spec.md`

## Summary

Extend the existing `CitytripDetail.razor` page with a generic day schedule and a sticky Google Maps sidebar. The schedule displays events (any type: museum, market, stadium, etc.) grouped by date with event-type icons. The map sidebar stays fixed alongside the scroll and shows only the markers for the day section currently in the viewport. The implementation extends the existing `DayPlan` domain model and `GetCitytripDetail` vertical slice — no new route or MediatR query is needed.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: Blazor Server (built-in), MediatR 14.0.0 (existing), IJSRuntime (built-in Blazor, for Google Maps interop)
**Storage**: In-memory (extending existing `InMemoryCitytripRepository` seed data with `ScheduledEvent` and `Place` data)
**Testing**: bUnit 2.5.3, xUnit, NSubstitute, FluentAssertions (all existing)
**Target Platform**: Web (Blazor Server, `@rendermode InteractiveServer`)
**Project Type**: Web application with Clean Architecture (Web → Features → Infrastructure)
**Performance Goals**: Detail page fully loaded within 2 seconds; map updates within 200ms of day-section scroll transition
**Constraints**: Read-only view; no event editing or geocoding; Google Maps API key required as configuration
**Scale/Scope**: Up to 30 events per day, up to 14 days per trip on a single scrollable page

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### ✅ Principle I: CQRS with Vertical Slices

- **Compliance**: No new query is added. The existing `GetCitytripDetail` slice is extended: `ScheduledEvent` and `Place` are added to `Citytrips/Domain/`; `DayPlan` gains an optional `Events` collection; `DayPlanDetail` response DTO is enriched with `List<ScheduledEventDetail>`. All changes remain within the `Citytrips/` slice boundary with no cross-slice dependencies.

### ✅ Principle II: Test-Driven Development

- **Compliance**: TDD strictly followed:
  1. Write failing domain tests for `ScheduledEvent` and `Place` validation
  2. Write failing handler tests asserting Events appear in `GetCitytripDetailResponse`
  3. Write failing bUnit tests for `ScheduledEventCard` and `TripMapSidebar`
  4. Implement minimum production code to pass each failing test before proceeding

### ✅ Principle III: Clean Architecture Layering

- **Web**: Updated `CitytripDetail.razor` (two-column layout), new `DaySchedulePanel`, `ScheduledEventCard`, `TripMapSidebar` components, `trip-map.js` in `wwwroot/js/`
- **Features**: `ScheduledEvent.cs`, `Place.cs` (domain), extended `DayPlan.cs`, extended `CitytripDetailResponse.cs`, extended `GetCitytripDetailHandler.cs`
- **Infrastructure**: Extended seed data in `InMemoryCitytripRepository.cs`
- No new repository interface methods needed; `GetByIdWithItineraryAsync` already returns the full object graph

### ✅ Principle IV: Simplicity & YAGNI

- Reuse existing `GetCitytripDetail` query — no new MediatR slice or route
- Use built-in `IJSRuntime` for Google Maps — no new NuGet package required
- Event type → icon mapping is a `static readonly Dictionary<string, string>` in the Web layer
- `ScheduledEvent` added as `List<ScheduledEvent>? Events` on `DayPlan` (optional, backward-compatible)
- No geocoding, real-time updates, or editing in scope

### Gate Assessment

**Status**: ✅ PASS — All constitutional principles satisfied. No violations requiring justification.

## Project Structure

### Documentation (this feature)

```text
specs/006-trip-day-schedule/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   └── GetCitytripDetailContract.md   # Extended MediatR contract
└── tasks.md             # Phase 2 output (/speckit.tasks — not created here)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Features/
│   └── Citytrips/
│       ├── Domain/
│       │   ├── Citytrip.cs                    # Existing — unchanged
│       │   ├── DayPlan.cs                     # Existing — add List<ScheduledEvent>? Events
│       │   ├── Attraction.cs                  # Existing — unchanged
│       │   ├── ScheduledEvent.cs              # NEW: generic timed event entity
│       │   ├── Place.cs                       # NEW: geographic point of interest
│       │   └── ICitytripRepository.cs         # Existing — unchanged
│       └── GetCitytripDetail/
│           ├── GetCitytripDetailQuery.cs      # Existing — unchanged
│           ├── GetCitytripDetailHandler.cs    # Existing — extend projection to map Events
│           └── CitytripDetailResponse.cs      # Existing — add ScheduledEventDetail, PlaceDetail
│
├── CitytripPlanner.Infrastructure/
│   └── Citytrips/
│       └── InMemoryCitytripRepository.cs     # Existing — add Events seed data (Paris trip)
│
└── CitytripPlanner.Web/
    └── Components/
        ├── Pages/
        │   ├── CitytripDetail.razor           # Existing — update to two-column layout
        │   └── CitytripDetail.razor.css       # Existing — update for sticky sidebar layout
        └── Citytrips/                         # NEW sub-component folder
            ├── DaySchedulePanel.razor         # NEW: renders day sections with date headers
            ├── DaySchedulePanel.razor.css
            ├── ScheduledEventCard.razor       # NEW: single event with icon + fields
            ├── ScheduledEventCard.razor.css
            ├── TripMapSidebar.razor           # NEW: sticky Google Maps sidebar
            └── TripMapSidebar.razor.css

    wwwroot/
    └── js/
        └── trip-map.js                        # NEW: Maps init + IntersectionObserver scroll-sync

CitytripPlanner.Tests/
└── Citytrips/
    ├── GetCitytripDetail/
    │   └── GetCitytripDetailTests.cs          # Existing — extend with Events assertions
    ├── Domain/
    │   ├── ScheduledEventTests.cs             # NEW: domain validation tests
    │   └── PlaceTests.cs                      # NEW: domain validation tests
    └── Web/
        └── Citytrips/
            ├── ScheduledEventCardTests.cs     # NEW: bUnit component tests
            └── TripMapSidebarTests.cs         # NEW: bUnit component tests
```

**Structure Decision**: Web application pattern. Follows existing three-project Clean Architecture. New Blazor sub-components placed in `Components/Citytrips/` to group citytrip-specific UI elements, consistent with the project's intent to scale per feature domain.

## Complexity Tracking

N/A — No constitutional violations. All principles satisfied.
