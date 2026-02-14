# Tasks: User Profile Management

**Input**: Design documents from `/specs/004-user-profile-page/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: This feature follows TDD (Test-Driven Development) as required by the project constitution. All tests MUST be written FIRST and FAIL before implementation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Features**: `CitytripPlanner/CitytripPlanner.Features/UserProfiles/`
- **Infrastructure**: `CitytripPlanner/CitytripPlanner.Infrastructure/UserProfiles/`
- **Web**: `CitytripPlanner/CitytripPlanner.Web/Components/Pages/`
- **Tests**: `CitytripPlanner/CitytripPlanner.Tests/UserProfiles/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and folder structure for UserProfiles feature domain

- [x] T001 Create UserProfiles feature domain folder at CitytripPlanner/CitytripPlanner.Features/UserProfiles/
- [x] T002 Create Domain subfolder at CitytripPlanner/CitytripPlanner.Features/UserProfiles/Domain/
- [x] T003 Create GetUserProfile slice folder at CitytripPlanner/CitytripPlanner.Features/UserProfiles/GetUserProfile/
- [x] T004 Create UpdateUserProfile slice folder at CitytripPlanner/CitytripPlanner.Features/UserProfiles/UpdateUserProfile/
- [x] T005 Create UserProfiles infrastructure folder at CitytripPlanner/CitytripPlanner.Infrastructure/UserProfiles/
- [x] T006 Create UserProfiles tests folder at CitytripPlanner/CitytripPlanner.Tests/UserProfiles/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core domain entities and infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Entities

- [x] T007 [P] Create UserProfile entity in CitytripPlanner/CitytripPlanner.Features/UserProfiles/Domain/UserProfile.cs
- [x] T008 [P] Create GenderOptions constants in CitytripPlanner/CitytripPlanner.Features/UserProfiles/Domain/GenderOptions.cs
- [x] T009 [P] Create Countries list in CitytripPlanner/CitytripPlanner.Features/UserProfiles/Domain/Countries.cs
- [x] T010 Create IUserProfileRepository interface in CitytripPlanner/CitytripPlanner.Features/UserProfiles/Domain/IUserProfileRepository.cs

### Infrastructure Implementation

- [x] T011 Create InMemoryUserProfileRepository in CitytripPlanner/CitytripPlanner.Infrastructure/UserProfiles/InMemoryUserProfileRepository.cs
- [x] T012 Register IUserProfileRepository and InMemoryUserProfileRepository in DI container (Program.cs or ServiceCollectionExtensions)
- [x] T013 Register MediatR handlers for UserProfiles assembly in DI container

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - View Personal Profile (Priority: P1) 🎯 MVP

**Goal**: Authenticated users can navigate to their profile page and view their current personal information (name, firstname, gender, country)

**Independent Test**: Navigate to /profile page, verify profile data displays correctly (or empty form if no profile exists)

### Tests for User Story 1 (TDD - Write FIRST, ensure FAIL)

- [x] T014 [P] [US1] Write test: GetUserProfileHandler returns null when profile does not exist - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/GetUserProfileHandlerTests.cs
- [x] T015 [P] [US1] Write test: GetUserProfileHandler returns UserProfileResponse when profile exists - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/GetUserProfileHandlerTests.cs
- [x] T016 [P] [US1] Write test: GetUserProfileHandler maps entity fields to response correctly - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/GetUserProfileHandlerTests.cs

**⚠️ VERIFY TESTS FAIL** before proceeding to implementation

### Implementation for User Story 1

- [x] T017 [P] [US1] Create GetUserProfileQuery record in CitytripPlanner/CitytripPlanner.Features/UserProfiles/GetUserProfile/GetUserProfileQuery.cs
- [x] T018 [P] [US1] Create UserProfileResponse record in CitytripPlanner/CitytripPlanner.Features/UserProfiles/GetUserProfile/UserProfileResponse.cs
- [x] T019 [US1] Implement GetUserProfileHandler in CitytripPlanner/CitytripPlanner.Features/UserProfiles/GetUserProfile/GetUserProfileHandler.cs (depends on T017, T018)
- [x] T020 [US1] Create Profile.razor component (view mode only) in CitytripPlanner/CitytripPlanner.Web/Components/Pages/Profile.razor
- [x] T021 [US1] Create Profile.razor.css for scoped styling in CitytripPlanner/CitytripPlanner.Web/Components/Pages/Profile.razor.css

**⚠️ RUN TESTS** - All User Story 1 tests should now PASS

**Checkpoint**: At this point, User Story 1 should be fully functional - users can view their profile page and see their information

---

## Phase 4: User Story 2 - Edit Basic Personal Information (Priority: P2)

**Goal**: Users can edit their name and firstname fields, save changes, and see validation errors for required fields

**Independent Test**: Click Edit button, modify name/firstname, save, verify changes persist after reload. Test required field validation.

