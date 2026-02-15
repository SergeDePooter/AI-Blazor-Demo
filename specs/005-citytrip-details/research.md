# Research: Citytrip Detail View

**Feature**: 005-citytrip-details
**Date**: 2026-02-14
**Purpose**: Document technical decisions and best practices for implementing the citytrip detail view

## Overview

This research document consolidates findings on Blazor routing, data modeling, component composition, and in-memory data strategies needed to implement a read-only citytrip detail view with itinerary information.

## Research Areas

### 1. Blazor Server Routing for Detail Pages

**Decision**: Use parameterized routes with `@page` directive

**Rationale**:
- Blazor Server supports route parameters directly in the `@page` directive
- Pattern: `@page "/citytrips/{id:int}"` provides type-safe routing
- Route parameters are automatically bound to component properties via `[Parameter]` attribute
- Supports deep linking and browser navigation (back button works automatically)

**Implementation Pattern**:
```csharp
@page "/citytrips/{Id:int}"

@code {
    [Parameter]
    public int Id { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        // Fetch data when Id changes
    }
}
```

**Alternatives Considered**:
- Query string parameters (`/citytrips?id=1`) - Rejected: Less semantic, worse SEO
- State management (NavigationManager with state) - Rejected: Doesn't support deep linking

**References**:
- Official Blazor routing documentation (.NET 10)
- Route constraints for type safety (`:int`, `:guid`, etc.)

---

### 2. Data Modeling for Itineraries

**Decision**: Use nested record types for immutable domain models

**Rationale**:
- C# records provide value-based equality and immutability by default
- Fits read-only view requirements (no mutations needed)
- Clean syntax for nested structures (DayPlan → List<Attraction>)
- Records work seamlessly with MediatR and serialization

**Data Model Structure**:
```csharp
// Domain models (Features/Citytrips/Domain/)
public record DayPlan(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<Attraction> Attractions);

public record Attraction(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);
```

**Repository Extension**:
- Extend `ICitytripRepository` with: `Task<Citytrip?> GetByIdWithItineraryAsync(int id)`
- Return `Citytrip` with populated `DayPlans` collection property
- In-memory implementation stores itinerary as part of seed data

**Alternatives Considered**:
- Classes with setters - Rejected: Mutability unnecessary for read-only view
- Separate itinerary repository - Rejected: Violates YAGNI, single aggregate root suffices
- Value objects with validation - Deferred: Can add if business rules emerge

**References**:
- C# 10 record types documentation
- DDD aggregate root patterns
- Entity Framework Core owned entities (for future persistence)

---

### 3. Blazor Component Composition

**Decision**: Hierarchical component structure with props drilling

**Rationale**:
- Simple parent-child communication via `[Parameter]` attributes
- No state management needed (data flows down from page component)
- Each component has single responsibility (day plan, attraction, transportation)
- Supports scoped CSS for isolated styling

**Component Hierarchy**:
```
CitytripDetail.razor (page)
├── Parameters: int Id
├── Fetches: CitytripDetailResponse via MediatR
└── Renders:
    ├── Basic trip info (header)
    └── DayPlanCard.razor (foreach day)
        ├── Parameters: DayPlanDto
        └── Renders:
            ├── Day header (date, timeframe)
            └── AttractionItem.razor (foreach attraction)
                ├── Parameters: AttractionDto
                └── Renders:
                    ├── Attraction details
                    └── TransportationInfo.razor (if has transport)
                        └── Parameters: List<string> options
```

**Component Communication**:
- Page component calls MediatR query in `OnParametersSetAsync`
- DTOs passed down as component parameters
- No events needed (read-only, no user interactions beyond navigation)

**Alternatives Considered**:
- Cascading parameters - Rejected: Overkill for simple prop drilling
- State container (Fluxor/Redux) - Rejected: No shared state between pages
- Monolithic page component - Rejected: Poor reusability and testability

**References**:
- Blazor component lifecycle documentation
- Component parameter binding best practices
- Scoped CSS (`.razor.css` files)

---

### 4. In-Memory Data Seeding Strategy

**Decision**: Extend `InMemoryCitytripRepository` with itinerary seed data

**Rationale**:
- Consistent with existing pattern (all features use same repository)
- Seed data defined in repository constructor/initialization
- Easy to add sample itineraries for testing and demos
- No database schema changes needed

**Seed Data Pattern**:
```csharp
private static readonly List<Citytrip> _citytrips = new()
{
    new Citytrip(
        Id: 1,
        Title: "Paris Adventure",
        // ... existing fields
        DayPlans: new List<DayPlan>
        {
            new(
                DayNumber: 1,
                Date: new DateOnly(2026, 6, 1),
                Timeframe: "Morning 9:00-12:00",
                Attractions: new List<Attraction>
                {
                    new("Eiffel Tower", "Iconic landmark",
                        "https://www.toureiffel.paris/en",
                        new[] { "Metro Line 6 to Bir-Hakeim" })
                })
        })
};
```

**Repository Method**:
- `GetByIdWithItineraryAsync(int id)` returns full object graph
- Existing `GetByIdAsync` can remain for lightweight queries
- Or: merge into single method if all callers need itinerary

**Alternatives Considered**:
- Separate itinerary repository - Rejected: Data is tightly coupled to Citytrip
- Lazy loading - Rejected: Not applicable for in-memory data
- External JSON file - Rejected: Adds complexity, seed data is simple enough inline

**References**:
- Repository pattern best practices
- In-memory data strategies for prototyping
- Test data builders (for unit tests)

---

## Summary of Decisions

| Area | Decision | Key Benefit |
|------|----------|-------------|
| Routing | Parameterized routes (`/citytrips/{id:int}`) | Type-safe, deep-linkable |
| Data Model | Nested records (DayPlan → Attractions) | Immutable, value-based equality |
| Components | Hierarchical composition with props | Simple, testable, scoped styling |
| Data Storage | Extend in-memory repository with itinerary | Consistent with existing patterns |

## Open Questions

None - all technical decisions resolved.

## Next Steps

Proceed to Phase 1:
1. Generate `data-model.md` with concrete C# record definitions
2. Generate `contracts/GetCitytripDetailContract.md` with MediatR query/response
3. Generate `quickstart.md` with TDD workflow for implementation
