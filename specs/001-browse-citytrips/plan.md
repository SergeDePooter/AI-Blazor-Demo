# Implementation Plan: Browse Citytrips

**Branch**: `001-browse-citytrips` | **Date**: 2026-02-06 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-browse-citytrips/spec.md`

## Summary

Build a Blazor Server application where users can browse
citytrips displayed in a masonry grid layout. Each card shows
an image, city name, duration in days, a like toggle, and an
enlist button with toast confirmation. The layout replaces
the default Blazor sidebar with a horizontal text-only
navigation menu using orange styling with opacity states.
CQRS vertical slices handle all business logic via MediatR.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: Blazor Server (built-in), MediatR
**Storage**: In-memory (scoped services for session state,
singleton seed data)
**Testing**: xUnit + bUnit (Blazor component testing)
**Target Platform**: Web (desktop/tablet, 768px+ viewports)
**Project Type**: Web application (existing solution)
**Performance Goals**: Page load < 2s, interactions < 300ms
**Constraints**: Session-only persistence, no database
**Scale/Scope**: Single-user session state, ~6-10 seed trips

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after
Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. CQRS with Vertical Slices | PASS | Three slices: ListCitytrips (query), ToggleLike (command), Enlist (command). Each in own folder under Features/Citytrips/. |
| II. Test-Driven Development | PASS | TDD enforced: tests written before implementation in every phase. xUnit + bUnit for handlers and components. |
| III. Clean Architecture Layering | PASS | Web → Features ← Infrastructure. Interfaces in Features (ICitytripRepository, IUserInteractionStore), implementations in Infrastructure. |
| IV. Simplicity & YAGNI | PASS | In-memory storage (no DB), CSS-only masonry (no JS library), custom toast (no component library). MediatR justified for CQRS enforcement. |

**NuGet packages requiring justification**:
- **MediatR**: Enables mandated CQRS pattern, enforces
  vertical slice isolation, pipeline behaviors for validation
- **bUnit**: Required for Blazor component TDD (constitution
  mandates test-first)

## Project Structure

### Documentation (this feature)

```text
specs/001-browse-citytrips/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   └── cqrs-contracts.md # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
CitytripPlanner/
├── CitytripPlanner.Web/
│   ├── Components/
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor          # Horizontal layout (no sidebar)
│   │   │   ├── MainLayout.razor.css      # Layout styles
│   │   │   ├── NavMenu.razor             # Horizontal text-only nav
│   │   │   └── NavMenu.razor.css         # Orange styling, opacity states
│   │   ├── Pages/
│   │   │   └── Citytrips.razor           # Main citytrips browse page
│   │   └── Shared/
│   │       ├── CitytripCard.razor         # Individual trip card component
│   │       ├── CitytripCard.razor.css     # Card masonry styles
│   │       ├── Toast.razor               # Toast notification component
│   │       └── Toast.razor.css           # Toast styles
│   ├── Program.cs                        # DI registration
│   └── wwwroot/
│       └── app.css                       # Global styles (orange vars)
├── CitytripPlanner.Features/
│   └── Citytrips/
│       ├── Domain/
│       │   ├── Citytrip.cs               # Entity record
│       │   ├── UserTripInteraction.cs     # Session interaction
│       │   ├── ICitytripRepository.cs     # Repository interface
│       │   └── IUserInteractionStore.cs   # Interaction store interface
│       ├── ListCitytrips/
│       │   ├── ListCitytripsQuery.cs      # Query request
│       │   ├── ListCitytripsHandler.cs    # Query handler
│       │   └── CitytripCard.cs            # Response DTO
│       ├── ToggleLike/
│       │   ├── ToggleLikeCommand.cs       # Command request
│       │   └── ToggleLikeHandler.cs       # Command handler
│       └── Enlist/
│           ├── EnlistCommand.cs           # Command request
│           └── EnlistHandler.cs           # Command handler
├── CitytripPlanner.Infrastructure/
│   └── Citytrips/
│       ├── InMemoryCitytripRepository.cs  # Singleton seed data
│       └── InMemoryUserInteractionStore.cs # Scoped session store
└── CitytripPlanner.Tests/                 # NEW test project
    └── Citytrips/
        ├── ListCitytrips/
        │   └── ListCitytripsHandlerTests.cs
        ├── ToggleLike/
        │   └── ToggleLikeHandlerTests.cs
        ├── Enlist/
        │   └── EnlistHandlerTests.cs
        └── Components/
            ├── CitytripCardTests.cs
            └── NavMenuTests.cs
```

**Structure Decision**: Web application using the existing
three-project solution. Test project added as a fourth
project (justified: constitution mandates TDD, no simpler
alternative). Source follows vertical slice folders mirroring
the CQRS contracts.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| 4th project (Tests) | Constitution mandates TDD; tests cannot live in production assemblies | No simpler alternative exists for .NET test isolation |
| MediatR NuGet | Constitution mandates CQRS pattern | Hand-rolled mediator would require more code with no benefit |
