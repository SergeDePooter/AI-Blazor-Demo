# Tasks: Filter Citytrips

**Input**: Design documents from `/specs/003-filter-citytrips/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: TDD is NON-NEGOTIABLE per Constitution Principle II. Tests are written first for all filter logic.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup

**Purpose**: Create the filter criteria model and pure filter function shell

- [x] T001 [P] Create `CitytripFilterCriteria` record in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripFilter.cs`
- [x] T002 [P] Create `CitytripFilter` static class with empty `Apply()` method stub in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripFilter.cs` (same file as T001)

**Checkpoint**: Filter types exist, project builds, no behavior yet

---

## Phase 2: Foundational (Filter Logic with TDD)

**Purpose**: Implement the pure filter logic with tests first — this is the shared foundation all UI stories depend on

**⚠️ CRITICAL**: No UI work can begin until the filter logic is tested and working

### Tests (write FIRST, must FAIL)

- [x] T003 [P] Write test `Apply_ReturnsAllTrips_WhenNoCriteriaSet` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T004 [P] Write test `Apply_FiltersByTitle_CaseInsensitive` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T005 [P] Write test `Apply_FiltersByDestination_CaseInsensitive` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T006 [P] Write test `Apply_FiltersByTitleOrDestination_PartialMatch` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T007 [P] Write test `Apply_TrimsWhitespaceSearchText` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T008 [P] Write test `Apply_FiltersByFromDate_ExcludesTripsEndingBefore` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T009 [P] Write test `Apply_FiltersByToDate_ExcludesTripsStartingAfter` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T010 [P] Write test `Apply_CombinesTextAndDateFilters_WithAndLogic` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`
- [x] T011 [P] Write test `Apply_ReturnsEmptyList_WhenNoTripsMatch` in `CitytripPlanner.Tests/Citytrips/ListCitytrips/CitytripFilterTests.cs`

### Implementation (make tests pass)

- [x] T012 Implement `CitytripFilter.Apply()` method: text search (case-insensitive, partial match on Title OR Destination, trim whitespace) in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripFilter.cs`
- [x] T013 Implement `CitytripFilter.Apply()` method: date range filtering (FromDate: keep EndDate >= FromDate, ToDate: keep StartDate <= ToDate) in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripFilter.cs`
- [x] T014 Implement `CitytripFilter.Apply()` method: AND combination of all active criteria in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripFilter.cs`

**Checkpoint**: All 9 filter tests pass. Foundation ready — UI implementation can begin.

---

## Phase 3: User Story 1 - Filter by Text (Priority: P1) 🎯 MVP

**Goal**: Users can type a search term to filter trips by title or destination with debounced input

**Independent Test**: Type a search term in the text input, verify only matching trips are shown after ~300ms pause

### Implementation for User Story 1

- [x] T015 [US1] Add filter state fields (`_searchText`, `_filteredTrips`, `CancellationTokenSource`) and `ApplyFilters()` method to `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T016 [US1] Add text search input with `@oninput` binding and debounce logic (CancellationTokenSource + Task.Delay 300ms) to `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T017 [US1] Update trip list rendering to use `_filteredTrips` instead of `_trips` in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T018 [US1] Add empty state message for zero filter results (distinct from "no trips exist") in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T019 [US1] Add filter area CSS styles (layout, text input styling) in `CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`

**Checkpoint**: Text filter works with debounce. Empty state shown when no matches. Existing like/enlist interactions preserved.

---

## Phase 4: User Story 2 - Filter by Date Range (Priority: P2)

**Goal**: Users can filter trips by selecting from/to dates with native date pickers

**Independent Test**: Select a from date and/or to date, verify only overlapping trips are shown

### Implementation for User Story 2

- [x] T020 [US2] Add date filter state fields (`_fromDate`, `_toDate`) to `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T021 [US2] Add "from" date picker input with `@onchange` binding and `max` constraint (bound to `_toDate`) in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T022 [US2] Add "to" date picker input with `@onchange` binding and `min` constraint (bound to `_fromDate`) in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T023 [US2] Wire date picker changes to call `ApplyFilters()` immediately (no debounce) in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T024 [US2] Add date picker CSS styles in `CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`

