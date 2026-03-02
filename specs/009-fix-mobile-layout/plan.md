# Implementation Plan: Mobile & Tablet Responsive Layout

**Branch**: `009-fix-mobile-layout` | **Date**: 2026-03-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/009-fix-mobile-layout/spec.md`

## Summary

Make the CitytripPlanner Blazor Server application fully usable on smartphones (320–428 px) and tablets (768–1024 px) in both orientations. The fix targets CSS layout in all existing pages and components. Navigation gains a hamburger toggle for mobile. The citytrip overview and detail pages use a 2-column layout on tablet. The create/edit wizard presents one step per screen on mobile. All changes comply with WCAG 2.1 AA. No new backend logic, CQRS handlers, or data model changes are required.

## Technical Context

**Language/Version**: C# / .NET 10, Blazor Server
**Primary Dependencies**: Bootstrap 5.x (already in wwwroot/lib), scoped Blazor CSS isolation
**Storage**: N/A — layout fix only
**Testing**: xUnit 2.9.3, bUnit 2.5.3, FluentAssertions 7.0.0, NSubstitute 5.3.0
**Target Platform**: Browser (iOS Safari + Android Chrome on mobile; desktop browsers unchanged)
**Performance Goals**: All pages interactive < 3 seconds on simulated 4G (existing Blazor Server architecture; no new assets introduced)
**Constraints**: No new NuGet packages; no JavaScript additions; WCAG 2.1 AA compliance; desktop layout must not regress
**Scale/Scope**: 20 files modified (CSS + Razor); 4 new bUnit test files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. CQRS with Vertical Slices | ✅ Not applicable | Feature is pure UI/CSS. No new commands, queries, or handlers. No vertical slice changes needed. |
| II. TDD (NON-NEGOTIABLE) | ✅ Compliant | bUnit tests written first for: hamburger toggle, grid structure, wizard step visibility, ARIA attributes. Tests verify structural/class correctness (CSS rendering is out of bUnit scope). |
| III. Clean Architecture Layering | ✅ Compliant | All changes in `CitytripPlanner.Web`. No cross-layer dependencies introduced. |
| IV. Simplicity & YAGNI | ✅ Compliant | No new abstractions. CSS media queries added to existing scoped CSS files. Hamburger toggle uses existing Blazor state pattern. |

**Complexity Tracking**: No violations — table omitted.

## Project Structure

### Documentation (this feature)

```text
specs/009-fix-mobile-layout/
├── plan.md              # This file
├── research.md          # Phase 0 output
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (affected files only)

```text
CitytripPlanner.Web/
├── wwwroot/
│   └── app.css                                      # Global touch targets, focus indicators, WCAG base
├── Components/
│   ├── Layout/
│   │   ├── NavMenu.razor                            # Add hamburger toggle state + aria attributes
│   │   ├── NavMenu.razor.css                        # Mobile nav collapse styles
│   │   └── MainLayout.razor.css                     # Fluid container, padding on mobile
│   ├── Pages/
│   │   ├── Citytrips.razor.css                      # Responsive masonry grid (1→2→3 columns)
│   │   ├── CitytripDetail.razor.css                 # Verify/complete 2-column tablet layout
│   │   ├── MyCitytrips.razor.css                    # Verify mobile layout
│   │   ├── CreateCitytrip.razor.css                 # Verify/complete one-step-per-screen
│   │   ├── EditCitytrip.razor.css                   # Verify/complete one-step-per-screen
│   │   └── Profile.razor.css                        # Verify mobile layout
│   ├── Citytrips/
│   │   ├── WizardStep1.razor.css                    # Verify full-width on mobile
│   │   ├── WizardStep2.razor.css                    # Verify full-width on mobile
│   │   ├── WizardStep3.razor.css                    # Verify full-width on mobile
│   │   ├── DaySchedulePanel.razor.css               # Verify single-column on mobile
│   │   ├── TripMapSidebar.razor.css                 # Map scales to screen width
│   │   ├── LocationPickerModal.razor.css            # Verify/complete modal mobile styles
│   │   └── EventEditorRow.razor.css                 # Verify flex direction on mobile
│   └── Shared/
│       └── TripCard.razor.css                       # Touch target min-size

CitytripPlanner.Tests/
└── Web/
    ├── Layout/
    │   └── NavMenuTests.cs                          # NEW: hamburger toggle + aria
    └── Pages/
        ├── CitytripsMobileTests.cs                  # NEW: grid structure
        ├── CitytripDetailMobileTests.cs             # NEW: layout structure on tablet
        └── WizardMobileTests.cs                     # NEW: one-step-per-screen structure
```

**Structure Decision**: Web-only change using existing single-project structure. `CitytripPlanner.Web` is the only project modified. Test additions go into `CitytripPlanner.Tests/Web/` mirroring the existing test folder layout.

## Implementation Design

### Breakpoint System

Three explicit breakpoints, consistent across all files:

| Range | Target | Column count (overview) |
|-------|--------|------------------------|
| `< 768px` | Smartphone | 1 column |
| `768px – 1024px` | Tablet | 2 columns (overview + detail), 1 column elsewhere |
| `> 1024px` | Desktop | 3 columns (existing) |

All media queries use `max-width` for mobile-first override approach:
```css
/* tablet */
@media (max-width: 1024px) { ... }
/* mobile */
@media (max-width: 767px) { ... }
```

