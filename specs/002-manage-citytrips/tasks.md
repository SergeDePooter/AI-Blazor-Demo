# Tasks: Manage Citytrips

**Input**: Design documents from `/specs/002-manage-citytrips/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Included — TDD is NON-NEGOTIABLE per project constitution (Principle II).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup

**Purpose**: Introduce shared infrastructure needed by all user stories

- [x] T001 Create `ICurrentUserService` interface with `UserId` and `DisplayName` properties in `CitytripPlanner.Features/Citytrips/Domain/ICurrentUserService.cs`
- [x] T002 Create `InMemoryCurrentUserService` implementing `ICurrentUserService` with hardcoded demo user (UserId: "demo-user", DisplayName: "Demo User") in `CitytripPlanner.Infrastructure/Citytrips/InMemoryCurrentUserService.cs`
- [x] T003 Register `ICurrentUserService` as singleton in DI container in `CitytripPlanner.Web/Program.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Evolve the Citytrip domain model and repository to support CRUD and per-user ownership. MUST complete before any user story.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Tests (TDD: write first, ensure they fail)

- [x] T004 [P] Write unit tests for updated `InMemoryCitytripRepository` covering `Add`, `Update`, `Delete`, and `GetByCreator` methods in `CitytripPlanner.Tests/Citytrips/Infrastructure/InMemoryCitytripRepositoryTests.cs`

### Implementation

- [x] T005 Update `Citytrip` record to new shape: replace `CityName` with `Title` + `Destination`, replace `DurationInDays` with `StartDate` (DateOnly) + `EndDate` (DateOnly), add `MaxParticipants` (int?), add `CreatorId` (string) in `CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs`
- [x] T006 Update `ICitytripRepository` interface: add `Add(Citytrip)`, `Update(Citytrip)`, `Delete(int)`, `GetByCreator(string creatorId)` methods in `CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs`
- [x] T007 Update `InMemoryCitytripRepository` to implement new CRUD methods, update seed data to use new field names (Title, Destination, StartDate, EndDate, CreatorId assigned to "seed-user"), switch internal storage to `ConcurrentDictionary` with auto-increment ID in `CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs`
- [x] T008 Update `CitytripCard` response DTO to use new field names (Title, Destination, StartDate, EndDate instead of CityName, DurationInDays) in `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripCard.cs`
- [x] T009 Update `ListCitytripsHandler` to map new Citytrip fields to updated CitytripCard DTO in `CitytripPlanner.Features/Citytrips/ListCitytrips/ListCitytripsHandler.cs`
- [x] T010 Update `TripCard.razor` component to display Title, Destination, StartDate/EndDate instead of CityName and DurationInDays in `CitytripPlanner.Web/Components/Shared/TripCard.razor`
- [x] T011 Fix all existing tests that reference old field names (CityName, DurationInDays) to use new names (Title, Destination, StartDate, EndDate) in `CitytripPlanner.Tests/Citytrips/`
- [x] T012 Verify all existing tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: Foundation ready — domain model evolved, existing feature 001 still works, repository supports CRUD. User story implementation can begin.

---

## Phase 3: User Story 1 — View My Created Citytrips (Priority: P1) 🎯 MVP

**Goal**: Users navigate to "My Citytrips" page and see a list of their own created citytrips with key details. Page defaults to this section.

**Independent Test**: Navigate to `/my-citytrips`, verify only the current user's created trips are displayed with title, destination, and dates.

### Tests (TDD: write first, ensure they fail)

- [x] T013 [P] [US1] Write unit tests for `GetMyTripsHandler`: returns only trips matching CreatorId, returns empty list when no trips, maps all fields to `MyTripItem` correctly in `CitytripPlanner.Tests/Citytrips/GetMyTrips/GetMyTripsHandlerTests.cs`

### Implementation

