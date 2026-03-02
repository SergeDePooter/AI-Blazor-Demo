# Tasks: Mobile & Tablet Responsive Layout

**Input**: Design documents from `/specs/009-fix-mobile-layout/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅

**Tests**: Included — Constitution principle II (TDD) is NON-NEGOTIABLE. Tests are written first (Red-Green-Refactor).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1–US4)
- All paths are relative to `CitytripPlanner/` unless prefixed with `CitytripPlanner.Tests/`

---

## Phase 1: Setup

**Purpose**: Create missing test folder structure before TDD work begins

- [X] T001 Create test directories `CitytripPlanner.Tests/Web/Layout/` and `CitytripPlanner.Tests/Web/Pages/` if not already present

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: WCAG AA global CSS baseline and breakpoint standards that all user story phases depend on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T002 Add WCAG 2.1 AA global styles to `CitytripPlanner.Web/wwwroot/app.css`: `min-height: 44px; min-width: 44px` on `button, a, [role="button"]`; `:focus-visible` outline (`2px solid #0056b3, offset 2px`); ensure no existing rules override focus outline
- [X] T003 [P] Audit all existing scoped CSS files for conflicting `outline: none` or `outline: 0` rules that suppress focus indicators; remove or replace with `:focus:not(:focus-visible)` pattern

**Checkpoint**: Global WCAG baseline in place — user story implementation can now begin

---

## Phase 3: User Story 1 — Browse Citytrips on a Smartphone (Priority: P1) 🎯 MVP

**Goal**: The citytrip overview page is fully usable on a smartphone (single-column, no horizontal scroll, touch-friendly cards)

**Independent Test**: Load Citytrips page in Chrome DevTools at 375px width → no horizontal scrollbar, all cards readable, all tap targets ≥ 44px

### Tests for User Story 1 ⚠️ Write first — must FAIL before implementation

- [X] T004 [US1] Write failing bUnit test `NavMenu_GridContainerRendersWithMasonryClass` in `CitytripPlanner.Tests/Web/Pages/CitytripsMobileTests.cs` — assert `.masonry-grid` element exists in rendered Citytrips component
- [X] T005 [P] [US1] Write failing bUnit test `TripCard_ContainsActionableElements` in `CitytripPlanner.Tests/Web/Pages/CitytripsMobileTests.cs` — assert TripCard renders button/link elements (touch target carriers)

### Implementation for User Story 1

- [X] T006 [US1] Update `CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`: change `.masonry-grid` from fixed `repeat(3, 1fr)` to desktop-default with `@media (max-width: 767px) { grid-template-columns: repeat(1, 1fr); }` — verify T004 passes
- [X] T007 [P] [US1] Update `CitytripPlanner.Web/Components/Shared/TripCard.razor.css`: ensure all interactive elements (like/enlist buttons) have `min-height: 44px` — verify T005 passes

**Checkpoint**: Citytrip overview fully usable on 375px smartphone — User Story 1 independently testable ✅

---

## Phase 4: User Story 2 — View Citytrip Details and Day Schedule on a Smartphone (Priority: P2)

**Goal**: The detail page and day schedule are fully readable and scrollable on a smartphone, including the map area

**Independent Test**: Load CitytripDetail page at 375px width → all sections visible in single-column, map scales to width, no event text truncated

### Tests for User Story 2 ⚠️ Write first — must FAIL before implementation

- [X] T008 [US2] Write failing bUnit test `CitytripDetail_DetailLayoutClassPresent` in `CitytripPlanner.Tests/Web/Pages/CitytripDetailMobileTests.cs` — assert `.detail-layout` element exists in rendered CitytripDetail component
- [X] T009 [P] [US2] Write failing bUnit test `DaySchedulePanel_RendersEventList` in `CitytripPlanner.Tests/Web/Pages/CitytripDetailMobileTests.cs` — assert DaySchedulePanel renders a container element with scheduled event children

### Implementation for User Story 2

