# Research: Manage Citytrips

**Feature**: 002-manage-citytrips
**Date**: 2026-02-07

## R1: User Identity Strategy (No Auth Exists)

**Decision**: Introduce a hardcoded "current user" concept via a simple `ICurrentUserService` interface in Features, with an `InMemoryCurrentUserService` in Infrastructure that returns a fixed user ID.

**Rationale**: The existing codebase has no authentication. A real auth system (ASP.NET Identity, OAuth) is out of scope for this feature. However, "My Citytrips" requires knowing _who_ the current user is. A simple interface + hardcoded implementation satisfies the feature while keeping the door open for real auth later. The interface lives in Features (per Clean Architecture), the implementation in Infrastructure.

**Alternatives considered**:
- Full ASP.NET Identity setup — too heavy for current demo scope, violates YAGNI
- Pass userId as parameter everywhere — leaky, clutters all method signatures
- Skip user identity entirely — impossible, feature requires per-user filtering

## R2: Citytrip Domain Model Evolution

**Decision**: Extend the existing `Citytrip` record with new fields: `Destination` (rename from `CityName` for consistency with spec), `StartDate`, `EndDate`, `MaxParticipants`, and `CreatorId`. Keep the record as immutable with `with` expressions for updates.

**Rationale**: The spec requires title, destination, start date, end date, description, and max participants. The existing model has `CityName`, `ImageUrl`, `DurationInDays`, and `Description`. We need to evolve the model while maintaining backward compatibility with feature 001's browse page. Renaming `CityName` to align with the spec vocabulary, and replacing `DurationInDays` with explicit `StartDate`/`EndDate` is cleaner but requires updating seed data and the browse page references.

**Alternatives considered**:
- Keep CityName and add Destination separately — creates confusion, two fields for similar concept
- Keep DurationInDays alongside dates — redundant, violates YAGNI
- Create separate entity for "managed trips" — violates single entity principle, over-engineering

## R3: Repository CRUD Pattern

**Decision**: Extend `ICitytripRepository` with `Add`, `Update`, `Delete`, `GetByCreator`, and `GetEnlistedByUser` methods. The in-memory implementation uses a `ConcurrentDictionary` with auto-incrementing IDs.

**Rationale**: The existing repository has `GetAll()` and `GetById()`. CRUD operations are the minimal addition needed. Using the existing in-memory pattern keeps consistency. `GetByCreator` and `GetEnlistedByUser` are query methods that filter by user relationship.

**Alternatives considered**:
- Generic repository pattern — over-abstraction for current needs, violates Simplicity principle
- Separate read/write repositories — CQRS purism, but overkill for in-memory demo

## R4: Modal Dialog Implementation

**Decision**: Build a simple Blazor component (`TripFormModal.razor`) that renders as a CSS modal overlay. Use Blazor's built-in component model with `[Parameter]` for data binding and `EventCallback` for save/cancel actions. No JavaScript interop needed.

**Rationale**: The clarification specified modal/dialog overlay. Blazor Server can render modal overlays purely with CSS (position: fixed, z-index) and C# state management. The existing project uses no JS interop, so keeping it CSS-only aligns with the current approach.

**Alternatives considered**:
- Bootstrap modal with JS — adds JS interop dependency, breaks current no-JS pattern
- Blazor Dialog library (Radzen, MudBlazor) — new NuGet dependency, violates Technology Constraints principle
- Separate page instead of modal — contradicts spec clarification

## R5: Tab/Toggle Component Pattern

**Decision**: Use a simple two-state toggle implemented as styled buttons/tabs within the `MyCitytrips.razor` page component. Track active tab via a C# enum (`ActiveSection { MyTrips, EnlistedTrips }`). No separate component needed — inline in the page.

**Rationale**: Only two sections exist, making a generic tab component unnecessary (YAGNI). A simple boolean or enum state with conditional rendering (`@if`) is the simplest Blazor approach.

**Alternatives considered**:
- Reusable tab component — over-engineering for two tabs
- URL-based routing (e.g., `/my-citytrips/created` vs `/my-citytrips/enlisted`) — adds routing complexity, spec describes single page with toggle
- Query string parameter — unnecessary state management complexity
