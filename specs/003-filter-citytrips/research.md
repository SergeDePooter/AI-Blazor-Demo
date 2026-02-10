# Research: Filter Citytrips

## Decision 1: Client-Side vs Server-Side Filtering

**Decision**: Client-side filtering on the already-loaded trip list.

**Rationale**: The `ListCitytripsQuery` already loads all trips into memory. The dataset is small (tens of items). Client-side filtering avoids server round-trips, provides instant feedback, and requires no new CQRS slices.

**Alternatives considered**:
- Server-side `FilterCitytripsQuery` with repository-level filtering — rejected as over-engineering for a small dataset (YAGNI).
- Hybrid approach (client-side for text, server-side for dates) — rejected as unnecessary complexity.

## Decision 2: Debounce Implementation

**Decision**: Use `CancellationTokenSource` + `Task.Delay(300)` pattern in the Blazor component.

**Rationale**: This is the standard Blazor Server debounce pattern. No external packages needed. The text input binds via `@oninput` event, cancels any pending delay, and starts a new 300ms delay before applying the filter.

**Alternatives considered**:
- External debounce library (e.g., Reactive Extensions) — rejected per constitution (no unnecessary dependencies).
- JavaScript interop for debounce — rejected per constraint (no JS interop).
- No debounce (filter on every keystroke) — rejected per clarification (user chose debounced).

## Decision 3: Filter Logic Architecture

**Decision**: Pure static method `CitytripFilter.Apply()` in the Features layer, co-located with `ListCitytrips` slice.

**Rationale**: A pure function operating on `List<CitytripCard>` is trivially testable with xUnit (no bUnit needed). Placing it in Features keeps presentation logic out of the Web layer while staying within the vertical slice boundary.

**Alternatives considered**:
- Filter logic inline in the Blazor component — rejected because it's harder to unit test and mixes concerns.
- Separate `FilterCitytrips` vertical slice folder — rejected as the filter operates on the same DTO and is not a standalone CQRS operation.

## Decision 4: Date Validation UX

**Decision**: Use HTML native `min`/`max` attributes on date inputs to prevent invalid ranges.

**Rationale**: Simplest approach. When "from" is set, the "to" input gets `min={from}`. When "to" is set, the "from" input gets `max={to}`. Native browser validation handles the constraint without custom code.

**Alternatives considered**:
- Custom validation with error messages — rejected as over-engineering for two related date pickers.
- Ignore invalid ranges and show empty results — rejected as poor UX.
