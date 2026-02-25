# Tasks: Create Citytrip with Full Fields and Map-Based Location Picker

**Input**: Design documents from `/specs/007-create-citytrip-map/`
**Prerequisites**: plan.md ✅ spec.md ✅ research.md ✅ data-model.md ✅ contracts/ ✅ quickstart.md ✅

**Tests**: Included — Constitution Principle II (TDD) is NON-NEGOTIABLE. Write failing tests before every implementation task.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Scaffold new files so subsequent tasks have compile targets. No logic yet.

- [X] T001 Create empty input record files in `CitytripPlanner.Features/Citytrips/CreateTrip/`: `PlaceInput.cs`, `ScheduledEventInput.cs`, `DayPlanInput.cs` (placeholder `record` declarations only)
- [X] T002 [P] Create empty scaffold `CitytripPlanner.Web/wwwroot/js/location-picker.js` with `window.locationPicker = {};` placeholder
- [X] T003 [P] Add `<script src="/js/location-picker.js"></script>` tag to `CitytripPlanner.Web/Components/App.razor` head section (alongside existing `trip-map.js` script)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Extend the existing `CreateTrip` CQRS slice to accept `ImageUrl` and `DayPlans`. ALL user story phases depend on this.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete. Tests MUST be written and failing BEFORE each implementation task.

### Input Records

- [X] T004 Implement `PlaceInput` record (Name, Latitude, Longitude) in `CitytripPlanner.Features/Citytrips/CreateTrip/PlaceInput.cs`
- [X] T005 [P] Implement `ScheduledEventInput` record (EventType, Name, StartTime, EndTime?, Description?, Place?) in `CitytripPlanner.Features/Citytrips/CreateTrip/ScheduledEventInput.cs`
- [X] T006 [P] Implement `DayPlanInput` record (DayNumber, Date, List\<ScheduledEventInput\> Events) in `CitytripPlanner.Features/Citytrips/CreateTrip/DayPlanInput.cs`

### Command Extension

- [X] T007 Extend `CreateTripCommand` with optional `string? ImageUrl = null` and `List<DayPlanInput>? DayPlans = null` parameters in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripCommand.cs`

### Validator Extension

- [X] T008 Write failing validator tests: ImageUrl must be valid absolute URL when provided; event EndTime must be after StartTime; PlaceInput latitude in [-90,90]; PlaceInput longitude in [-180,180]; DayPlan date within trip date range — in `CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripValidatorTests.cs`
- [X] T009 Extend `CreateTripValidator` with URL validation (`Uri.TryCreate`), event end-time rule, place coordinate range checks, and day-date range check in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripValidator.cs`

### Handler Extension

