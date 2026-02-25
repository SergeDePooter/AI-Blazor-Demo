# Research: Create Citytrip with Full Fields and Map-Based Location Picker

**Branch**: `007-create-citytrip-map` | **Date**: 2026-02-24

---

## Decision 1: Multi-Step Wizard Approach in Blazor Server

**Decision**: Implement wizard as a dedicated full-page route (`/citytrips/create`) with a single parent `CreateCitytrip.razor` page that holds an integer `_step` state variable (1, 2, 3) and conditionally renders step child components. Wizard state (form inputs) lives entirely in the parent component's `@code` block.

**Rationale**: Blazor Server has no built-in router step concept, but a simple `_step` integer with `@if (_step == N)` rendering is the idiomatic approach used throughout the project. The parent component owns the full model (trip basics + day plan inputs + place inputs), passes it to each step via parameters, and receives callbacks. This avoids a complex state machine or third-party wizard library.

**Alternatives considered**:
- **URL-based steps** (`/citytrips/create/step1`, `/citytrips/create/step2`): Rejected — breaks "Back" button behavior and requires passing state via query params or session; more complex for no benefit in a demo app.
- **JS-driven tab/step logic**: Rejected — violates YAGNI and moves logic out of C# where it's testable.

---

## Decision 2: Extend Existing `CreateTrip` Slice vs. New `CreateTripWithSchedule` Slice

**Decision**: Extend the existing `CreateTripCommand`, `CreateTripHandler`, and `CreateTripValidator` in `CitytripPlanner.Features/Citytrips/CreateTrip/` to accept optional `ImageUrl` and `List<DayPlanInput>? DayPlans`.

**Rationale**: The existing slice already handles trip creation and persistence. Adding optional parameters preserves backward compatibility (all new parameters have defaults). Creating a separate slice would duplicate the entire create flow with no benefit — a clear YAGNI violation per Constitution Principle IV.

**Alternatives considered**:
- **New `CreateTripWithSchedule` slice**: Rejected — duplicates `CreateTripCommand`/`CreateTripHandler` logic; violates YAGNI.
- **Two-step command (create trip then add events via separate commands)**: Rejected — over-engineers the creation flow; the spec requires a single "confirm" action that saves everything atomically.

---

## Decision 3: Input DTO Types for Day Plans and Events in the Command

**Decision**: Define lightweight input record types `DayPlanInput`, `ScheduledEventInput`, and `PlaceInput` in the `CreateTrip` folder. These are command-side view models — they are not domain entities and live alongside the command, not in `Domain/`.

**Rationale**: The domain `DayPlan`, `ScheduledEvent`, and `Place` records have constructor validation that runs at creation time. The command must pass raw user input (including potentially invalid data) to the handler, which runs the validator before constructing domain objects. Reusing domain types directly in the command would trigger validation during command construction rather than in the handler/validator, which is the wrong layer.

**Alternatives considered**:
- **Reuse domain types directly**: Rejected — domain constructors throw on invalid input; validation belongs in the handler flow, not during command construction.
- **Anonymous types / tuples**: Rejected — poor readability and non-testable.

---

## Decision 4: Map Location Picker — Separate JS File vs. Extend `trip-map.js`

**Decision**: Create a separate `location-picker.js` file (`wwwroot/js/location-picker.js`) with a `locationPicker` IIFE module exposing `initPicker(elementId, dotNetRef, lat?, lng?)` and `destroyPicker(elementId)`.

**Rationale**: The existing `trip-map.js` has a single-map-instance design (`let map = null` module-level variable). A second concurrent map instance (detail-page map + creation picker) would conflict. A separate module with its own state avoids this conflict cleanly. The `DotNetObjectReference` + `[JSInvokable]` callback pattern is already established in feature 006 (`observeDaySections`), so reusing it for the picker is consistent.

**Alternatives considered**:
- **Extend `trip-map.js`**: Rejected — module-level `map` singleton would conflict when both the detail page map and picker need to coexist; would require refactoring feature 006's JS.
- **Render a static map image for picking**: Rejected — doesn't satisfy the spec requirement for clicking to place a pin.

---

## Decision 5: Reverse Geocoding for Place Name

**Decision**: Attempt reverse geocoding using the Google Maps Geocoder API when the user places a pin. If it succeeds, auto-populate the place name field. Always show an editable text field so the user can override the geocoded name or type manually when the API is unavailable.

**Rationale**: The Google Maps JS API includes the Geocoder service at no additional setup cost when the Maps JavaScript API is loaded. Using it avoids requiring users to type place names manually for common locations. Showing an always-visible text field satisfies the fallback requirement when geocoding fails or the API is unavailable.

**Alternatives considered**:
- **Places Autocomplete API**: Rejected — requires enabling an additional API in Google Cloud Console (beyond the Maps JavaScript API); YAGNI for a demo.
- **Manual name input only (no geocoding)**: Rejected — spec assumption explicitly prefers auto-population via reverse geocoding if available.

---

## Decision 6: Wizard "Create" Entry Point — Replace Modal or Keep Both

**Decision**: Replace the "Create" flow only: `MyCitytrips.razor` `OpenCreateModal()` navigates to `/citytrips/create` instead of showing `TripFormModal`. The Edit flow keeps `TripFormModal` unchanged.

**Rationale**: The spec says the existing `TripFormModal` is replaced by the wizard for creation. Edit is not in scope for this feature. Keeping the edit modal untouched avoids unrelated changes and is consistent with the spec's boundary.

**Alternatives considered**:
- **Replace both create and edit with wizard**: Out of scope per spec.
- **Keep modal for create, add separate wizard page**: Rejected — spec clarification Q1 confirmed wizard replaces the create modal.