- [X] T010 [US2] Update `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor.css`: ensure `.detail-layout` switches to `grid-template-columns: 1fr` at `@media (max-width: 767px)` — verify T008 passes
- [X] T011 [P] [US2] Update `CitytripPlanner.Web/Components/Citytrips/DaySchedulePanel.razor.css`: ensure event list container is `width: 100%` with no fixed widths at `@media (max-width: 767px)` — verify T009 passes
- [X] T012 [P] [US2] Update `CitytripPlanner.Web/Components/Citytrips/TripMapSidebar.razor.css`: add `@media (max-width: 767px) { max-width: 100%; height: auto; }` to map container so Google Maps iframe fills screen width
- [X] T013 [P] [US2] Update `CitytripPlanner.Web/Components/Shared/ScheduledEventCard.razor.css`: verify no fixed widths cause horizontal overflow at 320px; add `word-wrap: break-word; overflow-wrap: break-word` to text containers
- [X] T014 [P] [US2] Update `CitytripPlanner.Web/Components/Citytrips/EventEditorRow.razor.css`: verify existing 600px media query correctly switches flex direction to column; extend to 767px for consistency

**Checkpoint**: Detail + day schedule fully usable on smartphone — User Story 2 independently testable ✅

---

## Phase 5: User Story 3 — Use the Application on a Tablet (Priority: P3)

**Goal**: Citytrip overview and detail pages display a 2-column layout on tablets (768–1024px); all other pages adapt without overflow

**Independent Test**: Load Citytrips page at 768px width → 2-column card grid; load CitytripDetail at 768px → 2-column layout; rotate to landscape at 1024px → layout holds

### Tests for User Story 3 ⚠️ Write first — must FAIL before implementation

- [X] T015 [US3] Write failing bUnit test `Citytrips_GridContainerExists` in `CitytripPlanner.Tests/Web/Pages/CitytripsMobileTests.cs` — extend existing test file with assertion that `.masonry-grid` is present (structure required for tablet CSS to target it)

### Implementation for User Story 3

- [X] T016 [US3] Update `CitytripPlanner.Web/Components/Pages/Citytrips.razor.css`: add tablet breakpoint `@media (min-width: 768px) and (max-width: 1024px) { .masonry-grid { grid-template-columns: repeat(2, 1fr); } }` — verify T015 passes and manual 768px check shows 2 columns
- [X] T017 [US3] Update `CitytripPlanner.Web/Components/Pages/CitytripDetail.razor.css`: ensure `.detail-layout` at `768px–1024px` retains `grid-template-columns: 1fr 1fr` (or equivalent 2-column) — manual check at 768px portrait
- [X] T018 [P] [US3] Update `CitytripPlanner.Web/Components/Pages/MyCitytrips.razor.css`: verify no fixed widths cause overflow at 768px; ensure trip list and action buttons are fluid

**Checkpoint**: Overview and detail pages show 2-column layout on tablet — User Story 3 independently testable ✅

---

## Phase 6: User Story 4 — Navigate Between Pages on Any Device (Priority: P4)

**Goal**: Navigation is accessible on all screen sizes — hamburger toggle on mobile, full nav on tablet and desktop, correct active indicator everywhere

**Independent Test**: Render NavMenu at 375px in bUnit → hamburger button present, toggle opens/closes nav links with correct `aria-expanded` state

### Tests for User Story 4 ⚠️ Write first — must FAIL before implementation

- [X] T019 [US4] Write failing bUnit test `NavMenu_RendersMobileToggleButton` in `CitytripPlanner.Tests/Web/Layout/NavMenuTests.cs` — assert element with class `nav-toggle` (or `aria-label="Toggle navigation"`) exists in rendered NavMenu
- [X] T020 [US4] Write failing bUnit test `NavMenu_ToggleButtonHasAriaExpandedFalseByDefault` in `CitytripPlanner.Tests/Web/Layout/NavMenuTests.cs` — assert hamburger button has `aria-expanded="false"` on initial render
- [X] T021 [US4] Write failing bUnit test `NavMenu_ClickingToggleOpensMenu` in `CitytripPlanner.Tests/Web/Layout/NavMenuTests.cs` — simulate click on toggle button, assert `aria-expanded="true"` and nav container has open CSS class
- [X] T022 [P] [US4] Write failing bUnit test `NavMenu_ClickingToggleAgainClosesMenu` in `CitytripPlanner.Tests/Web/Layout/NavMenuTests.cs` — simulate two clicks, assert menu returns to closed state

