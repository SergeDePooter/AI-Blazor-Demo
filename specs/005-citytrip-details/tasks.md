---

description: "Task list for citytrip detail view implementation"
---

# Tasks: Citytrip Detail View

**Input**: Design documents from `/specs/005-citytrip-details/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/GetCitytripDetailContract.md

**Tests**: TDD is MANDATORY per project constitution (Principle II). All test tasks MUST be completed and FAILING before implementation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

Project follows Clean Architecture with three main projects:
- **CitytripPlanner.Features/**: Application logic, CQRS handlers, domain models
- **CitytripPlanner.Web/**: Blazor pages and components
- **CitytripPlanner.Infrastructure/**: Data access, repositories
- **CitytripPlanner.Tests/**: Unit and integration tests

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Verify project structure and ensure all prerequisites are in place

- [x] T001 Verify .NET 10 SDK and dependencies installed
- [x] T002 Verify MediatR 14.x is registered in CitytripPlanner.Web/Program.cs
- [x] T003 [P] Verify bUnit 2.5.3 and xUnit test infrastructure in CitytripPlanner.Tests
- [x] T004 Run existing tests to ensure baseline passes

**Checkpoint**: Development environment ready

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core domain models and repository infrastructure that ALL user stories depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Models (Parallel)

- [x] T005 [P] Create DayPlan domain model in CitytripPlanner.Features/Citytrips/Domain/DayPlan.cs with validation (DayNumber > 0, Attractions not null)
- [x] T006 [P] Write unit tests for DayPlan validation in CitytripPlanner.Tests/Features/Citytrips/Domain/DayPlanTests.cs
- [x] T007 [P] Create Attraction domain model in CitytripPlanner.Features/Citytrips/Domain/Attraction.cs with validation (Name required, WebsiteUrl valid URI if present)
- [x] T008 [P] Write unit tests for Attraction validation in CitytripPlanner.Tests/Features/Citytrips/Domain/AttractionTests.cs
- [x] T009 Extend Citytrip domain model in CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs to add DayPlans property (List<DayPlan>? DayPlans = null)

### Repository Layer

- [x] T010 Extend ICitytripRepository interface in CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs with GetByIdWithItineraryAsync(int id) method
- [x] T011 Implement GetByIdWithItineraryAsync in CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs
- [x] T012 Add seed data with sample itinerary (Paris trip with 2-3 day plans including attractions and transportation) to InMemoryCitytripRepository constructor
- [x] T013 Write repository tests in CitytripPlanner.Tests/Infrastructure/InMemoryCitytripRepositoryTests.cs to verify itinerary data is returned

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - View Citytrip Details (Priority: P1) 🎯 MVP

**Goal**: Enable users to navigate from the citytrip list to a detail page showing basic trip information (title, destination, dates, photo, description)

**Independent Test**: Click any citytrip card in the main list and verify all basic information appears on the detail page. Should work even without itinerary data.

### Tests for User Story 1 (TDD REQUIRED) ⚠️

> **CRITICAL: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T014 [P] [US1] Write handler unit test for valid citytrip ID in CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetailTests.cs (mock repository, verify response mapping)
- [ ] T015 [P] [US1] Write handler unit test for non-existent citytrip ID in CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetailTests.cs (verify null response)
- [ ] T016 [P] [US1] Write handler unit test for day plans ordering by DayNumber in CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetailTests.cs
- [ ] T017 [P] [US1] Write Blazor component test for valid citytrip ID in CitytripPlanner.Tests/Web/Pages/CitytripDetailPageTests.cs using bUnit (verify title displays)
- [ ] T018 [P] [US1] Write Blazor component test for non-existent ID in CitytripPlanner.Tests/Web/Pages/CitytripDetailPageTests.cs (verify "not found" message)

### Implementation for User Story 1

**MediatR Query & Handler**

- [ ] T019 [P] [US1] Create GetCitytripDetailQuery record in CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailQuery.cs (int CitytripId parameter, returns CitytripDetailResponse?)
- [ ] T020 [P] [US1] Create CitytripDetailResponse DTOs in CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs (CitytripDetailResponse, DayPlanDto, AttractionDto)
- [ ] T021 [US1] Implement GetCitytripDetailHandler in CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs (fetch from repository, map to DTOs, order day plans by DayNumber)

**Blazor UI**

- [ ] T022 [US1] Create CitytripDetail.razor page in CitytripPlanner.Web/Pages/CitytripDetail.razor with route @page "/citytrips/{Id:int}" and basic info rendering (title, destination, dates, photo, description)
- [ ] T023 [US1] Add code-behind logic in CitytripDetail.razor @code block to send MediatR query in OnParametersSetAsync and handle null response
- [ ] T024 [P] [US1] Create CitytripDetail.razor.css scoped styles in CitytripPlanner.Web/Pages/CitytripDetail.razor.css for layout and basic styling

**Verification**

- [ ] T025 [US1] Run all User Story 1 tests and verify they pass
- [ ] T026 [US1] Manual test: Navigate to /citytrips/1 and verify basic info displays
- [ ] T027 [US1] Manual test: Navigate to /citytrips/999 and verify "not found" message

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently. Users can view basic citytrip details even without itinerary.

---

## Phase 4: User Story 2 - View Day-by-Day Itinerary (Priority: P2)

**Goal**: Display a structured day-by-day plan with timeframes for each day of the trip

**Independent Test**: View a citytrip with defined day plans and verify each day displays in chronological order with its timeframe

### Tests for User Story 2 (TDD REQUIRED) ⚠️

- [ ] T028 [P] [US2] Write DayPlanCard component test in CitytripPlanner.Tests/Web/Components/Citytrips/DayPlanCardTests.cs using bUnit (verify day number and timeframe display)
- [ ] T029 [P] [US2] Write test for empty attractions list in CitytripPlanner.Tests/Web/Components/Citytrips/DayPlanCardTests.cs (verify "no attractions" message)

### Implementation for User Story 2

- [ ] T030 [US2] Create DayPlanCard.razor component in CitytripPlanner.Web/Components/Citytrips/DayPlanCard.razor with [Parameter] DayPlanDto Day property and rendering logic for day header (day number, date, timeframe)
- [ ] T031 [US2] Add placeholder for attractions list in DayPlanCard.razor (will be populated in US3)
- [ ] T032 [P] [US2] Create DayPlanCard.razor.css scoped styles in CitytripPlanner.Web/Components/Citytrips/DayPlanCard.razor.css
- [ ] T033 [US2] Update CitytripDetail.razor to render DayPlanCard components for each day plan and display "No itinerary available" message when DayPlans list is empty

**Verification**

- [ ] T034 [US2] Run all User Story 2 tests and verify they pass
- [ ] T035 [US2] Manual test: View citytrip with itinerary and verify day plan cards display with correct timeframes

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently. Users can see day-by-day structure even without attraction details.

---

## Phase 5: User Story 3 - View Places to Visit (Priority: P3)

**Goal**: Display attractions (museums, landmarks, restaurants) for each day with optional website links

**Independent Test**: View a day plan with attractions and verify names, descriptions, and website links are displayed and functional

### Tests for User Story 3 (TDD REQUIRED) ⚠️

- [ ] T036 [P] [US3] Write AttractionItem component test in CitytripPlanner.Tests/Web/Components/Citytrips/AttractionItemTests.cs (verify name and description display)
- [ ] T037 [P] [US3] Write test for attraction with website link in CitytripPlanner.Tests/Web/Components/Citytrips/AttractionItemTests.cs (verify link opens in new tab with target="_blank")
- [ ] T038 [P] [US3] Write test for attraction without website link in CitytripPlanner.Tests/Web/Components/Citytrips/AttractionItemTests.cs (verify no broken link displayed)

### Implementation for User Story 3

- [ ] T039 [US3] Create AttractionItem.razor component in CitytripPlanner.Web/Components/Citytrips/AttractionItem.razor with [Parameter] AttractionDto Attraction property and rendering logic for name, description, and website link
- [ ] T040 [US3] Add conditional rendering for website link (only show if WebsiteUrl is not null, use target="_blank" for new tab)
- [ ] T041 [P] [US3] Create AttractionItem.razor.css scoped styles in CitytripPlanner.Web/Components/Citytrips/AttractionItem.razor.css
- [ ] T042 [US3] Update DayPlanCard.razor to render AttractionItem components for each attraction in the day plan

**Verification**

- [ ] T043 [US3] Run all User Story 3 tests and verify they pass
- [ ] T044 [US3] Manual test: Click website link for an attraction and verify it opens in new tab
- [ ] T045 [US3] Manual test: Verify attractions without links don't show broken/empty links

**Checkpoint**: All user stories 1-3 should now be independently functional. Users can see complete attraction details with website links.

---

## Phase 6: User Story 4 - View Transportation Options (Priority: P4)

**Goal**: Display transportation options (metro, bus, walking) for reaching each attraction

**Independent Test**: View attractions with transportation information and verify it displays clearly

### Tests for User Story 4 (TDD REQUIRED) ⚠️

- [ ] T046 [P] [US4] Write TransportationInfo component test in CitytripPlanner.Tests/Web/Components/Citytrips/TransportationInfoTests.cs (verify transportation options list displays)
- [ ] T047 [P] [US4] Write test for empty transportation options in CitytripPlanner.Tests/Web/Components/Citytrips/TransportationInfoTests.cs (verify no transportation section shown)
- [ ] T048 [P] [US4] Write test for multiple transportation options in CitytripPlanner.Tests/Web/Components/Citytrips/TransportationInfoTests.cs (verify all options listed)

### Implementation for User Story 4

- [ ] T049 [US4] Create TransportationInfo.razor component in CitytripPlanner.Web/Components/Citytrips/TransportationInfo.razor with [Parameter] List<string> Options property and rendering logic for transportation list
- [ ] T050 [US4] Add conditional rendering (only show section if Options.Any() is true)
- [ ] T051 [P] [US4] Create TransportationInfo.razor.css scoped styles in CitytripPlanner.Web/Components/Citytrips/TransportationInfo.razor.css
- [ ] T052 [US4] Update AttractionItem.razor to render TransportationInfo component with attraction's TransportationOptions

**Verification**

- [ ] T053 [US4] Run all User Story 4 tests and verify they pass
- [ ] T054 [US4] Manual test: Verify transportation options display for attractions that have them
- [ ] T055 [US4] Manual test: Verify no transportation section for attractions without options

**Checkpoint**: All user stories should now be independently functional. Feature is complete!

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Integration, navigation improvements, and final polish

### Navigation & Integration

- [ ] T056 [P] Update citytrip card component in CitytripPlanner.Web/Components/Citytrips/ to wrap card in <a href="/citytrips/@trip.Id"> link for clickable navigation (US1 integration)
- [ ] T057 [P] Add "Back to List" link/button in CitytripDetail.razor header navigation to /citytrips route
- [ ] T058 Add loading state indicator in CitytripDetail.razor while MediatR query is executing

### Testing & Validation

- [ ] T059 [P] Write end-to-end integration test in CitytripPlanner.Tests/Integration/ covering full user journey from list → detail → back to list
- [ ] T060 [P] Performance test: Create citytrip with 30-day itinerary and 100+ attractions to verify <1 second render time (spec SC-006)
- [ ] T061 Run all tests across all projects (dotnet test CitytripPlanner.sln) and verify 100% pass

### Documentation

- [ ] T062 [P] Update CLAUDE.md if new patterns or conventions were established
- [ ] T063 [P] Validate implementation follows quickstart.md TDD workflow
- [ ] T064 Add inline code comments for complex mapping logic in GetCitytripDetailHandler

### Code Quality

- [ ] T065 Code review: Verify all components follow Blazor best practices (scoped CSS, parameter validation)
- [ ] T066 Code review: Verify Clean Architecture boundaries respected (Web → Features → Infrastructure, no reverse dependencies)
- [ ] T067 Code review: Verify CQRS pattern followed (read-only query, no side effects in handler)
- [ ] T068 Final refactoring: Remove any duplication, ensure YAGNI compliance

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-6)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3 → P4)
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Builds on US1 (uses CitytripDetail.razor) but is independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Builds on US2 (uses DayPlanCard.razor) but is independently testable
- **User Story 4 (P4)**: Can start after Foundational (Phase 2) - Builds on US3 (uses AttractionItem.razor) but is independently testable

**Note**: While US2-4 build on previous stories' components, each delivers independent value and can be tested separately.

### Within Each User Story

- Tests MUST be written and FAIL before implementation (TDD Red phase)
- Domain models before services (T005-T009 before T010-T013)
- DTOs before handler (T019-T020 before T021)
- Handler before Blazor components (T021 before T022-T024)
- Tests MUST PASS after implementation (TDD Green phase)
- Story complete before moving to next priority

### Parallel Opportunities

**Setup Phase (Phase 1)**:
- All verification tasks (T001-T004) can run in parallel

**Foundational Phase (Phase 2)**:
- Domain model creation (T005, T007) can run in parallel
- Domain model tests (T006, T008) can run in parallel with their implementations
- Repository extension tasks (T010-T013) must run sequentially after domain models

**Within User Stories**:
- All test tasks marked [P] within a story can run in parallel
- All DTO/contract creation tasks marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members once Foundational phase completes

---

## Parallel Example: User Story 1

```bash
# RED Phase - Launch all tests for User Story 1 together (MUST FAIL):
Task T014: "Handler unit test for valid citytrip ID"
Task T015: "Handler unit test for non-existent ID"
Task T016: "Handler unit test for day plans ordering"
Task T017: "Blazor component test for valid ID"
Task T018: "Blazor component test for not found"

