# Tasks: Citytrip Detail Page with Day Schedule and Map

**Input**: Design documents from `/specs/006-trip-day-schedule/`
**Prerequisites**: plan.md ✅ spec.md ✅ research.md ✅ data-model.md ✅ contracts/ ✅ quickstart.md ✅

**Tests**: Included — Constitution Principle II (TDD) is NON-NEGOTIABLE. Write failing tests before every implementation task.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create new folders, scaffold files, and add shared Web-layer helpers. No domain or feature code yet.

- [X] T001 Create `CitytripPlanner.Web/Components/Citytrips/` sub-component folder (DaySchedulePanel, ScheduledEventCard, TripMapSidebar will live here)
- [X] T002 [P] Create empty `CitytripPlanner.Web/wwwroot/js/trip-map.js` scaffold with `window.tripMap = {}` placeholder
- [X] T003 [P] Create `CitytripPlanner.Web/EventTypeIconMap.cs` static class with `Dictionary<string, string>` icon mapping and `GetIcon(string)` fallback method

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: New domain entities, extended DayPlan, extended handler and DTOs, and seed data. ALL user story phases depend on this phase completing first.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete. Tests must be written and failing BEFORE each implementation task.

### Domain: Place entity

- [X] T004 Write failing unit tests for `Place` validation (valid coords, invalid lat/lng ranges, empty name) in `CitytripPlanner.Tests/Citytrips/Domain/PlaceTests.cs`
- [X] T005 [P] Implement `Place` record with coordinate range validation in `CitytripPlanner.Features/Citytrips/Domain/Place.cs`

### Domain: ScheduledEvent entity

- [X] T006 Write failing unit tests for `ScheduledEvent` validation (required fields, EndTime > StartTime, Place optional) in `CitytripPlanner.Tests/Citytrips/Domain/ScheduledEventTests.cs`
- [X] T007 Implement `ScheduledEvent` record with constructor validation in `CitytripPlanner.Features/Citytrips/Domain/ScheduledEvent.cs`

### Domain: DayPlan extension

- [X] T008 Write failing unit tests for `DayPlan` extension: no Events param → empty list; Events param → exposed correctly (extend `CitytripPlanner.Tests/Citytrips/Domain/DayPlanTests.cs`)
- [X] T009 Extend `DayPlan` record with optional `List<ScheduledEvent>? events = null` constructor parameter defaulting to empty list in `CitytripPlanner.Features/Citytrips/Domain/DayPlan.cs`

### Response DTOs

- [X] T010 [P] Add `ScheduledEventDetail` and `PlaceDetail` records, and extend `DayPlanDetail` with `List<ScheduledEventDetail> Events` in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs`

### Handler extension

- [X] T011 Write failing handler tests: events appear in response ordered by StartTime; event with Place maps Latitude/Longitude correctly (extend `CitytripPlanner.Tests/Citytrips/GetCitytripDetail/GetCitytripDetailTests.cs`)
- [X] T012 Extend `GetCitytripDetailHandler` to project `DayPlan.Events → List<ScheduledEventDetail>` ordered by `StartTime` in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs`

### Seed data

- [X] T013 Add `ScheduledEvent` and `Place` seed data to Paris trip (ID 1) DayPlan days 1 and 2 in `CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs`

**Checkpoint**: Run `dotnet test` — all domain and handler tests must pass before proceeding to user story phases.

---

## Phase 3: User Story 1 — Navigate to Citytrip Detail (Priority: P1) 🎯 MVP

**Goal**: Confirm the existing detail page (`/citytrips/{Id:int}`) loads correctly with the extended domain data. Navigation, trip header (name, destination, date range), and back navigation are inherited from feature 005 and must remain unbroken.

**Independent Test**: Navigate to `/citytrips/1` — page loads, trip title "Paris Adventure" and destination display correctly, back link works, and no runtime errors occur.

### Regression verification for User Story 1