### Implementation for User Story 4

- [X] T023 [US4] Update `CitytripPlanner.Web/Components/Layout/NavMenu.razor`: add `bool _menuOpen = false;` field and `void ToggleMenu() => _menuOpen = !_menuOpen;` method; wrap nav links in `<div class="nav-menu @(_menuOpen ? "nav-menu--open" : string.Empty)">` container; add hamburger `<button>` with `@onclick="ToggleMenu"`, `aria-label="Toggle navigation"`, `aria-expanded="@_menuOpen"`, `aria-controls="nav-menu"`, class `nav-toggle` — verify T019, T020, T021, T022 pass
- [X] T024 [US4] Update `CitytripPlanner.Web/Components/Layout/NavMenu.razor.css`: add `@media (max-width: 767px) { .nav-menu { display: none; } .nav-menu--open { display: flex; flex-direction: column; } .nav-toggle { display: flex; } }` and `@media (min-width: 768px) { .nav-menu { display: flex; } .nav-toggle { display: none; } }`
- [X] T025 [P] [US4] Update `CitytripPlanner.Web/Components/Layout/MainLayout.razor.css`: add `@media (max-width: 767px) { padding: 0 1rem; }` to main content container; ensure `max-width: 100%` and no fixed widths on layout wrapper

**Checkpoint**: Navigation accessible on all screen sizes — User Story 4 independently testable ✅

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Wizard mobile experience, remaining pages, and final WCAG/acceptance validation

- [X] T026 [P] Write failing bUnit test `Wizard_ActiveStepIsRendered` and `Wizard_NextButtonIsPresent` in `CitytripPlanner.Tests/Web/Pages/WizardMobileTests.cs` — assert active wizard step component is in DOM and Next button element exists
- [X] T027 [P] Update `CitytripPlanner.Web/Components/Pages/CreateCitytrip.razor.css`: add `@media (max-width: 767px)` rules to make wizard container `width: 100%`, step body `padding: 1rem`, and Next/Back buttons `width: 100%; min-height: 44px` — verify T026 passes
- [X] T028 [P] Update `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor.css`: apply same mobile wizard styles as T027 (mirror changes)
- [X] T029 [P] Update `CitytripPlanner.Web/Components/Citytrips/WizardStep1.razor.css`: ensure form fields and inputs are `width: 100%` at `@media (max-width: 767px)`
- [X] T030 [P] Update `CitytripPlanner.Web/Components/Citytrips/WizardStep2.razor.css`: verify existing 600px breakpoint correctly stacks event editor rows; extend to 767px breakpoint for consistency
- [X] T031 [P] Update `CitytripPlanner.Web/Components/Citytrips/WizardStep3.razor.css`: ensure review/confirm layout is single-column on mobile with readable text
- [X] T032 [P] Update `CitytripPlanner.Web/Components/Citytrips/LocationPickerModal.razor.css`: verify existing 600px mobile styles handle 320px screens; ensure modal content is `max-width: 100vw` and action buttons are full-width stacked
- [X] T033 [P] Update `CitytripPlanner.Web/Components/Pages/Profile.razor.css`: audit for fixed widths; ensure form, avatar, and action areas stack and fill width at `@media (max-width: 767px)`
- [X] T034 Run `dotnet test` in repo root — all bUnit tests (T004, T005, T008, T009, T015, T019–T022, T026) must pass
- [ ] T035 Manual acceptance testing: open DevTools → test all pages at 320px, 375px, 768px (portrait), 1024px (landscape) in iOS Safari simulator and Android Chrome simulator per test plan in `plan.md`; verify checklist (no horizontal scroll, all text readable, all tap targets ≥ 44px, nav accessible, wizard one-step-per-screen)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — BLOCKS all user stories
- **User Stories (Phases 3–6)**: All depend on Phase 2 completion
  - US1 (P1) → US2 (P2) are independent; can be worked in parallel if capacity allows
  - US3 (P3) reuses the same CSS files as US1 and US2 — do not start US3 until US1 and US2 are complete to avoid file conflicts
  - US4 (P4) touches different files from US1–US3 — can proceed in parallel after Phase 2