# Then in parallel, create contracts:
Task T019: "Create GetCitytripDetailQuery"
Task T020: "Create CitytripDetailResponse DTOs"

# GREEN Phase - Implement handler (depends on T019, T020):
Task T021: "Implement GetCitytripDetailHandler"

# Then implement UI:
Task T022: "Create CitytripDetail.razor page"
Task T023: "Add MediatR query logic"
Task T024: "Create scoped CSS" (parallel with T023)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (basic detail view)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready - users can view citytrip details even without full itinerary

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP! Basic detail view works)
3. Add User Story 2 → Test independently → Deploy/Demo (Day-by-day itinerary added)
4. Add User Story 3 → Test independently → Deploy/Demo (Attractions with links added)
5. Add User Story 4 → Test independently → Deploy/Demo (Transportation options added)
6. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (T014-T027)
   - Developer B: User Story 2 (T028-T035) - can start after US1 creates CitytripDetail.razor
   - Developer C: User Story 3 (T036-T045) - can start after US2 creates DayPlanCard.razor
   - Developer D: User Story 4 (T046-T055) - can start after US3 creates AttractionItem.razor
3. Stories complete and integrate independently

**Realistic Parallel**: Due to component dependencies, stagger starts:
- US1 starts immediately after Foundational
- US2 starts when T022 (CitytripDetail.razor) completes
- US3 starts when T030 (DayPlanCard.razor) completes
- US4 starts when T039 (AttractionItem.razor) completes