- [X] T010 Write failing handler tests: trip with `ImageUrl` saves ImageUrl to domain entity; trip with one `DayPlanInput` containing two events saves both events ordered by StartTime; trip with event containing `PlaceInput` saves correct lat/lng/name on `Place` domain object — extend `CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripHandlerTests.cs`
- [X] T011 Extend `CreateTripHandler` to pass `ImageUrl` to `Citytrip` constructor and map `DayPlanInput → DayPlan (with empty Attractions, events ordered by StartTime) → ScheduledEvent → Place` in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripHandler.cs`

**Checkpoint**: Run `dotnet test` — all foundational tests must pass before proceeding.

---

## Phase 3: User Story 1 — Fill in Citytrip Basics (Priority: P1) 🎯 MVP

**Goal**: Dedicated wizard page at `/citytrips/create` with Step 1 (basic fields form) and Step 3 (review + confirm), replacing the modal for the Create flow. No day plans needed for this story.

**Independent Test**: Navigate to `/my-citytrips`, click "+ Create Citytrip", confirm you land on `/citytrips/create` (not a modal). Fill in title, destination, dates, description, max participants, and image URL. Click Next → Step 3 shows a read-only summary. Click Confirm → redirected to `/my-citytrips`, new trip card visible. Open detail page — all fields correct.

### Tests for User Story 1

> **Write these tests FIRST and confirm they FAIL before implementing**

- [X] T012 Write failing bUnit test: `WizardStep1` renders inputs for title, destination, start date, end date, description, max participants, image URL in `CitytripPlanner.Tests/Web/Citytrips/WizardStep1Tests.cs`
- [X] T013 [P] Write failing bUnit test: `WizardStep1` invokes `OnNext` callback when all required fields valid; displays validation errors and does NOT invoke `OnNext` when title, destination, or start date is missing in `CitytripPlanner.Tests/Web/Citytrips/WizardStep1Tests.cs`
- [X] T014 [P] Write failing bUnit test: `WizardStep3` renders all trip basics (title, destination, date range, description, max participants, image URL) from parameters in `CitytripPlanner.Tests/Web/Citytrips/WizardStep3Tests.cs`

### Implementation for User Story 1

- [X] T015 [US1] Create `WizardStep1.razor` — renders all basic trip fields with two-way binding; validates title (required), destination (required), start date (required), end date ≥ start date, max participants ≥ 1 when set, image URL format; exposes `OnNext(model)` EventCallback in `CitytripPlanner.Web/Components/Citytrips/WizardStep1.razor`
- [X] T016 [P] [US1] Create `WizardStep1.razor.css` with form field layout and validation error styles in `CitytripPlanner.Web/Components/Citytrips/WizardStep1.razor.css`
- [X] T017 [US1] Create `WizardStep3.razor` — renders read-only summary of trip basics (title, destination, date range, description, max participants, image URL preview); exposes `OnConfirm` and `OnBack` EventCallbacks in `CitytripPlanner.Web/Components/Citytrips/WizardStep3.razor`
- [X] T018 [P] [US1] Create `WizardStep3.razor.css` in `CitytripPlanner.Web/Components/Citytrips/WizardStep3.razor.css`
- [X] T019 [US1] Create `CreateCitytrip.razor` page at route `/citytrips/create` — owns `_step` state (1/2/3), trip basics fields, and `_dayPlans` list; Step 1 renders `<WizardStep1>`, Step 3 renders `<WizardStep3>`; Confirm dispatches `CreateTripCommand` (with empty `DayPlans`) via `IMediator`, then navigates to `/my-citytrips`; includes step indicator (Step N of 3) in `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor`
- [X] T020 [P] [US1] Create `CreateCitytrip.razor.css` with wizard shell styles (step indicator, step content area, nav buttons) in `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor.css`
- [X] T021 [US1] Modify `MyCitytrips.razor` — inject `NavigationManager`; replace `OpenCreateModal()` body with `Navigation.NavigateTo("/citytrips/create")`; keep `TripFormModal` usage for edit flow unchanged in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`

**Checkpoint**: User Story 1 fully functional. Navigate `/my-citytrips` → Create → wizard at `/citytrips/create` → Step 1 fields → Step 3 review → Confirm → trip on list + correct detail page.

---

## Phase 4: User Story 2 — Add Day Plans with Scheduled Events (Priority: P2)

**Goal**: Wizard Step 2 renders auto-generated day plan slots from the date range. Each slot allows adding/removing scheduled events (type, name, start time, optional end time, description). Step 3 review includes the event summary.

**Independent Test**: Complete Step 1 (2-day trip). Advance to Step 2 — verify 2 day sections appear. Add one event to Day 1 (museum / Shibuya Crossing / 10:00), add one to Day 2 (restaurant / Ramen / 12:00). Advance to Step 3 — review shows both days and their events. Confirm — detail page shows Day 1 and Day 2 sections with correct events in order.

### Tests for User Story 2

> **Write these tests FIRST and confirm they FAIL before implementing**