**Checkpoint**: Date range filtering works. Combined with text filter (AND logic). Date validation prevents invalid ranges.

---

## Phase 5: User Story 3 & 4 - Combined Filters and Clear All (Priority: P3)

**Goal**: Users can use all filters together and reset them with a single action

**Independent Test**: Apply text + date filters, click "Clear all filters", verify all inputs reset and full list returns

### Implementation for User Stories 3 & 4

- [x] T025 [US3] Verify combined filter behavior (text + dates AND logic) works correctly — adjust `ApplyFilters()` if needed in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T026 [US4] Add `ClearAllFilters()` method that resets `_searchText`, `_fromDate`, `_toDate` and calls `ApplyFilters()` in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T027 [US4] Add "Clear all filters" button (visible/enabled only when at least one filter is active) in `CitytripPlanner.Web/Components/Pages/Citytrips.razor`
- [x] T028 [US4] Add "Clear all filters" button CSS styles in `CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`

**Checkpoint**: All filter combinations work. Clear button resets everything. Full feature complete.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and regression check

- [x] T029 Run full test suite with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/` and fix any regressions
- [x] T030 Verify existing feature 001 browse page interactions (like, enlist) work correctly with filters active
- [x] T031 Run quickstart.md validation: start app, test all filter scenarios on `/citytrips` page

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — tests need the types to exist. BLOCKS all UI work.
- **US1 (Phase 3)**: Depends on Phase 2 — needs working filter logic
- **US2 (Phase 4)**: Depends on Phase 3 — extends the same filter area UI
- **US3 & US4 (Phase 5)**: Depends on Phase 4 — combines and clears all filters
- **Polish (Phase 6)**: Depends on Phase 5

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) — introduces filter UI
- **User Story 2 (P2)**: Depends on US1 — adds date pickers to the same filter area
- **User Story 3 (P3)**: Depends on US1 + US2 — verifies combined behavior
- **User Story 4 (P3)**: Depends on US1 + US2 — "clear all" resets all filter inputs

### Within Each Phase

- TDD: Tests MUST be written and FAIL before implementation (Phase 2)
- Filter logic before UI (Phase 2 before Phase 3+)
- Text filter before date filter (simpler first)
- Commit after each phase

### Parallel Opportunities

- T001 and T002 can run in parallel (same file but independent definitions)
- T003–T011 can all run in parallel (independent test methods in same file)
- T012–T014 are sequential (building up the Apply method incrementally)

---

## Parallel Example: Phase 2 (Foundational Tests)

```bash
# Launch all filter tests together (they all go in CitytripFilterTests.cs):
Task: "Write test Apply_ReturnsAllTrips_WhenNoCriteriaSet"
Task: "Write test Apply_FiltersByTitle_CaseInsensitive"
Task: "Write test Apply_FiltersByDestination_CaseInsensitive"
Task: "Write test Apply_FiltersByTitleOrDestination_PartialMatch"
Task: "Write test Apply_TrimsWhitespaceSearchText"
Task: "Write test Apply_FiltersByFromDate_ExcludesTripsEndingBefore"
Task: "Write test Apply_FiltersByToDate_ExcludesTripsStartingAfter"
Task: "Write test Apply_CombinesTextAndDateFilters_WithAndLogic"
Task: "Write test Apply_ReturnsEmptyList_WhenNoTripsMatch"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (types)
2. Complete Phase 2: Foundational (filter logic + tests)
3. Complete Phase 3: User Story 1 (text filter UI with debounce)
4. **STOP and VALIDATE**: Text filter works, trips can be found by name/destination
5. Demo if ready

### Incremental Delivery

1. Setup + Foundational → Filter logic tested and ready
2. Add US1 (Text Filter) → Test → Demo (MVP!)
3. Add US2 (Date Filter) → Test → Demo
4. Add US3+US4 (Combined + Clear All) → Test → Demo
5. Polish → Final validation

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- TDD is enforced per Constitution Principle II — filter logic has tests written first
- US3 and US4 are combined in Phase 5 because they're both P3 and share the same UI area
- All UI changes are in a single file (`Citytrips.razor`) so UI tasks within a phase are sequential
- Commit after each phase or logical group
- Stop at any checkpoint to validate story independently
