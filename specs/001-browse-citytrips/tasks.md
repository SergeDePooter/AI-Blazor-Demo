# Tasks: Browse Citytrips

**Input**: Design documents from `/specs/001-browse-citytrips/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Included — constitution mandates TDD (Principle II: Test-Driven Development, NON-NEGOTIABLE).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

- **Web project**: `CitytripPlanner/CitytripPlanner.Web/`
- **Features project**: `CitytripPlanner/CitytripPlanner.Features/`
- **Infrastructure project**: `CitytripPlanner/CitytripPlanner.Infrastructure/`
- **Test project**: `CitytripPlanner/CitytripPlanner.Tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create test project, install NuGet packages, configure DI

- [x] T001 Create xUnit test project `CitytripPlanner/CitytripPlanner.Tests/CitytripPlanner.Tests.csproj` with references to Features, Infrastructure, and Web projects
- [x] T002 Install MediatR NuGet package in `CitytripPlanner/CitytripPlanner.Features/CitytripPlanner.Features.csproj`
- [x] T003 [P] Install bUnit NuGet package in `CitytripPlanner/CitytripPlanner.Tests/CitytripPlanner.Tests.csproj`
- [x] T004 Add project reference from Web to Features in `CitytripPlanner/CitytripPlanner.Web/CitytripPlanner.Web.csproj`
- [x] T005 [P] Add project reference from Infrastructure to Features in `CitytripPlanner/CitytripPlanner.Infrastructure/CitytripPlanner.Infrastructure.csproj`
- [x] T006 Add project reference from Web to Infrastructure in `CitytripPlanner/CitytripPlanner.Web/CitytripPlanner.Web.csproj`
- [x] T007 Define CSS custom property `--color-orange` and `--color-orange-75` in `CitytripPlanner/CitytripPlanner.Web/wwwroot/app.css`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain entities, interfaces, and infrastructure implementations that ALL user stories depend on

**CRITICAL**: No user story work can begin until this phase is complete