- [X] T022 Write failing bUnit test: `EventEditorRow` renders event type, name, start time, end time (optional), and description inputs; renders a remove button in `CitytripPlanner.Tests/Web/Citytrips/EventEditorRowTests.cs`
- [X] T023 [P] Write failing bUnit test: `EventEditorRow` fires `OnValidationError` when end time ≤ start time; fires `OnRemove` when remove button clicked in `CitytripPlanner.Tests/Web/Citytrips/EventEditorRowTests.cs`
- [X] T024 Write failing bUnit test: `WizardStep2` renders one `<section data-day="N">` per day in the provided `DayPlans` list, with a date header and an "Add Event" button in `CitytripPlanner.Tests/Web/Citytrips/WizardStep2Tests.cs`
- [X] T025 [P] Write failing bUnit test: `WizardStep2` adds a new empty event row when "Add Event" is clicked; fires `OnBack` and `OnNext` callbacks in `CitytripPlanner.Tests/Web/Citytrips/WizardStep2Tests.cs`
- [X] T026 [P] Write failing bUnit test: `WizardStep3` renders day plan summary section with day headers and event names/times when `DayPlans` parameter is non-empty in `CitytripPlanner.Tests/Web/Citytrips/WizardStep3Tests.cs`

### Implementation for User Story 2

- [X] T027 [US2] Create `EventEditorRow.razor` — renders event type, name, start time, end time, description inputs; validates end time > start time; Remove button fires `OnRemove` EventCallback; exposes `OnPickLocation` EventCallback (stub for US3) in `CitytripPlanner.Web/Components/Citytrips/EventEditorRow.razor`
- [X] T028 [P] [US2] Create `EventEditorRow.razor.css` with event input row, field labels, and remove button styles in `CitytripPlanner.Web/Components/Citytrips/EventEditorRow.razor.css`
- [X] T029 [US2] Create `WizardStep2.razor` — renders one `<section data-day="@day.DayNumber">` per `DayPlan` slot with date header; "Add Event" button appends empty event row; renders `<EventEditorRow>` per event with remove support; Back/Next callbacks; validates no event has end time ≤ start time before allowing Next in `CitytripPlanner.Web/Components/Citytrips/WizardStep2.razor`
- [X] T030 [P] [US2] Create `WizardStep2.razor.css` with day section, date header, and event list styles in `CitytripPlanner.Web/Components/Citytrips/WizardStep2.razor.css`
- [X] T031 [US2] Update `WizardStep3.razor` to include a day plans summary section rendering each day's number, date, and event list (type icon via `EventTypeIconMap.GetIcon`, name, time range) in `CitytripPlanner.Web/Components/Citytrips/WizardStep3.razor`
- [X] T032 [US2] Update `CreateCitytrip.razor` — auto-generate `_dayPlans` from date range on Step 1→2 transition; render `<WizardStep2 DayPlans="_dayPlans">` at `_step == 2`; pass full `DayPlanInput` list in `CreateTripCommand` on confirm in `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor`

**Checkpoint**: User Story 2 fully functional. Step 2 renders day slots, events can be added/removed, Step 3 review includes event summary, confirmed trip shows full schedule on detail page.

---

## Phase 5: User Story 3 — Map Location Picker (Priority: P3)

**Goal**: "Pick on map" button on each event row in Step 2 opens a map picker modal. User clicks the map to place a pin, reverse geocoding auto-fills place name. Confirm records the `PlaceInput` on the event. Saved trip shows the place as a marker on the detail-page map.

**Independent Test**: Complete Step 1 (1-day trip). In Step 2, add one event and click "Pick on map" — verify a modal opens with a Google Map. Click a location — pin placed, name field populated. Click Confirm Location — modal closes, event row shows place name badge. Complete wizard — detail page map shows a marker at the chosen location.

### Tests for User Story 3

> **Write these tests FIRST and confirm they FAIL before implementing**

- [X] T033 Write failing bUnit test: `LocationPickerModal` renders map container div when `FakeJSRuntime` returns `true`; renders fallback message when `FakeJSRuntime` returns `false` in `CitytripPlanner.Tests/Web/Citytrips/LocationPickerModalTests.cs`
- [X] T034 [P] Write failing bUnit test: `LocationPickerModal` fires `OnConfirm(PlaceInput)` with correct name/lat/lng when Confirm button is clicked after `OnLocationPicked` has been called; fires `OnCancel` when Cancel is clicked in `CitytripPlanner.Tests/Web/Citytrips/LocationPickerModalTests.cs`
- [X] T035 [P] Write failing bUnit test: `LocationPickerModal` pre-populates place name and shows "Change location" label when existing `InitialLat`/`InitialLng`/`InitialName` parameters are provided in `CitytripPlanner.Tests/Web/Citytrips/LocationPickerModalTests.cs`