- [x] T014 [P] [US1] Create `MyTripItem` response DTO record (Id, Title, Destination, ImageUrl, StartDate, EndDate, Description, MaxParticipants, EnlistedCount) in `CitytripPlanner.Features/Citytrips/GetMyTrips/MyTripItem.cs`
- [x] T015 [P] [US1] Create `GetMyTripsQuery` MediatR request with `CreatorId` property in `CitytripPlanner.Features/Citytrips/GetMyTrips/GetMyTripsQuery.cs`
- [x] T016 [US1] Create `GetMyTripsHandler` that queries repository by CreatorId and maps to `MyTripItem` list (include EnlistedCount from UserInteractionStore) in `CitytripPlanner.Features/Citytrips/GetMyTrips/GetMyTripsHandler.cs`
- [x] T017 [US1] Create `MyCitytrips.razor` page at route `/my-citytrips` with tab toggle UI (enum `ActiveSection { MyTrips, EnlistedTrips }`), default to MyTrips section, load and display created trips via `GetMyTripsQuery` using MediatR, show empty state when no trips exist in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`
- [x] T018 [US1] Create `MyCitytrips.razor.css` with scoped styles for tab toggle, trip list cards, and empty state in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor.css`
- [x] T019 [US1] Add "My Citytrips" NavLink to navigation menu in `CitytripPlanner.Web/Components/Layout/NavMenu.razor`
- [x] T020 [US1] Verify tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: User Story 1 complete — users can view their created citytrips on a dedicated page with tab UI. MVP deliverable.

---

## Phase 4: User Story 6 — Toggle Between Sections (Priority: P1)

**Goal**: Users can switch between "My Citytrips" and "Enlisted Citytrips" tabs. Only the selected section is visible at a time. Active tab is clearly indicated.

**Independent Test**: Click between the two tabs, verify only one section's content is visible and the active tab is visually highlighted.

### Tests (TDD: write first, ensure they fail)

- [x] T021 [P] [US6] Write unit tests for `GetEnlistedTripsHandler`: returns only trips where user is enlisted but not creator, returns empty list when no enlisted trips, maps all fields to `EnlistedTripItem` correctly in `CitytripPlanner.Tests/Citytrips/GetEnlistedTrips/GetEnlistedTripsHandlerTests.cs`

### Implementation

- [x] T022 [P] [US6] Create `EnlistedTripItem` response DTO record (Id, Title, Destination, ImageUrl, StartDate, EndDate, Description, CreatorName) in `CitytripPlanner.Features/Citytrips/GetEnlistedTrips/EnlistedTripItem.cs`
- [x] T023 [P] [US6] Create `GetEnlistedTripsQuery` MediatR request with `UserId` property in `CitytripPlanner.Features/Citytrips/GetEnlistedTrips/GetEnlistedTripsQuery.cs`
- [x] T024 [US6] Create `GetEnlistedTripsHandler` that queries UserInteractionStore for enlisted trip IDs, fetches matching trips from repository (excluding trips created by user), maps to `EnlistedTripItem` list in `CitytripPlanner.Features/Citytrips/GetEnlistedTrips/GetEnlistedTripsHandler.cs`
- [x] T025 [US6] Update `MyCitytrips.razor` to load enlisted trips via `GetEnlistedTripsQuery` when "Enlisted" tab is selected, display enlisted trips in read-only list (no edit/delete buttons), show empty state when no enlisted trips in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`
- [x] T026 [US6] Update `MyCitytrips.razor.css` with styles for enlisted trip cards (read-only appearance) in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor.css`
- [x] T027 [US6] Verify tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: User Stories 1 and 6 complete — page shows both tabs with full toggle functionality. Enlisted section shows read-only data (also completes US5 view).

---

## Phase 5: User Story 2 — Create a New Citytrip (Priority: P1)

**Goal**: Users can create a new citytrip via a modal dialog with all required fields. The new trip appears in the "My Citytrips" list.

**Independent Test**: Click "Create Citytrip", fill in the form, submit, verify the new trip appears in the list. Submit with empty required fields, verify validation messages.

### Tests (TDD: write first, ensure they fail)

- [x] T028 [P] [US2] Write unit tests for `CreateTripHandler`: creates trip with valid data, returns new ID, assigns CreatorId in `CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripHandlerTests.cs`
- [x] T029 [P] [US2] Write unit tests for `CreateTripValidator`: rejects empty title, empty destination, EndDate before StartDate, MaxParticipants < 1, accepts valid input in `CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripValidatorTests.cs`

### Implementation

