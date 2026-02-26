# Tasks: Edit Citytrip via Wizard

**Input**: Design documents from `/specs/008-edit-citytrip-wizard/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/update-trip-command.md, contracts/edit-wizard-page.md

**Tests**: TDD is NON-NEGOTIABLE (Constitution Principle II). Every implementation
task is preceded by its failing test(s). Write the test, confirm it fails, then
implement.

**Organization**: Tasks are grouped by user story to enable independent implementation
and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to

---

## Phase 2: Foundational — CitytripDetailResponse + CreatorId

**Purpose**: `EditCitytrip.razor` must verify trip ownership using `CreatorId`.
The response DTO currently lacks this field. All three user stories depend on this
fix before the edit page can load a trip.

**⚠️ CRITICAL**: All user story phases depend on this phase being complete.

- [X] T001 Write failing test `Handle_ValidId_MapsCreatorId` in `CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetail/GetCitytripDetailTests.cs` — assert `result.CreatorId == "creator"` using NSubstitute mock; test will not compile until `CitytripDetailResponse` gains the field
- [X] T002 Add `string CreatorId` positional field (after `MaxParticipants`) to the record in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs` — T001 now compiles but fails (handler does not map the field yet)
- [X] T003 Update `GetCitytripDetailHandler.Handle` to pass `trip.CreatorId` at the new position in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs` — T001 passes; also fix the `CitytripDetail.razor` page if the positional ctor call there breaks

**Checkpoint**: `GetCitytripDetailTests` all green; `CitytripDetailResponse` carries `CreatorId`.

---

## Phase 3: User Story 1 — Edit Citytrip Basics (Priority: P1) 🎯 MVP

**Goal**: Owner navigates from My Citytrips → Edit, wizard opens at Step 1 with all
basic fields pre-filled; they change the title and image URL; Step 3 shows "Save
Changes"; confirm → trip updated on My Citytrips page.

**Independent Test**: Navigate to My Citytrips → click Edit on an owned trip → verify
title and image URL are pre-filled → change title → advance to Step 3 → confirm →
trip card shows updated title.

### Tests for User Story 1

> **Write ALL tests in this section FIRST. Confirm they FAIL before proceeding to implementation.**

- [X] T004 [P] [US1] Extend `UpdateTripHandlerTests` with two failing tests — `Handle_UpdatesTripWithValidImageUrl` (ImageUrl stored on updated trip) and `Handle_PreservesExistingImageUrl_WhenImageUrlIsNull` — in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripHandlerTests.cs`
- [X] T005 [P] [US1] Extend `UpdateTripValidatorTests` with two failing tests — `Validate_InvalidImageUrl_ReturnsError` (non-absolute URL string) and `Validate_NullImageUrl_IsValid` — in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripValidatorTests.cs`
- [X] T006 [P] [US1] Extend `WizardStep1Tests` with two failing tests — `WizardStep1_WithInitialModel_PreFillsAllFields` (input values match InitialModel properties after render) and `WizardStep1_WithNoInitialModel_RendersEmptyFields` (backward-compat check) — in `CitytripPlanner.Tests/Web/Citytrips/WizardStep1Tests.cs`
- [X] T007 [US1] Create `EditCitytripPageTests.cs` with four failing tests: `EditPage_LoadsTrip_PreFillsStep1Fields`, `EditPage_NonExistentTrip_ShowsNotFound`, `EditPage_OtherUsersTrip_ShowsUnauthorized`, `EditPage_Confirm_DispatchesUpdateTripCommand` — in `CitytripPlanner.Tests/Web/Pages/EditCitytripPageTests.cs`; use NSubstitute mock for `IMediator` (inject via `Services.AddSingleton`), stub `ICurrentUserService` returning "test-user"

### Implementation for User Story 1

- [X] T008 [P] [US1] Extend `UpdateTripCommand` record with `string? ImageUrl = null` optional parameter at the end in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripCommand.cs` — T004/T005 tests now compile but still fail
- [X] T009 [P] [US1] Extend `UpdateTripValidator.Validate` with ImageUrl rule: if non-null and non-empty, `Uri.TryCreate(url, UriKind.Absolute, out _)` must be true in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripValidator.cs` — T005 tests pass
- [X] T010 [US1] Extend `UpdateTripHandler.Handle` to apply `ImageUrl = request.ImageUrl ?? existing.ImageUrl` in the `updated = existing with { ... }` block in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripHandler.cs` — T004 tests pass
- [X] T011 [US1] Add `[Parameter] public WizardStep1Model? InitialModel { get; set; }` and `protected override void OnInitialized()` that copies all seven fields from `InitialModel` into the private `_model` instance in `CitytripPlanner.Web/Components/Citytrips/WizardStep1.razor` — T006 tests pass
- [X] T012 [P] [US1] Add `[Parameter] public string ConfirmLabel { get; set; } = "Confirm";` to `WizardStep3.razor` and use `@ConfirmLabel` as the confirm button text in `CitytripPlanner.Web/Components/Citytrips/WizardStep3.razor`
- [X] T013 [US1] Create `EditCitytrip.razor` at `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor` with: `@page "/citytrips/edit/{Id:int}"`, `@rendermode InteractiveServer`; `OnInitializedAsync` sends `GetCitytripDetailQuery(Id)`, sets `_notFound` / `_unauthorized` flags if null or wrong owner, maps `CitytripDetailResponse` to `_step1Model` (basics + ImageUrl); `HandleStep1Next` advances step (regenerates empty day slots on date change — no event preservation yet); `HandleConfirm` dispatches `UpdateTripCommand` with basics + `ImageUrl` + `DayPlans = null`; not-found/unauthorized guards before wizard markup; passes `InitialModel="_step1Model"` to WizardStep1 and `ConfirmLabel="Save Changes"` to WizardStep3 — T007 tests pass
- [X] T014 [P] [US1] Create `EditCitytrip.razor.css` at `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor.css` — copy `.create-wizard-page`, `.wizard-header`, `.wizard-body`, `.step-indicator`, `.step`, `.step-separator`, `.wizard-errors` from `CreateCitytrip.razor.css`
- [X] T015 [US1] Modify `MyCitytrips.razor` in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`: replace `OpenEditModal` body with `Navigation.NavigateTo($"/citytrips/edit/{trip.Id}")`, remove `_showFormModal` / `_editingTrip` fields, remove `HandleFormSave` / `CloseFormModal` methods, remove the `@if (_showFormModal) { <TripFormModal ... /> }` block, remove `@using CitytripPlanner.Features.Citytrips.UpdateTrip` import

**Checkpoint**: US1 complete — owner can open edit wizard from My Citytrips, see
basics pre-filled, edit title/image, and save. Verify via quickstart.md Scenario A.

---

## Phase 4: User Story 2 — Edit Day Plans and Events (Priority: P2)

**Goal**: Step 2 shows existing events pre-loaded per day; owner can change event
names, remove/add events, update places; Step 3 reflects changes; confirm saves
updated itinerary.

**Independent Test**: Edit a trip that has events → open Step 2 → verify events are
pre-filled → change one event name → advance to Step 3 → review shows updated name →
confirm → detail page shows updated event.

### Tests for User Story 2

> **Write ALL tests in this section FIRST. Confirm they FAIL before proceeding to implementation.**

- [X] T016 [P] [US2] Extend `UpdateTripHandlerTests` with three failing tests — `Handle_UpdatesDayPlansWhenProvided`, `Handle_PreservesExistingDayPlans_WhenDayPlansIsNull`, `Handle_UpdatesDayPlansWithPlace` — in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripHandlerTests.cs`
- [X] T017 [P] [US2] Extend `UpdateTripValidatorTests` with two failing tests — `Validate_EventEndTimeBeforeStartTime_ReturnsError` and `Validate_EventNullEndTime_IsValid` — in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripValidatorTests.cs`
- [X] T018 [US2] Extend `EditCitytripPageTests` with one failing test — `EditPage_LoadsTrip_PreFillsStep2Events` (after advancing to Step 2, event rows are rendered with existing event names) — in `CitytripPlanner.Tests/Web/Pages/EditCitytripPageTests.cs`

### Implementation for User Story 2

- [X] T019 [P] [US2] Extend `UpdateTripCommand` record with `List<DayPlanInput>? DayPlans = null` optional parameter at the end in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripCommand.cs` — T016/T017 tests now compile but still fail
- [X] T020 [US2] Extend `UpdateTripValidator.Validate` with event EndTime rule: for each event in each day plan where EndTime is not null, if `event.EndTime <= event.StartTime` add error `"End time must be after start time."` in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripValidator.cs` — T017 tests pass
- [X] T021 [US2] Extend `UpdateTripHandler.Handle` with private static `MapDayPlans` method (same logic as `CreateTripHandler`) and apply `DayPlans = request.DayPlans is not null ? MapDayPlans(request.DayPlans) : existing.DayPlans` in the `updated = existing with { ... }` block in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripHandler.cs` — T016 tests pass
- [X] T022 [US2] Extend `EditCitytrip.razor` in `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor`: in `OnInitializedAsync` map each `DayPlanDetail` → `DayPlanInputModel` (including `ScheduledEventDetail` → `ScheduledEventInputModel` and `PlaceDetail` → `PlaceInputModel`); if response has no DayPlans, call `PreserveByDayNumber([], start, end)` to generate empty slots; update `HandleConfirm` to include `DayPlans = dayPlanInputs` in the `UpdateTripCommand` — T018 test passes