### Implementation for User Story 3

- [X] T036 [US3] Implement `location-picker.js` with `initPicker(elementId, dotNetRef, lat, lng)` — initialises Google Maps map, places draggable marker if lat/lng provided, handles map click to place/move marker, attempts reverse geocoding and calls `dotNetRef.invokeMethodAsync('OnLocationPicked', lat, lng, name)`, returns `true`/`false`; and `destroyPicker(elementId)` — clears marker and map reference — in `CitytripPlanner.Web/wwwroot/js/location-picker.js`
- [X] T037 [US3] Create `LocationPickerModal.razor` — renders named map div or fallback; `OnAfterRenderAsync(firstRender)` calls `locationPicker.initPicker` with `DotNetObjectReference`; `[JSInvokable] OnLocationPicked(double lat, double lng, string name)` updates `_lat`, `_lng`, `_placeName` and calls `StateHasChanged()`; editable place name text field always visible (override geocoded name); Confirm button fires `OnConfirm(new PlaceInput(_placeName, _lat, _lng))`; Cancel fires `OnCancel`; `IAsyncDisposable` calls `locationPicker.destroyPicker` in `CitytripPlanner.Web/Components/Citytrips/LocationPickerModal.razor`
- [X] T038 [P] [US3] Create `LocationPickerModal.razor.css` — modal overlay, map container height (400px), fallback styles, place name input, confirm/cancel buttons — in `CitytripPlanner.Web/Components/Citytrips/LocationPickerModal.razor.css`
- [X] T039 [US3] Update `EventEditorRow.razor` — add "Pick on map" button that fires `OnPickLocation` EventCallback; when `Place` parameter is non-null, show a place name badge with a "Change" link; expose `Place` as a parameter and `OnPickLocation` as EventCallback in `CitytripPlanner.Web/Components/Citytrips/EventEditorRow.razor`
- [X] T040 [US3] Update `WizardStep2.razor` — add `_pickingForEvent` state (`ScheduledEventInputModel?`); when "Pick on map" fires on an event row, set `_pickingForEvent`; render `<LocationPickerModal>` modal when `_pickingForEvent != null`; on `OnConfirm(place)` update `_pickingForEvent.Place` and clear `_pickingForEvent`; on `OnCancel` clear `_pickingForEvent` in `CitytripPlanner.Web/Components/Citytrips/WizardStep2.razor`

**Checkpoint**: All three user stories fully functional. Events can have pinned locations. Detail page shows markers. Map fallback shown when API unavailable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Date change confirmation, responsive layout, final validation.

- [X] T041 [P] Add date-range change confirmation prompt to `CreateCitytrip.razor` — when the user navigates back to Step 1 and changes start/end date while `_dayPlans` already contains events, call `JS.InvokeAsync<bool>("confirm", "Changing the dates will clear your event schedule. Continue?")` before regenerating; if user cancels, restore original dates in `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor`
- [X] T042 [P] Add responsive CSS breakpoint in `CreateCitytrip.razor.css` — below 768px, step indicator wraps vertically and wizard form fields stack to single column in `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor.css`
- [X] T043 Run full test suite `dotnet test CitytripPlanner/CitytripPlanner.Tests/CitytripPlanner.Tests.csproj` — verify zero failures
- [X] T044 Manual end-to-end walkthrough: follow quickstart.md Scenarios 1–6 (basic creation, validation, events, back navigation, date change confirmation, map picker with API key); verify all acceptance criteria pass

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately. T001 is sequential; T002 and T003 are parallel.
- **Foundational (Phase 2)**: Depends on Phase 1. T004–T006 (input records) must complete before T007 (command extension). T008 (validator tests) must FAIL before T009 (validator impl). T010 (handler tests) must FAIL before T011 (handler impl).
- **US1 (Phase 3)**: Depends on Phase 2 completion. Tests (T012–T014) must be written and failing before implementation (T015–T021).
- **US2 (Phase 4)**: Depends on Phase 2 completion and US1 completion (CreateCitytrip.razor shell must exist). Tests (T022–T026) must be written and failing before implementation (T027–T032).
- **US3 (Phase 5)**: Depends on US2 completion (EventEditorRow and WizardStep2 must exist). Tests (T033–T035) must be written and failing before implementation (T036–T040).
- **Polish (Phase 6)**: Depends on Phases 3–5 completion.