- [x] T030 [P] [US2] Create `CreateTripCommand` MediatR request (Title, Destination, StartDate, EndDate, Description, MaxParticipants, CreatorId) returning `int` in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripCommand.cs`
- [x] T031 [P] [US2] Create `CreateTripValidator` with validation rules: Title required (max 100), Destination required (max 100), EndDate >= StartDate, MaxParticipants >= 1 if provided in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripValidator.cs`
- [x] T032 [US2] Create `CreateTripHandler` that validates input, creates Citytrip via repository `Add`, returns new trip ID in `CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripHandler.cs`
- [x] T033 [US2] Create `TripFormModal.razor` component: modal overlay with form fields (Title, Destination, StartDate, EndDate, Description, MaxParticipants), save/cancel buttons, validation error display, `[Parameter]` for existing trip data (null for create), `EventCallback<CreateTripCommand>` for save in `CitytripPlanner.Web/Components/Shared/TripFormModal.razor`
- [x] T034 [US2] Create `TripFormModal.razor.css` with modal overlay styles (position: fixed, z-index, backdrop, centered form card) in `CitytripPlanner.Web/Components/Shared/TripFormModal.razor.css`
- [x] T035 [US2] Update `MyCitytrips.razor` to add "Create Citytrip" button, wire up TripFormModal opening/closing, dispatch CreateTripCommand via MediatR on save, refresh trip list after creation in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`
- [x] T036 [US2] Verify tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: User Stories 1, 6, and 2 complete — users can view and create citytrips.

---

## Phase 6: User Story 3 — Edit an Existing Citytrip (Priority: P2)

**Goal**: Users can edit a citytrip they created via a modal pre-filled with current values. Changes are saved and reflected in the list.

**Independent Test**: Click edit on an existing trip, modify fields, save, verify changes are reflected. Cancel edit, verify no changes.

### Tests (TDD: write first, ensure they fail)

- [x] T037 [P] [US3] Write unit tests for `UpdateTripHandler`: updates trip with valid data, rejects if trip not found, rejects if UserId != CreatorId in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripHandlerTests.cs`
- [x] T038 [P] [US3] Write unit tests for `UpdateTripValidator`: same rules as CreateTripValidator plus TripId required in `CitytripPlanner.Tests/Citytrips/UpdateTrip/UpdateTripValidatorTests.cs`

### Implementation

- [x] T039 [P] [US3] Create `UpdateTripCommand` MediatR request (TripId, Title, Destination, StartDate, EndDate, Description, MaxParticipants, UserId) returning `bool` in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripCommand.cs`
- [x] T040 [P] [US3] Create `UpdateTripValidator` with validation rules: same as Create plus TripId > 0 in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripValidator.cs`
- [x] T041 [US3] Create `UpdateTripHandler` that validates input, verifies trip exists and UserId matches CreatorId, updates via repository `Update` in `CitytripPlanner.Features/Citytrips/UpdateTrip/UpdateTripHandler.cs`
- [x] T042 [US3] Update `TripFormModal.razor` to support edit mode: accept existing `MyTripItem` as parameter, pre-fill form fields, dispatch `UpdateTripCommand` instead of `CreateTripCommand` when editing in `CitytripPlanner.Web/Components/Shared/TripFormModal.razor`
- [x] T043 [US3] Update `MyCitytrips.razor` to add edit button on each trip card in "My Citytrips" section, wire up TripFormModal in edit mode, refresh list after save in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`
- [x] T044 [US3] Verify tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: User Stories 1, 6, 2, and 3 complete — users can view, create, and edit citytrips.

---

## Phase 7: User Story 4 — Delete a Citytrip (Priority: P2)

**Goal**: Users can delete a citytrip they created with a confirmation dialog. The trip is removed from the list.

**Independent Test**: Click delete on a trip, confirm, verify it disappears. Cancel deletion, verify trip remains.

### Tests (TDD: write first, ensure they fail)

- [x] T045 [P] [US4] Write unit tests for `DeleteTripHandler`: deletes trip with valid data, rejects if trip not found, rejects if UserId != CreatorId in `CitytripPlanner.Tests/Citytrips/DeleteTrip/DeleteTripHandlerTests.cs`

### Implementation

- [x] T046 [P] [US4] Create `DeleteTripCommand` MediatR request (TripId, UserId) returning `bool` in `CitytripPlanner.Features/Citytrips/DeleteTrip/DeleteTripCommand.cs`
- [x] T047 [US4] Create `DeleteTripHandler` that verifies trip exists and UserId matches CreatorId, deletes via repository `Delete` in `CitytripPlanner.Features/Citytrips/DeleteTrip/DeleteTripHandler.cs`
- [x] T048 [US4] Create `DeleteConfirmModal.razor` component: modal overlay with trip name, warning message, confirm/cancel buttons, `EventCallback` for confirm in `CitytripPlanner.Web/Components/Shared/DeleteConfirmModal.razor`
- [x] T049 [US4] Create `DeleteConfirmModal.razor.css` with confirmation dialog styles in `CitytripPlanner.Web/Components/Shared/DeleteConfirmModal.razor.css`
- [x] T050 [US4] Update `MyCitytrips.razor` to add delete button on each trip card in "My Citytrips" section, wire up DeleteConfirmModal opening/closing, dispatch DeleteTripCommand via MediatR on confirm, refresh list after deletion in `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor`
- [x] T051 [US4] Verify tests pass with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`