### Tests for User Story 2 (TDD - Write FIRST, ensure FAIL)

- [x] T022 [P] [US2] Write test: UpdateUserProfileHandler creates new profile when none exists - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileHandlerTests.cs
- [x] T023 [P] [US2] Write test: UpdateUserProfileHandler updates existing profile - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileHandlerTests.cs
- [x] T024 [P] [US2] Write test: UpdateUserProfileHandler uses UserId from ICurrentUserService - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileHandlerTests.cs
- [x] T025 [P] [US2] Write test: UpdateUserProfileValidator rejects empty Name - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T026 [P] [US2] Write test: UpdateUserProfileValidator rejects empty Firstname - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T027 [P] [US2] Write test: UpdateUserProfileValidator rejects Name longer than 100 characters - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T028 [P] [US2] Write test: UpdateUserProfileValidator rejects Firstname longer than 100 characters - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T029 [P] [US2] Write test: UpdateUserProfileValidator accepts valid Name and Firstname - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs

**⚠️ VERIFY TESTS FAIL** before proceeding to implementation

### Implementation for User Story 2

- [x] T030 [P] [US2] Create UpdateUserProfileCommand record in CitytripPlanner/CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileCommand.cs
- [x] T031 [US2] Create UpdateUserProfileValidator in CitytripPlanner/CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileValidator.cs (depends on T030)
- [x] T032 [US2] Implement UpdateUserProfileHandler in CitytripPlanner/CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileHandler.cs (depends on T030)
- [x] T033 [US2] Update Profile.razor to add edit mode with Edit/Save/Cancel buttons
- [x] T034 [US2] Add form fields for Name and Firstname in Profile.razor with validation display
- [x] T035 [US2] Implement save logic in Profile.razor to send UpdateUserProfileCommand via MediatR
- [x] T036 [US2] Implement cancel logic in Profile.razor to restore original values
- [x] T037 [US2] Add success/error message display in Profile.razor

**⚠️ RUN TESTS** - All User Story 2 tests should now PASS

**Checkpoint**: At this point, User Stories 1 AND 2 should both work - users can view and edit name/firstname

---

## Phase 5: User Story 3 - Edit Additional Personal Information (Priority: P3)

**Goal**: Users can select gender and country from dropdowns, save changes, and see the selections persist

**Independent Test**: In edit mode, select gender and country from dropdowns, save, verify selections persist and display correctly

### Tests for User Story 3 (TDD - Write FIRST, ensure FAIL)

- [x] T038 [P] [US3] Write test: UpdateUserProfileValidator accepts valid Gender option - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T039 [P] [US3] Write test: UpdateUserProfileValidator rejects invalid Gender - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T040 [P] [US3] Write test: UpdateUserProfileValidator accepts valid Country - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T041 [P] [US3] Write test: UpdateUserProfileValidator rejects invalid Country - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T042 [P] [US3] Write test: UpdateUserProfileValidator accepts null Gender and Country (optional fields) - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileValidatorTests.cs
- [x] T043 [P] [US3] Write test: UpdateUserProfileHandler saves Gender and Country correctly - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/UpdateUserProfileHandlerTests.cs

**⚠️ VERIFY TESTS FAIL** before proceeding to implementation

### Implementation for User Story 3

- [x] T044 [US3] Update UpdateUserProfileValidator to add Gender validation rules (must be in GenderOptions.All if provided)
- [x] T045 [US3] Update UpdateUserProfileValidator to add Country validation rules (must be in Countries.All if provided)
- [x] T046 [US3] Add Gender dropdown in Profile.razor bound to GenderOptions.All
- [x] T047 [US3] Add Country dropdown in Profile.razor bound to Countries.All
- [x] T048 [US3] Update save logic in Profile.razor to include Gender and Country in UpdateUserProfileCommand

**⚠️ RUN TESTS** - All User Story 3 tests should now PASS

**Checkpoint**: All user stories (1, 2, 3) should now be independently functional

---

## Phase 6: Blazor Component Testing & Integration

**Purpose**: End-to-end Blazor component tests using bUnit to verify UI behavior

### Blazor Component Tests (bUnit)

- [ ] T049 [P] Write bUnit test: Profile.razor displays loading state on initial render - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T050 [P] Write bUnit test: Profile.razor displays profile data when query returns result - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T051 [P] Write bUnit test: Profile.razor displays empty form when query returns null - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T052 [P] Write bUnit test: Clicking Edit button enables form fields - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T053 [P] Write bUnit test: Clicking Cancel button restores original values and disables editing - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T054 [P] Write bUnit test: Clicking Save with valid data sends UpdateUserProfileCommand - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs
- [ ] T055 [P] Write bUnit test: Validation errors display when saving invalid data - CitytripPlanner/CitytripPlanner.Tests/UserProfiles/ProfilePageTests.cs

**⚠️ RUN COMPONENT TESTS** - Verify all bUnit tests pass

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Navigation, styling, and final touches