### Tests for Foundational Phase

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T008 [P] Write unit test for InMemoryCitytripRepository.GetAllAsync returns seed data in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Infrastructure/InMemoryCitytripRepositoryTests.cs`
- [x] T009 [P] Write unit test for InMemoryCitytripRepository.GetByIdAsync returns correct trip in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Infrastructure/InMemoryCitytripRepositoryTests.cs`
- [x] T010 [P] Write unit test for InMemoryUserInteractionStore get/save interaction roundtrip in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Infrastructure/InMemoryUserInteractionStoreTests.cs`

### Implementation for Foundational Phase

- [x] T011 [P] Create Citytrip record in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs` with Id, CityName, ImageUrl, DurationInDays, Description fields
- [x] T012 [P] Create UserTripInteraction class in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Domain/UserTripInteraction.cs` with CitytripId, IsLiked, IsEnlisted fields
- [x] T013 [P] Create ICitytripRepository interface in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs` with GetAllAsync and GetByIdAsync methods
- [x] T014 [P] Create IUserInteractionStore interface in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Domain/IUserInteractionStore.cs` with GetInteraction, GetAllInteractions, SaveInteraction methods
- [x] T015 Implement InMemoryCitytripRepository (singleton) with seed data (Paris, Barcelona, Rome, Amsterdam, Prague, Lisbon) in `CitytripPlanner/CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs`
- [x] T016 [P] Implement InMemoryUserInteractionStore (scoped) in `CitytripPlanner/CitytripPlanner.Infrastructure/Citytrips/InMemoryUserInteractionStore.cs`
- [x] T017 Register MediatR, ICitytripRepository (singleton), and IUserInteractionStore (scoped) in DI in `CitytripPlanner/CitytripPlanner.Web/Program.cs`
- [x] T018 Verify all foundational tests pass by running `dotnet test CitytripPlanner/CitytripPlanner.Tests`

**Checkpoint**: Foundation ready — domain entities, interfaces, and in-memory implementations all in place. User story work can begin.

---

## Phase 3: User Story 1 — Browse Available Citytrips (Priority: P1) MVP

**Goal**: Display citytrips in a masonry grid with image, city name, duration, like button, and enlist button

**Independent Test**: Navigate to main page and verify citytrip cards render in masonry layout with all required fields. Verify empty-state message when no trips available.

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T019 [P] [US1] Write unit test for ListCitytripsHandler returns all trips merged with interaction state in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/ListCitytrips/ListCitytripsHandlerTests.cs`
- [x] T020 [P] [US1] Write unit test for ListCitytripsHandler returns empty list when no trips exist in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/ListCitytrips/ListCitytripsHandlerTests.cs`
- [x] T021 [P] [US1] Write bUnit test for CitytripCard component renders image, city name, duration, like button, and enlist button in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Components/CitytripCardTests.cs`
- [x] T022 [P] [US1] Write bUnit test for CitytripCard displays fallback placeholder when image fails to load in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Components/CitytripCardTests.cs`

### Implementation for User Story 1

- [x] T023 [P] [US1] Create CitytripCard response DTO in `CitytripPlanner/CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripCard.cs` with Id, CityName, ImageUrl, DurationInDays, IsLiked, IsEnlisted
- [x] T024 [P] [US1] Create ListCitytripsQuery request in `CitytripPlanner/CitytripPlanner.Features/Citytrips/ListCitytrips/ListCitytripsQuery.cs`
- [x] T025 [US1] Implement ListCitytripsHandler in `CitytripPlanner/CitytripPlanner.Features/Citytrips/ListCitytrips/ListCitytripsHandler.cs` — retrieve all trips, merge with session interactions, return List<CitytripCard>
- [x] T026 [US1] Create CitytripCard.razor component in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/CitytripCard.razor` — render image (with onerror fallback), city name, "{N} days" duration, like button, enlist button
- [x] T027 [US1] Style CitytripCard with scoped CSS in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/CitytripCard.razor.css` — card layout, image sizing, button placement
- [x] T028 [US1] Create Citytrips.razor page in `CitytripPlanner/CitytripPlanner.Web/Components/Pages/Citytrips.razor` — dispatch ListCitytripsQuery via MediatR, render masonry grid of CitytripCard components, show empty-state message when list is empty
- [x] T029 [US1] Style masonry layout using CSS columns in `CitytripPlanner/CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`
- [x] T030 [US1] Verify all US1 tests pass by running `dotnet test CitytripPlanner/CitytripPlanner.Tests --filter "FullyQualifiedName~ListCitytrips|FullyQualifiedName~CitytripCard"`

**Checkpoint**: Citytrip browse page is functional with masonry grid. Cards display all required information. Empty state handled.

---

## Phase 4: User Story 2 — Navigate Using Horizontal Menu (Priority: P1)

**Goal**: Replace default Blazor sidebar with horizontal text-only navigation using orange styling

**Independent Test**: Load any page and verify: horizontal menu at top, text-only items, selected = orange + underline, non-selected = orange at 75% opacity, no left sidebar.

### Tests for User Story 2

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T031 [P] [US2] Write bUnit test for NavMenu renders horizontal list of text-only navigation links in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Components/NavMenuTests.cs`
- [x] T032 [P] [US2] Write bUnit test for NavMenu applies active CSS class to selected page link in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Components/NavMenuTests.cs`

### Implementation for User Story 2

- [x] T033 [US2] Replace NavMenu.razor with horizontal text-only navigation in `CitytripPlanner/CitytripPlanner.Web/Components/Layout/NavMenu.razor` — remove icons/SVGs, render flat list of NavLink items horizontally
- [x] T034 [US2] Style NavMenu with orange colors in `CitytripPlanner/CitytripPlanner.Web/Components/Layout/NavMenu.razor.css` — selected: orange + underline, non-selected: orange at 75% opacity, no icons, horizontal layout
- [x] T035 [US2] Replace MainLayout.razor to remove sidebar in `CitytripPlanner/CitytripPlanner.Web/Components/Layout/MainLayout.razor` — remove sidebar div, render NavMenu at top, full-width main content area below
- [x] T036 [US2] Update MainLayout scoped CSS in `CitytripPlanner/CitytripPlanner.Web/Components/Layout/MainLayout.razor.css` — remove sidebar styles, add top nav layout, full-width content
- [x] T037 [US2] Remove Counter.razor and Weather.razor default pages from `CitytripPlanner/CitytripPlanner.Web/Components/Pages/` and update nav links to point to Citytrips page
- [x] T038 [US2] Verify all US2 tests pass by running `dotnet test CitytripPlanner/CitytripPlanner.Tests --filter "FullyQualifiedName~NavMenu"`

**Checkpoint**: Navigation is horizontal, text-only, with correct orange styling. No sidebar present. Menu correctly highlights active page.

---

## Phase 5: User Story 3 — Like a Citytrip (Priority: P2)

**Goal**: Toggle like state on a citytrip card with visual feedback

**Independent Test**: Click like button on a trip card, verify visual toggle. Click again, verify unlike.

### Tests for User Story 3

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T039 [P] [US3] Write unit test for ToggleLikeHandler toggles IsLiked from false to true in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/ToggleLike/ToggleLikeHandlerTests.cs`
- [x] T040 [P] [US3] Write unit test for ToggleLikeHandler toggles IsLiked from true to false in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/ToggleLike/ToggleLikeHandlerTests.cs`
- [x] T041 [P] [US3] Write unit test for ToggleLikeHandler returns error for non-existent CitytripId in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/ToggleLike/ToggleLikeHandlerTests.cs`

