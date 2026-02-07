# Research: Browse Citytrips

**Feature Branch**: `001-browse-citytrips`
**Date**: 2026-02-06

## R1: Masonry Layout in Blazor

**Decision**: Use CSS-only masonry via CSS `columns` property.

**Rationale**: CSS columns provide a Pinterest-style masonry
effect without JavaScript dependencies. Native CSS masonry
(`masonry-auto-flow`) is still experimental and not widely
supported. CSS columns work in all modern browsers and are
trivial to implement in Blazor scoped CSS.

**Alternatives considered**:
- CSS Grid with `masonry` value — not yet supported in
  production browsers
- JavaScript masonry libraries (Masonry.js) — adds JS interop
  complexity, violates YAGNI principle
- Flexbox wrapping — does not produce true masonry column flow

## R2: CQRS Mediator Pattern

**Decision**: Use MediatR for command/query dispatching.

**Rationale**: MediatR is the de facto standard for CQRS in
.NET. It provides `IRequest<T>` / `IRequestHandler<T, R>`
abstractions that map directly to our vertical slice folder
structure. Each slice folder gets its own handler. The
pipeline behavior feature supports cross-cutting concerns
(validation, logging) without polluting handler code.

**Alternatives considered**:
- Hand-rolled mediator — more code to maintain, no ecosystem
  benefit. Violates YAGNI only if we never use pipeline
  behaviors, but validation alone justifies the dependency.
- Direct service injection — breaks CQRS pattern, couples
  Blazor pages to business logic

**Justification per constitution**: New NuGet package
(MediatR) justified by: (1) enables the mandated CQRS
pattern, (2) enforces vertical slice isolation, (3) widely
adopted with minimal footprint.

## R3: Data Storage for Initial Feature

**Decision**: Use in-memory seed data for citytrips. Like
and enlist state stored in-memory per session via scoped
service.

**Rationale**: The spec explicitly states persistence is
per-session only. Blazor Server circuits maintain a scoped
DI container per connection, making scoped services the
natural session store. No database is needed for this feature.

**Alternatives considered**:
- SQLite / EF Core — premature; spec says session-only
  persistence
- Browser localStorage via JS interop — adds complexity,
  not needed for server-side Blazor
- Static in-memory store (singleton) — would share state
  across users

## R4: Toast/Snackbar Notification

**Decision**: Implement a lightweight custom Blazor toast
component.

**Rationale**: A simple toast component requires ~30 lines
of Razor + CSS. No external library needed. The toast
renders at a fixed position, auto-dismisses after a
configurable duration using `Task.Delay` + `StateHasChanged`.

**Alternatives considered**:
- Blazored.Toast NuGet — external dependency for a single
  use case; violates YAGNI
- MudBlazor Snackbar — pulls in entire component library;
  massive overkill
- Browser alert() — poor UX, blocks interaction

## R5: Navigation Menu Without Sidebar

**Decision**: Replace the default Blazor template's sidebar
`NavMenu` with a custom horizontal top navigation bar. Remove
the sidebar layout entirely from `MainLayout.razor`.

**Rationale**: The spec requires (FR-002 through FR-005) a
horizontal text-only menu with specific orange styling and
explicitly forbids a left-side sidebar. The default Blazor
template uses a vertical sidebar which must be replaced.

**Alternatives considered**:
- Keep sidebar and add horizontal nav — contradicts FR-005
- Use Bootstrap navbar component — adds Bootstrap JS
  dependency for a simple text menu; orange styling is
  custom regardless

## R6: Blazor Render Mode

**Decision**: Use Blazor Interactive Server render mode.

**Rationale**: The project is already configured for Blazor
Server (`Microsoft.NET.Sdk.Web`, `AddInteractiveServerComponents`
in Program.cs). Like/enlist interactions require server-side
state (scoped services), making Server mode the natural fit.
No WASM compilation overhead needed.

**Alternatives considered**:
- Blazor WASM — requires separate hosting, API layer, and
  client-side state management; overkill for session-scoped
  state
- Static SSR — cannot handle interactive like/enlist buttons