**Checkpoint**: All user stories complete — full CRUD for own trips, read-only view of enlisted trips, tab toggle.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final quality pass across all stories

- [x] T052 Run full test suite and fix any regressions with `dotnet test` from `CitytripPlanner/CitytripPlanner.Tests/`
- [x] T053 Verify feature 001 browse page (`/` and `/citytrips`) still works correctly with evolved domain model
- [x] T054 Run quickstart.md validation: start app, navigate to `/my-citytrips`, verify both tabs, create/edit/delete a trip, check enlisted section

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — BLOCKS all user stories
- **US1 (Phase 3)**: Depends on Phase 2 — MVP target
- **US6 (Phase 4)**: Depends on Phase 3 (needs page skeleton with tab UI)
- **US2 (Phase 5)**: Depends on Phase 3 (needs page to place create button)
- **US3 (Phase 6)**: Depends on Phase 5 (reuses TripFormModal in edit mode)
- **US4 (Phase 7)**: Depends on Phase 3 (needs page with trip list)
- **Polish (Phase 8)**: Depends on all story phases

### User Story Dependencies

- **US1 (View My Trips)**: Foundation only — first to implement
- **US6 (Toggle Tabs)**: Depends on US1 (page must exist with tab skeleton)
- **US2 (Create Trip)**: Depends on US1 (page must exist to place create button)
- **US3 (Edit Trip)**: Depends on US2 (reuses TripFormModal component)
- **US4 (Delete Trip)**: Depends on US1 (page must exist with trip list)
- **US5 (View Enlisted)**: Delivered as part of US6 (enlisted data loaded when tab switches)

### Within Each User Story

- Tests MUST be written and FAIL before implementation (TDD - Constitution Principle II)
- DTOs/Commands before Handlers
- Handlers before Blazor components
- Core implementation before UI integration

### Parallel Opportunities

- T014, T015 can run in parallel (different files, no dependencies)
- T022, T023 can run in parallel
- T028, T029 can run in parallel
- T030, T031 can run in parallel
- T037, T038 can run in parallel
- T039, T040 can run in parallel
- US4 (Phase 7) can run in parallel with US3 (Phase 6) since they don't share files

---

## Parallel Example: User Story 2

```bash
# Launch tests in parallel (TDD: write first):
Task: T028 "Unit tests for CreateTripHandler in CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripHandlerTests.cs"
Task: T029 "Unit tests for CreateTripValidator in CitytripPlanner.Tests/Citytrips/CreateTrip/CreateTripValidatorTests.cs"

# Launch DTOs/Commands in parallel:
Task: T030 "CreateTripCommand in CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripCommand.cs"
Task: T031 "CreateTripValidator in CitytripPlanner.Features/Citytrips/CreateTrip/CreateTripValidator.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T003)
2. Complete Phase 2: Foundational (T004–T012)
3. Complete Phase 3: User Story 1 (T013–T020)
4. **STOP and VALIDATE**: Navigate to `/my-citytrips`, verify trips display
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US1 (View My Trips) → Test → Demo (MVP!)
3. Add US6 (Toggle Tabs) + US5 (Enlisted View) → Test → Demo
4. Add US2 (Create Trip) → Test → Demo
5. Add US3 (Edit Trip) → Test → Demo
6. Add US4 (Delete Trip) → Test → Demo
7. Polish → Final validation

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- TDD is enforced per Constitution Principle II — every handler has tests written first
- US5 (View Enlisted Trips) is delivered within US6 (Toggle Between Sections) since the enlisted list is loaded when the tab switches
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