- **Polish (Phase 7)**: Depends on all US1–US4 phases complete

### User Story Dependencies

- **US1 (P1)**: After Phase 2 — independent
- **US2 (P2)**: After Phase 2 — independent of US1 (different files)
- **US3 (P3)**: After US1 + US2 complete (modifies same CSS files as US1/US2)
- **US4 (P4)**: After Phase 2 — independent (NavMenu + MainLayout files)

### Within Each User Story

- Tests MUST be written and confirmed FAILING before implementation begins (TDD Red-Green-Refactor)
- CSS changes must be verified in browser (DevTools) in addition to bUnit structural tests
- Commit after each task or logical group

### Parallel Opportunities

- T002 and T003 (Phase 2) can run in parallel
- T004 and T005 (US1 tests) can run in parallel
- T006 and T007 (US1 impl) can run in parallel
- T008 and T009 (US2 tests) can run in parallel
- T010–T014 (US2 impl) can all run in parallel
- T019–T022 (US4 tests) can run in parallel
- T023 must complete before T024; T024 and T025 can run in parallel
- T026–T033 (Polish) can all run in parallel

---

## Parallel Example: User Story 2

```text
# Write all US2 tests in parallel:
Task: T008 — CitytripDetail_DetailLayoutClassPresent in CitytripDetailMobileTests.cs
Task: T009 — DaySchedulePanel_RendersEventList in CitytripDetailMobileTests.cs

# After tests fail, run all US2 implementation tasks in parallel:
Task: T011 — DaySchedulePanel.razor.css
Task: T012 — TripMapSidebar.razor.css
Task: T013 — ScheduledEventCard.razor.css
Task: T014 — EventEditorRow.razor.css
# (T010 CitytripDetail.razor.css can also run in parallel with the above)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (WCAG AA global baseline)
3. Complete Phase 3: User Story 1 (overview on smartphone)
4. **STOP and VALIDATE**: `dotnet test` passes; DevTools check at 375px shows single-column grid
5. Demo/review if ready

### Incremental Delivery

1. Phase 1 + 2 → WCAG baseline ready
2. Phase 3 (US1) → Smartphone overview working ✅
3. Phase 4 (US2) → Detail + schedule working ✅
4. Phase 5 (US3) → Tablet 2-column layout working ✅
5. Phase 6 (US4) → Navigation mobile-ready ✅
6. Phase 7 (Polish) → Wizard + remaining pages + full acceptance test ✅

### Parallel Team Strategy

After Phase 2:
- Developer A: US1 (Phase 3) + US3 (Phase 5, sequential)
- Developer B: US2 (Phase 4)
- Developer C: US4 (Phase 6)
- All converge for Phase 7 (Polish)

---

## Notes

- `[P]` tasks = different files, no shared-state dependencies
- `[Story]` label maps each task to a user story for traceability
- bUnit tests verify HTML structure and CSS class presence — visual CSS rendering must be verified manually in browser DevTools
- CSS changes use Blazor scoped CSS isolation — use `::deep` if parent component needs to style child component elements
- Breakpoints: `max-width: 767px` = mobile, `768px–1024px` = tablet, `> 1024px` = desktop (unchanged)
- All button/link touch targets must be ≥ 44px (enforced globally by T002; verify per-component during implementation)
- Commit after each completed task phase