### Implementation for User Story 3

- [x] T042 [P] [US3] Create ToggleLikeCommand request in `CitytripPlanner/CitytripPlanner.Features/Citytrips/ToggleLike/ToggleLikeCommand.cs` with CitytripId property
- [x] T043 [US3] Implement ToggleLikeHandler in `CitytripPlanner/CitytripPlanner.Features/Citytrips/ToggleLike/ToggleLikeHandler.cs` — look up/create interaction, toggle IsLiked, save, return new state
- [x] T044 [US3] Wire like button in CitytripCard.razor to dispatch ToggleLikeCommand via MediatR and update visual state with debounce in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/CitytripCard.razor`
- [x] T045 [US3] Verify all US3 tests pass by running `dotnet test CitytripPlanner/CitytripPlanner.Tests --filter "FullyQualifiedName~ToggleLike"`

**Checkpoint**: Like button toggles correctly. Visual state reflects liked/unliked. Debounce prevents double-clicks.

---

## Phase 6: User Story 4 — Enlist for a Citytrip (Priority: P2)

**Goal**: Enlist for a citytrip with toast notification confirmation

**Independent Test**: Click enlist button, verify toast appears and button shows enlisted state. Verify already-enlisted state persists within session.

### Tests for User Story 4

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T046 [P] [US4] Write unit test for EnlistHandler sets IsEnlisted to true in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Enlist/EnlistHandlerTests.cs`
- [x] T047 [P] [US4] Write unit test for EnlistHandler returns true when already enlisted (idempotent) in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Enlist/EnlistHandlerTests.cs`
- [x] T048 [P] [US4] Write unit test for EnlistHandler returns error for non-existent CitytripId in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Enlist/EnlistHandlerTests.cs`
- [x] T049 [P] [US4] Write bUnit test for Toast component renders message and auto-dismisses in `CitytripPlanner/CitytripPlanner.Tests/Citytrips/Components/ToastTests.cs`

### Implementation for User Story 4

- [x] T050 [P] [US4] Create EnlistCommand request in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Enlist/EnlistCommand.cs` with CitytripId property
- [x] T051 [US4] Implement EnlistHandler in `CitytripPlanner/CitytripPlanner.Features/Citytrips/Enlist/EnlistHandler.cs` — look up/create interaction, set IsEnlisted = true, save, return state
- [x] T052 [US4] Create Toast.razor component in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/Toast.razor` — render message at fixed position, auto-dismiss after configurable duration via Task.Delay + StateHasChanged
- [x] T053 [US4] Style Toast component in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/Toast.razor.css` — fixed bottom-right position, fade-in/out animation, orange accent
- [x] T054 [US4] Wire enlist button in CitytripCard.razor to dispatch EnlistCommand, show toast on success, update button to enlisted state with debounce in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/CitytripCard.razor`
- [x] T055 [US4] Add Toast component to MainLayout.razor for page-level toast rendering in `CitytripPlanner/CitytripPlanner.Web/Components/Layout/MainLayout.razor`
- [x] T056 [US4] Verify all US4 tests pass by running `dotnet test CitytripPlanner/CitytripPlanner.Tests --filter "FullyQualifiedName~Enlist|FullyQualifiedName~Toast"`