### Navigation (NavMenu.razor + NavMenu.razor.css)

**Blazor state**: Add `bool _menuOpen = false;` field and `void ToggleMenu() => _menuOpen = !_menuOpen;` method.

**Markup changes**:
- Wrap existing nav links in a `<div class="nav-menu @(_menuOpen ? "nav-menu--open" : "")">` container
- Add a `<button>` hamburger toggle visible only on mobile (`aria-label="Toggle navigation"`, `aria-expanded="@_menuOpen"`)

**CSS**:
```css
/* mobile: collapse nav */
@media (max-width: 767px) {
    .nav-menu { display: none; }
    .nav-menu--open { display: flex; flex-direction: column; }
    .nav-toggle { display: flex; } /* hamburger button */
}
/* desktop: always show, hide toggle */
@media (min-width: 768px) {
    .nav-menu { display: flex; }
    .nav-toggle { display: none; }
}
```

### Citytrip Overview Grid (Citytrips.razor.css)

Current (broken): `.masonry-grid { grid-template-columns: repeat(3, 1fr); }`

Fixed:
```css
.masonry-grid { grid-template-columns: repeat(3, 1fr); }

@media (max-width: 1024px) {
    .masonry-grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 767px) {
    .masonry-grid { grid-template-columns: repeat(1, 1fr); }
}
```

### Citytrip Detail (CitytripDetail.razor.css)

Partial media query at 768px already exists. Audit and complete:
- Ensure `.detail-layout` switches to single-column below 768px
- At 768–1024px keep 2-column (already partially implemented)
- Verify map container and schedule panel stack correctly on mobile

### Wizard (CreateCitytrip.razor.css, EditCitytrip.razor.css, WizardStep*.razor.css)

Partial media queries already exist at 768px. Verify and complete:
- Each wizard step (`WizardStep1/2/3`) must be `width: 100%` on mobile
- The step indicator bar must either wrap or scroll horizontally
- Next/Back buttons: full-width, min-height 44px on mobile

No Razor logic changes needed if the wizard already shows only the active step via Blazor `@if` state (confirmed by research decision 4). CSS ensures each displayed step fills the screen.

### WCAG 2.1 AA (app.css)

Global additions:
```css
/* Touch targets */
button, a, [role="button"] {
    min-height: 44px;
    min-width: 44px;
}

/* Focus indicators */
:focus-visible {
    outline: 2px solid #0056b3;
    outline-offset: 2px;
}
```

Aria attributes added to:
- Hamburger toggle button: `aria-label`, `aria-expanded`, `aria-controls`
- Any icon-only buttons (audit during implementation)

### TripMapSidebar (TripMapSidebar.razor.css)

Add `max-width: 100%` and `height: auto` to map container on mobile so the Google Maps iframe scales to screen width without breaking surrounding layout.

## Test Plan

### bUnit Tests (TDD — write tests first)

**NavMenuTests.cs**
- `NavMenu_RendersMobileToggleButton` — hamburger button element is present in rendered markup
- `NavMenu_ToggleButtonHasCorrectAria` — `aria-expanded="false"` by default
- `NavMenu_ToggleOpensMenu` — clicking toggle sets `aria-expanded="true"` and nav-menu--open class applied
- `NavMenu_ToggleClosesMenu` — clicking toggle again removes open class

**CitytripsMobileTests.cs**
- `Citytrips_GridContainerRendersWithMasonryClass` — `.masonry-grid` CSS class present on grid element

**CitytripDetailMobileTests.cs**
- `CitytripDetail_DetailLayoutClassPresent` — `.detail-layout` CSS class present on container

**WizardMobileTests.cs**
- `Wizard_ActiveStepIsRendered` — active step component is in the DOM
- `Wizard_NextButtonPresent` — Next button rendered on active step
- `Wizard_BackButtonPresent` — Back button rendered on steps > 1

### Manual Acceptance Testing

Performed in browser DevTools device simulator:

| Viewport | Browser | Pages to verify |
|----------|---------|----------------|
| 375×812 (iPhone 14) | iOS Safari sim | All pages |
| 390×844 (iPhone 14 Pro) | iOS Safari sim | All pages |
| 360×800 (Android) | Chrome DevTools | All pages |
| 768×1024 (iPad portrait) | Chrome DevTools | Overview, Detail |
| 1024×768 (iPad landscape) | Chrome DevTools | Overview, Detail |

Checklist per page:
- [ ] No horizontal scroll bar
- [ ] All text readable (no overflow/clip)
- [ ] All buttons/links tappable (≥ 44px)
- [ ] Navigation accessible (hamburger works on mobile)
- [ ] Focus indicators visible on keyboard navigation

## Risk Register

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Blazor CSS isolation scoping breaks media query overrides | Medium | Medium | Use `::deep` where needed; test in browser, not just bUnit |
| Bootstrap utility classes conflict with custom media queries | Low | Low | Use more specific selectors in scoped CSS; scoped CSS has higher specificity |
| Google Maps iframe doesn't scale | Low | Medium | Set `width: 100%; height: auto` on wrapper; test with DevTools |
| Hamburger toggle state lost on Blazor reconnect | Low | Low | State is ephemeral; acceptable for a toggle — resets to closed, which is correct |
| WCAG colour contrast failures in existing palette | Medium | Medium | Audit all text/background pairs during implementation; adjust CSS variables |