### User Story Dependencies

- **US1 (P1)**: Depends on Foundational (Phase 2). Independently testable without US2/US3.
- **US2 (P2)**: Depends on Foundational (Phase 2) + US1 shell (`CreateCitytrip.razor` must exist). Independently testable — add events, confirm, check detail page.
- **US3 (P3)**: Depends on US2 (`EventEditorRow` + `WizardStep2` must exist with `OnPickLocation` stub). Independently testable — map picker functions without US1/US2 changes.

### Within Each Phase

- TDD strictly enforced: test tasks must be written and FAILING before corresponding implementation tasks
- Input records (T004–T006) before command extension (T007)
- Validator (T009) before handler (T011)
- `WizardStep1` (T015) before `CreateCitytrip.razor` (T019)
- `EventEditorRow` (T027) before `WizardStep2` (T029)
- `LocationPickerModal` (T037) before `WizardStep2` update (T040)

---

## Parallel Opportunities

### Phase 2 Parallel Example

```bash
# Input records can be written in parallel (different files):
Task T004: PlaceInput.cs
Task T005: ScheduledEventInput.cs
Task T006: DayPlanInput.cs

# After T007 (command extended):
Task T008: Write validator tests         (CitytripPlanner.Tests)
Task T010: Write handler tests           (CitytripPlanner.Tests)
```

### Phase 3 Parallel Example

```bash
# All failing tests for US1 can be written in parallel:
Task T012: WizardStep1 renders all fields
Task T013: WizardStep1 validation behavior
Task T014: WizardStep3 renders basics summary

# CSS files are always parallel to their razor counterparts:
Task T015: WizardStep1.razor implementation
Task T016: WizardStep1.razor.css
Task T018: WizardStep3.razor.css   (after T017 implementation)
Task T020: CreateCitytrip.razor.css (after T019 implementation)
```

### Phase 5 Parallel Example

```bash
# All failing tests for US3 can be written in parallel:
Task T033: LocationPickerModal map/fallback test
Task T034: LocationPickerModal confirm/cancel callbacks test
Task T035: LocationPickerModal pre-existing pin test

# CSS independent from JS and razor:
Task T036: location-picker.js implementation
Task T038: LocationPickerModal.razor.css
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (command/validator/handler extension)
3. Complete Phase 3: US1 — wizard page with basics form and review
4. **STOP and VALIDATE**: Navigate to `/citytrips/create`, fill all Step 1 fields, confirm, verify detail page
5. Demo/deploy if ready — day plans and map not yet present

### Full Incremental Delivery

1. Setup → Foundation → US1 → **Demo: full-fields create wizard works**
2. Add US2 (day plans + events) → **Demo: events appear in schedule on detail page**
3. Add US3 (map picker) → **Demo: pin locations, markers on detail map**
4. Polish → final test run

---

## Notes

- [P] tasks = different files, safe to parallelize
- TDD is non-negotiable (Constitution Principle II): every implementation task has a preceding failing test
- `DayPlanInputModel` / `ScheduledEventInputModel` / `PlaceInputModel` are mutable Web-layer classes for `@bind` two-way binding — they are separate from the `DayPlanInput` / `ScheduledEventInput` / `PlaceInput` record types used in the command (Features layer)
- `TripFormModal` is NOT modified — it remains for the edit flow only
- `location-picker.js` is a separate IIFE from `trip-map.js` to avoid the JS singleton conflict (see research.md Decision 4)
- All 44 tasks reference exact file paths for direct implementation without additional context