---

## TDD Workflow Reminder

**MANDATORY per Constitution Principle II**

For EVERY task involving implementation:

1. **RED**: Write failing test first
   - Test should fail because implementation doesn't exist
   - Verify test actually fails (don't skip this!)

2. **GREEN**: Write minimum code to pass test
   - Don't over-engineer
   - Just make the test pass

3. **REFACTOR**: Clean up code while keeping tests green
   - Remove duplication
   - Improve clarity
   - Tests still pass

4. **Commit**: Atomic commits per logical change

---

## Notes

- **[P] tasks** = different files, no dependencies, safe to parallelize
- **[Story] label** maps task to specific user story for traceability
- Each user story should be independently completable and testable
- **TDD is non-negotiable**: Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- **Avoid**: vague tasks, same-file conflicts, skipping tests, cross-story dependencies that break independence

---

## Summary

**Total Tasks**: 68
- Phase 1 (Setup): 4 tasks
- Phase 2 (Foundational): 9 tasks
- Phase 3 (US1 - View Details): 14 tasks
- Phase 4 (US2 - View Itinerary): 8 tasks
- Phase 5 (US3 - View Attractions): 10 tasks
- Phase 6 (US4 - View Transportation): 10 tasks
- Phase 7 (Polish): 13 tasks

**Parallel Opportunities**: 36 tasks marked [P]

**Independent Test Criteria**:
- US1: Navigate to detail page, verify basic info displays
- US2: View citytrip, verify day plans show with timeframes
- US3: View day plan, verify attractions with website links work
- US4: View attraction, verify transportation options display

**Suggested MVP Scope**: Phase 1 + Phase 2 + Phase 3 (US1 only) = 27 tasks
This delivers a working detail view with basic information - users can already benefit from the feature!
