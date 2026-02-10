# Implementation Plan: Filter Citytrips

**Branch**: `003-filter-citytrips` | **Date**: 2026-02-07 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/003-filter-citytrips/spec.md`

## Summary

Add client-side filtering to the main citytrips browse page (`/` and `/citytrips`). Users can filter the displayed trip list by a text search term (matching title and destination) and by a date range (from/to date pickers). Filters use AND logic, text input is debounced (~300ms), and a "Clear all filters" button resets everything. All filtering happens in-memory on the already-loaded trip list — no new backend queries or CQRS slices are needed.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: Blazor Server (built-in), MediatR 14.x (existing)
**Storage**: N/A — filtering is client-side on existing in-memory data
**Testing**: xUnit 2.9.3, bUnit 2.5.3
**Target Platform**: Web (Blazor Server, InteractiveServer render mode)
**Project Type**: Web application (.NET solution with 3 projects + test project)
**Performance Goals**: Filtered results update within 200ms of filter change
**Constraints**: No new NuGet packages; no JavaScript interop; pure Blazor/C#
**Scale/Scope**: Filtering a small in-memory list (tens of items)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. CQRS with Vertical Slices | PASS | No new slices needed — filtering is purely presentational logic in the Blazor page. The existing `ListCitytripsQuery` continues to load data; filtering applies client-side. |
| II. Test-Driven Development | PASS | Filter logic will be extracted into a testable pure function/class. Tests written first for all filter scenarios. |
| III. Clean Architecture Layering | PASS | Filter logic lives in Features layer (pure function). UI binds to it in Web layer. No infrastructure changes. |
| IV. Simplicity & YAGNI | PASS | No abstractions beyond what's needed. A single filter model class and a pure filter function. No debounce library — use a simple `System.Threading.Timer` or `CancellationTokenSource` pattern. |
| Technology Constraints | PASS | No new NuGet packages. Uses built-in Blazor capabilities. |

All gates pass. No violations to justify.

## Project Structure

### Documentation (this feature)

```text
specs/003-filter-citytrips/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output (via /speckit.tasks)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Features/
│   └── Citytrips/
│       └── ListCitytrips/
│           ├── CitytripCard.cs          # (existing) DTO — no changes
│           ├── CitytripFilter.cs        # (NEW) Filter criteria model + pure filter function
│           ├── ListCitytripsQuery.cs    # (existing) — no changes
│           └── ListCitytripsHandler.cs  # (existing) — no changes
├── CitytripPlanner.Web/
│   └── Components/
│       ├── Pages/
│       │   ├── Citytrips.razor          # (MODIFIED) Add filter UI + debounce logic
│       │   └── Citytrips.razor.css      # (MODIFIED) Add filter area styles
│       └── Shared/
│           └── TripCard.razor           # (existing) — no changes
├── CitytripPlanner.Tests/
│   └── Citytrips/
│       └── ListCitytrips/
│           ├── CitytripFilterTests.cs   # (NEW) Unit tests for filter logic
│           └── ListCitytripsHandlerTests.cs # (existing) — no changes
└── CitytripPlanner.Infrastructure/      # No changes
```

**Structure Decision**: Filter logic is co-located with the existing `ListCitytrips` slice since it operates on the same `CitytripCard` DTO. This keeps the feature self-contained within one folder while maintaining the vertical slice pattern.

## Design Decisions

### D1: Filter Logic Placement

**Decision**: Extract filter logic into a pure static class `CitytripFilter` in the Features layer, co-located with the `ListCitytrips` slice.

**Rationale**: A pure function `Apply(List<CitytripCard> trips, CitytripFilterCriteria criteria)` is trivially testable without Blazor, follows Clean Architecture (logic in Features, not Web), and keeps the Blazor page thin.

### D2: Debounce Strategy

**Decision**: Use `CancellationTokenSource` pattern in the Blazor component for text input debounce. When the user types, cancel the previous pending filter and schedule a new one after 300ms.

**Rationale**: No external library needed. Built-in `CancellationTokenSource` + `Task.Delay` is the standard Blazor pattern for debouncing. Date picker changes apply filters immediately (no debounce needed).

### D3: Date Range Validation

**Decision**: When the user selects a "from" date, constrain the "to" date picker's `min` attribute. When the user selects a "to" date, constrain the "from" date picker's `max` attribute. Use HTML `<input type="date">` native constraints.

**Rationale**: Simplest approach — leverages native browser date input validation. No custom validation logic needed for the date relationship constraint.

### D4: No New CQRS Slice

**Decision**: Do NOT create a new query like `FilterCitytripsQuery`. Filtering is applied client-side to the existing `ListCitytripsQuery` results.

**Rationale**: The trip list is already loaded in memory. Creating a server-side filter query would be over-engineering (YAGNI). Client-side filtering on a small dataset is instant and avoids unnecessary round-trips.