**Checkpoint**: US2 complete — edit wizard pre-fills events and saves full itinerary.
Verify via quickstart.md Scenario B.

---

## Phase 5: User Story 3 — Date Range Change Warning (Priority: P3)

**Goal**: When the owner changes start/end date and events exist, a confirmation
prompt appears. Cancel restores original dates. Confirm rebuilds day slots by
day number (events on Day N stay on Day N; events on removed days are discarded).

**Independent Test**: Edit a trip with events → change end date → confirm warning →
Step 2 shows updated day count with Day 1/Day 2 events intact → shorten again →
Day 3 events gone.

### Tests for User Story 3

> **Write ALL tests in this section FIRST. Confirm they FAIL before proceeding to implementation.**

- [X] T023 [US3] Extend `EditCitytripPageTests` with four failing tests — `EditPage_DateChange_WithEvents_ShowsConfirmation`, `EditPage_DateChange_CancelRestoresOriginalDates`, `EditPage_DateChange_Confirm_PreservesEventsByDayNumber`, `EditPage_DateChange_ReduceLength_DiscardsTailDayEvents` — in `CitytripPlanner.Tests/Web/Pages/EditCitytripPageTests.cs`; use `bUnit.JSInterop.Setup<bool>("confirm", _ => true)` for confirm path and `Setup<bool>("confirm", _ => false)` for cancel path