**Checkpoint**: Enlist button works, toast confirms enlistment, button reflects enlisted state. Idempotent on re-click.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, cleanup, and edge case handling

- [x] T057 [P] Add image onerror fallback placeholder handling across all CitytripCard instances in `CitytripPlanner/CitytripPlanner.Web/Components/Shared/CitytripCard.razor`
- [x] T058 Run full test suite with `dotnet test CitytripPlanner/CitytripPlanner.Tests` and verify all tests pass
- [x] T059 Run quickstart.md validation — follow steps in `specs/001-browse-citytrips/quickstart.md` to verify end-to-end flow
- [x] T060 [P] Code cleanup: remove unused default template files (Counter.razor.css, Weather.razor.css if present) from `CitytripPlanner/CitytripPlanner.Web/Components/Pages/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — BLOCKS all user stories
- **US1 Browse (Phase 3)**: Depends on Foundational phase completion
- **US2 Navigation (Phase 4)**: Depends on Foundational phase completion. Can run in parallel with US1.
- **US3 Like (Phase 5)**: Depends on Foundational phase. Shares CitytripCard.razor with US1, so best done after US1.
- **US4 Enlist (Phase 6)**: Depends on Foundational phase. Shares CitytripCard.razor with US1, so best done after US1.
- **Polish (Phase 7)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: After Phase 2 — no story dependencies
- **User Story 2 (P1)**: After Phase 2 — no story dependencies, can run parallel with US1
- **User Story 3 (P2)**: After Phase 2 + US1 (shares CitytripCard.razor)
- **User Story 4 (P2)**: After Phase 2 + US1 (shares CitytripCard.razor)

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Domain types/DTOs before handlers
- Handlers before UI components
- Components before page integration
- Story complete before moving to next priority

### Parallel Opportunities

- T002 + T003: NuGet installs in parallel
- T004 + T005: Project references in parallel
- T008 + T009 + T010: All foundational tests in parallel
- T011 + T012 + T013 + T014: All domain types in parallel
- T015 + T016: Both infrastructure implementations in parallel
- T019 + T020 + T021 + T022: All US1 tests in parallel
- T023 + T024: US1 DTO + Query in parallel
- T031 + T032: All US2 tests in parallel
- T039 + T040 + T041: All US3 tests in parallel
- T046 + T047 + T048 + T049: All US4 tests in parallel
- US1 + US2: Can be developed in parallel (different files)

---

## Parallel Example: User Story 1

```bash
# Launch all tests for US1 together (TDD — write first, must FAIL):
Task: T019 "Unit test for ListCitytripsHandler returns all trips"
Task: T020 "Unit test for ListCitytripsHandler returns empty list"
Task: T021 "bUnit test for CitytripCard renders all fields"
Task: T022 "bUnit test for CitytripCard fallback image"

# Launch DTO + Query in parallel:
Task: T023 "Create CitytripCard DTO"
Task: T024 "Create ListCitytripsQuery request"

# Then sequentially:
Task: T025 "Implement ListCitytripsHandler" (depends on T023, T024)
Task: T026 "Create CitytripCard.razor" (depends on T025)
Task: T027 "Style CitytripCard.razor.css" (depends on T026)
Task: T028 "Create Citytrips.razor page" (depends on T026)
Task: T029 "Style masonry layout" (depends on T028)
```

---

## Implementation Strategy

### MVP First (User Stories 1 + 2 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1 (Browse)
4. Complete Phase 4: User Story 2 (Navigation)
5. **STOP and VALIDATE**: Browse page with correct layout and navigation
6. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add US1 (Browse) + US2 (Navigation) → Test independently → Deploy/Demo (MVP!)
3. Add US3 (Like) → Test independently → Deploy/Demo
4. Add US4 (Enlist + Toast) → Test independently → Deploy/Demo
5. Polish phase → Final validation → Done

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (Browse)
   - Developer B: User Story 2 (Navigation)
3. After US1 completes:
   - Developer A: User Story 3 (Like)
   - Developer B: User Story 4 (Enlist)
4. Both do Polish together

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- TDD is NON-NEGOTIABLE per constitution — write tests first, verify they fail, then implement
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
