# Research: Edit Citytrip via Wizard

## Decision 1: Wizard Page Strategy — New Page vs. Mode Flag

**Decision**: Create a new `EditCitytrip.razor` page at `/citytrips/edit/{Id:int}` rather than adding an `IsEditMode` flag to `CreateCitytrip.razor`.

**Rationale**: The two flows share the same step components but differ significantly in page-level orchestration — load-on-init vs. empty state, `UpdateTripCommand` vs. `CreateTripCommand`, ownership check, and day-number preservation logic on date change. Merging both modes into one file would make `CreateCitytrip.razor` harder to reason about and test. A separate page keeps each route's lifecycle clean, follows the YAGNI principle (no shared branching logic for hypothetical reuse), and allows independent deployment and testing per constitution Principle IV.

**Alternatives considered**:
- Route parameter on CreateCitytrip (`?tripId=N`) — rejected because it complicates `@code` with conditionals throughout, muddying the currently simple create flow.
- Shared base class — rejected as premature abstraction (constitution Principle IV: three similar lines preferable to premature abstraction).

---

## Decision 2: WizardStep1 Pre-fill Strategy — InitialModel Parameter

**Decision**: Add an `[Parameter] public WizardStep1Model? InitialModel { get; set; }` to `WizardStep1.razor` and copy its field values into the internal `_model` in `OnInitialized`.

**Rationale**: `WizardStep1` owns its `_model` as a mutable class instance (for `@bind` to work). Passing a reference directly would mean the edit page and step share the same object — mutations in the form would change the pre-loaded data stored in the page, breaking Back-navigation. Copying field-by-field in `OnInitialized` gives the component independent ownership while still pre-populating every field. This approach is backward-compatible: the parameter is optional, so `CreateCitytrip.razor` requires no change.

**Alternatives considered**:
- Replace `_model` entirely with the passed reference — rejected because form mutations would pollute the parent's copy.
- Use `OnParametersSet` — `OnInitialized` is sufficient since the component is never re-parameterized mid-session; `OnParametersSet` would risk overwriting edits if parent triggers a re-render.

---

## Decision 3: UpdateTripCommand Extension — Add ImageUrl + DayPlans

**Decision**: Extend `UpdateTripCommand` with `string? ImageUrl = null` and `List<DayPlanInput>? DayPlans = null`, mirroring the extension applied to `CreateTripCommand` in feature 007.

**Rationale**: The same `DayPlanInput`, `ScheduledEventInput`, and `PlaceInput` records already exist in `CitytripPlanner.Features/Citytrips/CreateTrip/`. Reusing them avoids duplication (constitution Principle IV). The `UpdateTripHandler` will fully replace the trip's `DayPlans` list with the incoming value — a simple, safe approach for an in-memory store. Partial-update patching is not necessary at this scale.

**Alternatives considered**:
- Separate `UpdateDayPlansCommand` — rejected as over-engineering for a single-page wizard that always submits all data together.
- Reuse `DayPlanInput` from the CreateTrip namespace directly — accepted, no need to duplicate the records.

---

## Decision 4: Date-Change Event Preservation — By Day Number

**Decision**: When the owner changes the trip's date range, events are re-mapped by day number. Specifically: after generating the new day slots, for each new slot with `DayNumber ≤ old trip length`, copy the existing events from the matching old day number. New slots beyond the old trip length start empty. Old day plans with a day number beyond the new trip length are discarded.

**Rationale**: Clarified in spec Session 2026-02-25 (Q1 → C). Day number is the user-understood identity of a slot ("Day 1", "Day 2"), not the calendar date. Preserving by day number meets the user's mental model — if they add a day at the end, everything they entered for days 1–N stays on those same days.

**Alternatives considered**:
- Clear all events on date change — simpler but destructive; rejected per clarification.
- Preserve by calendar date — complex matching logic; rejected per clarification.

---

## Decision 5: MyCitytrips Edit Entry Point — Navigate to Wizard Page

**Decision**: Update the Edit button in `MyCitytrips.razor` to `Navigation.NavigateTo($"/citytrips/edit/{trip.Id}")` instead of opening the `TripFormModal`.

**Rationale**: The spec requires the edit wizard to be the single edit entry point (FR-001, FR-002). The existing `TripFormModal` covers only basic fields and will become redundant for the primary edit flow. The modal remains in the codebase for now but is no longer wired to the Edit button.

**Alternatives considered**:
- Keep `TripFormModal` for quick edits and add the wizard for full edits — rejected as out of scope; the spec says the wizard is the edit experience.