- [X] T014 [US1] Run existing `GetCitytripDetailTests.cs` tests and confirm all previously passing assertions still pass after T009–T012 changes
- [X] T015 [US1] Manually navigate to `/citytrips/1` and verify trip name, destination, and date range render correctly in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor`

**Checkpoint**: User Story 1 fully functional. Detail page loads with correct trip header. Foundation for schedule and map is in place.

---

## Phase 4: User Story 2 — View Day Schedule (Priority: P2)

**Goal**: The detail page displays a full scrollable day schedule. Each day section has a styled date header. Each event shows its type icon, type label, name, start time, and location name. Any event type renders correctly without code changes.

**Independent Test**: Navigate to `/citytrips/1` — at least two day sections appear with date headers; each event in Day 1 shows an icon (e.g., 🏛 or 🛒), the event name, start time, and location name; events are in chronological order within each day.

### Tests for User Story 2

- [X] T016 Write failing bUnit test: `ScheduledEventCard` renders event type label and name in `CitytripPlanner.Tests/Web/Citytrips/ScheduledEventCardTests.cs`
- [X] T017 [P] Write failing bUnit test: `ScheduledEventCard` renders Place name when Place is present; renders no location text when Place is null (extend `CitytripPlanner.Tests/Web/Citytrips/ScheduledEventCardTests.cs`)
- [X] T018 [P] Write failing bUnit test: `ScheduledEventCard` renders start time; renders end time when present (extend `CitytripPlanner.Tests/Web/Citytrips/ScheduledEventCardTests.cs`)
- [X] T019 Write failing bUnit test: `DaySchedulePanel` renders one section per day with `data-day="N"` attribute and date header text in `CitytripPlanner.Tests/Web/Citytrips/DaySchedulePanelTests.cs`
- [X] T020 [P] Write failing bUnit test: `DaySchedulePanel` renders empty-state message when all day plans have zero events (extend `CitytripPlanner.Tests/Web/Citytrips/DaySchedulePanelTests.cs`)

### Implementation for User Story 2

- [X] T021 [US2] Create `ScheduledEventCard.razor` — renders event type icon (via `EventTypeIconMap.GetIcon`), type label, name, start time, optional end time, optional description, optional location name in `CitytripPlanner.Web/Components/Citytrips/ScheduledEventCard.razor`
- [X] T022 [P] [US2] Create `ScheduledEventCard.razor.css` with event card styling (icon + label row, time row, location row) in `CitytripPlanner.Web/Components/Citytrips/ScheduledEventCard.razor.css`
- [X] T023 [US2] Create `DaySchedulePanel.razor` — renders a `<section data-day="@day.DayNumber">` for each day with styled date header (`Day N · weekday date`) and a `ScheduledEventCard` for each event; fires `OnDayVisible` EventCallback when a day section changes in `CitytripPlanner.Web/Components/Citytrips/DaySchedulePanel.razor`
- [X] T024 [P] [US2] Create `DaySchedulePanel.razor.css` with day section, date header, and event list styles in `CitytripPlanner.Web/Components/Citytrips/DaySchedulePanel.razor.css`
- [X] T025 [US2] Update `CitytripDetail.razor` to inject and render `<DaySchedulePanel DayPlans="@_trip.DayPlans" OnDayVisible="HandleDayVisible" />` replacing the old Attractions-based itinerary section in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor`
- [X] T026 [US2] Add empty-state message in `CitytripDetail.razor` when `_trip.DayPlans` is null or all day plans have zero events in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor`

**Checkpoint**: User Story 2 fully functional. Day schedule renders all events with icons, times, and locations. Empty-state shows correctly. No map yet — page is full-width.

---

## Phase 5: User Story 3 — View Map with Place Markers (Priority: P3)

**Goal**: A sticky map sidebar renders alongside the schedule in a two-column layout. On page load, the first day's event markers appear on the map. As the user scrolls into each new day section, the map updates to show only that day's markers. Interacting with a marker shows the place name.

**Independent Test**: Navigate to `/citytrips/1` — a map appears to the right of the schedule; Day 1 markers are visible on load; scrolling to Day 2 updates the map to show Day 2 markers only; clicking a marker shows the place name; if the map fails to load, the schedule remains readable.

### Tests for User Story 3

- [X] T027 Write failing bUnit test: `TripMapSidebar` renders a div with an id attribute (required for JS map init); renders with empty Markers list without error in `CitytripPlanner.Tests/Web/Citytrips/TripMapSidebarTests.cs`
- [X] T028 [P] Write failing bUnit test: `TripMapSidebar` renders a fallback message element when `MapLoadFailed` parameter is true (extend `CitytripPlanner.Tests/Web/Citytrips/TripMapSidebarTests.cs`)

### Implementation for User Story 3

- [X] T029 [US3] Implement `trip-map.js` with four functions: `initMap(elementId, markers)`, `updateMarkers(markers)`, `destroyMap(elementId)`, `observeDaySections(dotNetRef)` using Google Maps JS API and IntersectionObserver in `CitytripPlanner.Web/wwwroot/js/trip-map.js`
- [X] T030 [US3] Add Google Maps script tag to `CitytripPlanner.Web/Components/App.razor` head section reading API key from configuration (rendered server-side, never hard-coded)
- [X] T031 [US3] Create `TripMapSidebar.razor` — renders named map div; on `OnAfterRenderAsync(firstRender)` calls `tripMap.initMap`; on `OnParametersSetAsync` calls `tripMap.updateMarkers`; implements `IAsyncDisposable` to call `tripMap.destroyMap`; shows fallback when `MapLoadFailed` is true in `CitytripPlanner.Web/Components/Citytrips/TripMapSidebar.razor`
- [X] T032 [P] [US3] Create `TripMapSidebar.razor.css` with `height: 100%`, `border-radius`, and fallback message styles in `CitytripPlanner.Web/Components/Citytrips/TripMapSidebar.razor.css`
- [X] T033 [US3] Add `[JSInvokable] OnDayChanged(int dayNumber)` method to `CitytripDetail.razor`; on `OnAfterRenderAsync(firstRender)` call `tripMap.observeDaySections` with a `DotNetObjectReference`; dispose the reference on `DisposeAsync` in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor`
- [X] T034 [US3] Render `<TripMapSidebar Markers="@_activeMarkers" />` in the map column of `CitytripDetail.razor`; compute `_activeMarkers` from the active day's events that have a non-null `Place` in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor`
- [X] T035 [US3] Apply two-column CSS Grid layout (`detail-layout`: `1fr 400px`) and `position: sticky; top: 64px; height: calc(100vh - 80px)` on the map column in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor.css`

**Checkpoint**: All three user stories fully functional. Day schedule and map sidebar render side by side. Scroll-sync updates map markers per day. Fallback handles map service failure.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Edge case handling, empty states, and final validation across all stories.

- [X] T036 [P] Add Google Maps API key to `CitytripPlanner.Web/appsettings.Development.json` as a placeholder key with a comment instructing developers to replace it (`"GoogleMaps": { "ApiKey": "YOUR_KEY_HERE" }`)
- [X] T037 [P] Extend `EventTypeIconMap.cs` icon dictionary with at least 8 common event types (museum, market, stadium, restaurant, park, church, beach, gallery) in `CitytripPlanner.Web/EventTypeIconMap.cs`
- [X] T038 Add CSS responsive breakpoint: below 768px viewport width, map column collapses (hidden or stacked below schedule) in `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor.css`
- [X] T039 Run full test suite with `dotnet test CitytripPlanner/CitytripPlanner.Tests/CitytripPlanner.Tests.csproj` and verify zero failures
- [X] T040 Manual end-to-end walkthrough: navigate to `/citytrips/1`, verify schedule shows two days, events render with icons, map sidebar is sticky, scrolling Day 2 updates map markers, clicking a marker shows place name

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately. T001 is sequential; T002 and T003 are parallel.
- **Foundational (Phase 2)**: Depends on Phase 1. Domain tasks (T004–T009) must complete before T010–T013. T010 can parallel T004–T009.
- **US1 (Phase 3)**: Depends on Phase 2 completion.
- **US2 (Phase 4)**: Depends on Phase 2 completion. Tests (T016–T020) must be written and failing before implementation (T021–T026).
- **US3 (Phase 5)**: Depends on Phase 4 completion (needs `DaySchedulePanel` `data-day` attributes and `OnDayVisible` callback). Tests (T027–T028) must be written and failing before implementation (T029–T035).
- **Polish (Phase 6)**: Depends on Phases 3–5 completion.

### User Story Dependencies

- **US1 (P1)**: Depends on Foundational (Phase 2). No dependency on US2 or US3.
- **US2 (P2)**: Depends on Foundational (Phase 2). No dependency on US1 or US3.
- **US3 (P3)**: Depends on US2 (needs `data-day` attributes and `OnDayVisible` from `DaySchedulePanel`).

### Within Each Phase

- TDD strictly enforced: test tasks must be written and FAILING before corresponding implementation tasks
- `Place` before `ScheduledEvent` (T004→T005 before T006→T007)
- Domain entities before handler (T009 before T011)
- DTOs before handler extension (T010 before T012)
- `ScheduledEventCard` before `DaySchedulePanel` (T021 before T023)
- `DaySchedulePanel` before `TripMapSidebar` wiring (T023 before T033)

---

## Parallel Opportunities

### Phase 2 Parallel Example

```bash
# After T004→T005 (Place):
Task T006: Write ScheduledEvent tests   (different file from T004)
Task T010: Add DTOs to response file    (different file from T006)
```

### Phase 4 Parallel Example

```bash
# All failing tests for US2 can be written in parallel:
Task T016: ScheduledEventCard type/name test
Task T017: ScheduledEventCard Place test
Task T018: ScheduledEventCard time test
# Then:
Task T019: DaySchedulePanel header test
Task T020: DaySchedulePanel empty-state test