- [x] T056 Add navigation link to Profile page in main layout/navigation menu - CitytripPlanner/CitytripPlanner.Web/Components/Layout/
- [ ] T057 Add route configuration for /profile in routing setup
- [ ] T058 [P] Add responsive styling for mobile devices in Profile.razor.css
- [ ] T059 [P] Add ARIA labels and accessibility attributes to form fields in Profile.razor
- [ ] T060 Test edge case: Verify 100-character limit on Name field (maxlength attribute)
- [ ] T061 Test edge case: Verify 100-character limit on Firstname field (maxlength attribute)
- [ ] T062 Test edge case: Verify special characters (é, ñ, 中文) are accepted in name fields
- [ ] T063 [P] Update CLAUDE.md with UserProfiles feature patterns (if not already done)
- [ ] T064 Run full test suite: dotnet test from solution root
- [ ] T065 Manual validation per quickstart.md checklist

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion - BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - User Story 1 (Phase 3): Can start after Phase 2
  - User Story 2 (Phase 4): Can start after Phase 2 (independent of US1, but builds on same component)
  - User Story 3 (Phase 5): Can start after Phase 2 (extends US2 functionality)
- **Blazor Testing (Phase 6)**: Can start after any user story is implemented
- **Polish (Phase 7)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Extends Profile.razor from US1
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Further extends Profile.razor from US2

### TDD Workflow Within Each User Story

1. **RED**: Write failing tests first (T014-T016 for US1, T022-T029 for US2, etc.)
2. **Verify**: Run tests, confirm they FAIL (no implementation yet)
3. **GREEN**: Implement code to make tests pass (T017-T021 for US1, T030-T037 for US2, etc.)
4. **Verify**: Run tests, confirm they PASS
5. **REFACTOR**: Clean up code if needed
6. **Checkpoint**: Story complete and independently testable

### Parallel Opportunities

- **Phase 1 (Setup)**: All 6 folder creation tasks can run in parallel
- **Phase 2 (Foundational)**: Domain entities (T007-T009) can run in parallel
- **User Story Tests**: All test tasks within a story marked [P] can run in parallel
  - US1: T014-T016 parallel
  - US2: T022-T029 parallel
  - US3: T038-T043 parallel
- **User Story Models/DTOs**: Command/Query/Response creation tasks (T017-T018, T030) can run in parallel
- **Blazor Component Tests**: T049-T055 can all run in parallel
- **Polish Tasks**: Styling (T058), accessibility (T059), CLAUDE.md (T063) can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together (RED phase):
Task T014: "Write test: GetUserProfileHandler returns null when profile does not exist"
Task T015: "Write test: GetUserProfileHandler returns UserProfileResponse when profile exists"
Task T016: "Write test: GetUserProfileHandler maps entity fields to response correctly"

# Launch DTOs for User Story 1 together (GREEN phase):
Task T017: "Create GetUserProfileQuery record"
Task T018: "Create UserProfileResponse record"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T006)
2. Complete Phase 2: Foundational (T007-T013) - CRITICAL - blocks all stories
3. Complete Phase 3: User Story 1 (T014-T021)
   - Write tests first (T014-T016)
   - Verify tests FAIL
   - Implement code (T017-T021)
   - Verify tests PASS
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Demo/review if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 (View Profile) → Test independently → Demo (MVP!)
3. Add User Story 2 (Edit Name/Firstname) → Test independently → Demo
4. Add User Story 3 (Edit Gender/Country) → Test independently → Demo
5. Add Blazor Component Tests → Full UI coverage
6. Polish → Production ready

### Full Feature Strategy

1. Phase 1: Setup (6 tasks)
2. Phase 2: Foundational (7 tasks)
3. Phase 3: User Story 1 - View Profile (8 tasks)
4. Phase 4: User Story 2 - Edit Basic Info (16 tasks)
5. Phase 5: User Story 3 - Edit Additional Info (7 tasks)
6. Phase 6: Blazor Component Testing (7 tasks)
7. Phase 7: Polish (10 tasks)

**Total: 61 tasks**

---

## Notes

- [P] tasks = different files, no dependencies - can run in parallel
- [Story] label maps task to specific user story for traceability
- **TDD is mandatory** per project constitution - tests MUST be written first
- Each user story should be independently completable and testable
- **RED-GREEN-REFACTOR cycle**: Write failing test → Implement → Verify pass → Refactor
- Commit after each logical group of tasks
- Stop at any checkpoint to validate story independently
- Run `dotnet test` frequently to verify tests
- Follow existing patterns from Citytrips feature domain

---

## Test Execution Commands

```bash
# Run all tests
cd CitytripPlanner
dotnet test

# Run only UserProfiles tests
dotnet test --filter "FullyQualifiedName~UserProfiles"

# Run specific test file
dotnet test --filter "FullyQualifiedName~GetUserProfileHandlerTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```