### Implementation for User Story 3

- [X] T024 [US3] Extend `EditCitytrip.razor` in `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor`: add private static `PreserveByDayNumber(List<DayPlanInputModel> oldPlans, DateOnly newStart, DateOnly newEnd)` helper; update `HandleStep1Next` to detect date change (`_step1Model.StartDate != model.StartDate || _step1Model.EndDate != model.EndDate`), check `hadEvents = _dayPlans.Any(dp => dp.Events.Count > 0)`, show `await JS.InvokeAsync<bool>("confirm", "...")` when both true — cancel returns without updating, confirm calls `PreserveByDayNumber`; replace the simple `GenerateDayPlans` call in `OnInitializedAsync` with `PreserveByDayNumber([], start, end)` — T023 tests pass

**Checkpoint**: US3 complete — date change warning + day-number preservation works.
Verify via quickstart.md Scenarios C1, C2, C3.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T025 Run full test suite `dotnet test CitytripPlanner.Tests` from repo root and verify all tests pass (expect 0 failures); note any compilation errors and fix before proceeding
- [X] T026 [P] Verify quickstart.md Scenarios D (ownership guard) and E (not-found) in the browser — navigate to `/citytrips/edit/1` (seed trip owned by "seed-user") and `/citytrips/edit/9999`; confirm correct error messages render and no wizard form appears
- [X] T027 [P] Verify quickstart.md Scenario F (back-navigation preserves edits) in the browser — confirm Step 1 retains changed title after Back from Step 2