# Component + CSS files can be created in parallel:
Task T021: ScheduledEventCard.razor implementation
Task T022: ScheduledEventCard.razor.css
Task T024: DaySchedulePanel.razor.css
```

### Phase 5 Parallel Example

```bash
# Tests written in parallel:
Task T027: TripMapSidebar map container test
Task T028: TripMapSidebar fallback test

# CSS independent from JS:
Task T029: trip-map.js implementation
Task T032: TripMapSidebar.razor.css
```

---

## Implementation Strategy

### MVP First (User Story 1 + 2 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (domain + handler + seed data)
3. Complete Phase 3: US1 — verify existing navigation unbroken
4. Complete Phase 4: US2 — day schedule renders
5. **STOP and VALIDATE**: Navigate to `/citytrips/1`, confirm schedule with event icons, times, and locations
6. Demo/deploy if ready — map is not yet present

### Full Incremental Delivery

1. Setup → Foundation → US1 verification → **Demo: detail page loads**
2. Add US2 (day schedule) → **Demo: events appear in styled schedule**
3. Add US3 (map sidebar) → **Demo: sticky map syncs with scroll**
4. Polish → final test run

---

## Notes

- [P] tasks = different files, safe to parallelize
- TDD is non-negotiable: every implementation task has a preceding test task that must FAIL first
- Commit after each completed task or logical group (e.g., one commit per domain entity)
- US3 depends on US2's `data-day` attribute convention — do not skip US2
- Google Maps API key must never be committed to source control; use `appsettings.Development.json` (git-ignored) or environment variables
- All 40 tasks reference exact file paths for direct implementation without additional context