---

## Dependencies & Execution Order

### Phase Dependencies

- **Foundational (Phase 2)**: No dependencies — start immediately
- **US1 (Phase 3)**: Depends on Phase 2 (T001–T003) completion — BLOCKED until `CreatorId` is in response
- **US2 (Phase 4)**: Depends on US1 completion — `EditCitytrip.razor` must exist before extending it
- **US3 (Phase 5)**: Depends on US2 completion — `HandleStep1Next` is extended, not replaced
- **Polish (Phase 6)**: Depends on all user story phases

### User Story Dependencies

- **US1 (P1)**: Depends on Phase 2. No other story dependency.
- **US2 (P2)**: Depends on US1 — `EditCitytrip.razor` is created in US1 and extended in US2.
- **US3 (P3)**: Depends on US2 — `HandleStep1Next` extended; `_dayPlans` pre-fill must exist first.

### Within Each User Story

- TDD: ALL test tasks MUST be written and confirmed FAILING before any implementation task begins
- T004, T005, T006 [P]: can be written in parallel (different test files)
- T007: must be written after T006 (page tests reference WizardStep1)
- T008, T009 [P]: can be extended in parallel (different files in UpdateTrip slice)
- T011, T012 [P]: can be modified in parallel (different Razor components)
- T013 depends on T008, T010, T011, T012, and T002/T003 all being complete
- T014 can be written in parallel with T013 (CSS file, different file)
- T015 is independent of T013 once T002/T003 are done

---

## Parallel Execution Example: User Story 1

```text
# Step 1 — Write all US1 tests together (parallel):
Task T004: UpdateTripHandlerTests.cs — failing ImageUrl tests
Task T005: UpdateTripValidatorTests.cs — failing ImageUrl tests
Task T006: WizardStep1Tests.cs — failing InitialModel tests

# Step 2 — Write page test (depends on T006 completing first):
Task T007: EditCitytripPageTests.cs — failing page tests (4 tests)

# Step 3 — Implement command + validator in parallel:
Task T008: UpdateTripCommand.cs — add ImageUrl field
Task T009: UpdateTripValidator.cs — add ImageUrl rule

# Step 4 — Implement handler + component changes in parallel:
Task T010: UpdateTripHandler.cs — apply ImageUrl
Task T011: WizardStep1.razor — InitialModel parameter
Task T012: WizardStep3.razor — ConfirmLabel parameter

# Step 5 — Create page (depends on T010, T011, T012):
Task T013: EditCitytrip.razor — full page implementation
Task T014: EditCitytrip.razor.css — styles

# Step 6 — Wire up entry point:
Task T015: MyCitytrips.razor — Edit button navigation
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 2: Foundational (T001–T003)
2. Complete Phase 3: User Story 1 (T004–T015)
3. **STOP and VALIDATE**: Run `dotnet test` + quickstart.md Scenario A
4. Owners can already edit all basic trip fields — this is a shippable increment

### Incremental Delivery

1. Complete Phase 2 + Phase 3 → edit basics works → demo/validate
2. Add Phase 4 (US2) → day plan editing works → demo/validate
3. Add Phase 5 (US3) → date change safety warning works → demo/validate
4. Phase 6 → polish and full test run

---

## Notes

- `[P]` tasks touch different files and have no pending dependencies — safe to run in parallel
- Every implementation task MUST be preceded by a confirmed-failing test (TDD, non-negotiable per constitution)
- `EditCitytrip.razor` is built incrementally across US1 → US2 → US3; do NOT implement day preservation in US1
- `UpdateTripCommand` record is extended twice (ImageUrl in T008, DayPlans in T019); both are optional params — no breaking change
- `CitytripDetail.razor` positional constructor call will break when T002 adds `CreatorId` — fix it in T003
- For bUnit page tests (T007, T018, T023): use `Services.AddSingleton<IMediator>(Substitute.For<IMediator>())` and configure mock returns; use `Services.AddSingleton<ICurrentUserService>(stub)` where stub returns "test-user"
- Mark tasks `[X]` as they complete
