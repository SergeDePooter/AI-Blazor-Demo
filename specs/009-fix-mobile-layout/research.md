# Research: Mobile & Tablet Responsive Layout

**Feature**: 009-fix-mobile-layout
**Date**: 2026-03-02

---

## Decision 1: Breakpoint Strategy

**Decision**: Use three breakpoints aligned with Bootstrap's built-in grid:
- `< 768px` — smartphone (single-column, full-width)
- `768px – 1024px` — tablet (2-column for overview/detail, single-column elsewhere)
- `> 1024px` — desktop (existing layout, unchanged)

**Rationale**: Bootstrap is already in the project. Reusing its breakpoints (md = 768px) avoids conflicts between Bootstrap utilities and custom CSS. The existing partial media queries in the project already use 768px and 600px — standardising on 768px as the mobile/tablet boundary minimises rework.

**Alternatives considered**:
- 600px mobile boundary — already used in some files but inconsistent; consolidating to 768px is cleaner.
- CSS Container Queries — not yet broadly supported on all target browsers and adds complexity without benefit here.

---

## Decision 2: Navigation on Mobile

**Decision**: Add a hamburger toggle to `NavMenu.razor` using Blazor state (`bool _menuOpen`). The nav links collapse behind a toggle button on viewports `< 768px`. The toggle is managed server-side (Blazor `@onclick`) with CSS `display: none / block` driven by an active class.

**Rationale**: The app is Blazor Server — there is no separate JS bundle to add. Using a Blazor `@onclick` handler on the toggle button is the simplest approach with zero new dependencies (YAGNI principle). Bootstrap's existing `collapse` JS is available but would require adding Bootstrap JS bundle interaction; the Blazor state approach is self-contained.

**Alternatives considered**:
- Bootstrap navbar collapse (JS) — would work but requires Bootstrap JS and a `<script>` include; adds implicit dependency.
- Pure CSS checkbox hack — avoids JS/Blazor state but is semantically poor and fails WCAG focus requirements.
- Bottom navigation bar — better UX but is a new design pattern, out of scope for a layout fix.

---

## Decision 3: Citytrip Overview Grid

**Decision**: Change `Citytrips.razor.css` from fixed `repeat(3, 1fr)` to responsive:
- `< 768px`: `repeat(1, 1fr)` (single column)
- `768px – 1024px`: `repeat(2, 1fr)` (two columns — satisfies tablet multi-column requirement from spec)
- `> 1024px`: `repeat(3, 1fr)` (existing desktop layout)

**Rationale**: The masonry grid is the most visible breakage on mobile. A single CSS media query change fixes it. The 2-column tablet view fulfils the spec requirement for multi-column on key screens.

**Alternatives considered**:
- CSS `auto-fill` / `minmax()` fluid grid — more elegant but harder to predict column count at target breakpoints; explicit breakpoints are clearer for testers.

---

## Decision 4: Wizard One-Step-Per-Screen on Mobile

**Decision**: The wizard already tracks the active step via Blazor state (`_currentStep`). On mobile (`< 768px`), apply CSS to make the active step fill the full viewport width. The existing step-rendering logic (which renders all step components but controls visibility) is unchanged. A CSS class `.wizard-step--active` (or equivalent already present) ensures only the active step is visible and full-width on mobile.

If the wizard renders only the active step via `@if` guards (to be confirmed when reading the razor files), no CSS change is needed — only width/padding fixes.

**Rationale**: Minimum invasive change. The wizard's step-control logic is already correct; only the CSS layout of each step needs to fill the screen on small viewports.

**Alternatives considered**:
- Separate mobile wizard routing (one URL per step) — significant architectural change, out of scope.
- CSS-only visibility via sibling selectors — fragile without knowing exact DOM structure.

---

## Decision 5: WCAG 2.1 AA Implementation Approach

**Decision**: Address WCAG 2.1 AA requirements through CSS additions to `app.css` and scoped component CSS:
1. **Focus indicators**: Add `:focus-visible` outlines to all interactive elements globally in `app.css` (minimum 2px solid, high contrast)
2. **Colour contrast**: Audit existing colour variables; update text/background pairs failing 4.5:1 (normal text) or 3:1 (large text/UI) ratios
3. **Touch targets**: Add `min-height: 44px; min-width: 44px` to button and link selectors in `app.css`
4. **Screen reader labels**: Add `aria-label` attributes to icon-only buttons in Razor components; add `aria-expanded` / `aria-controls` to the hamburger toggle

**Rationale**: Most WCAG AA requirements for this app are addressable via CSS (focus, contrast, sizing). The semantic/ARIA concerns are limited to the new hamburger toggle and any existing icon-only buttons. No JS is required.

**Alternatives considered**:
- Full WCAG audit tool integration — out of scope for this layout fix; a CSS-level fix is sufficient for the identified gaps.

---

## Decision 6: Test Strategy (bUnit)

**Decision**: Write bUnit tests for:
1. **NavMenu**: hamburger toggle renders correctly; `aria-expanded` attribute changes on toggle; nav links hidden/shown by CSS class.
2. **Citytrips page**: grid container renders with correct CSS class structure.
3. **Wizard**: active step renders with full-width class on mobile; Next/Back buttons are present and operable.

**Rationale**: bUnit tests cannot verify CSS visual rendering directly (no CSSOM in jsdom). Tests verify structural/class correctness — that the right HTML structure and CSS classes are in place for the responsive CSS to work. Visual regression at specific breakpoints is validated manually using browser DevTools.

CSS rendering is out of bUnit's scope; the tests ensure the markup structure that enables responsiveness is correct.

**Alternatives considered**:
- Playwright E2E tests for visual breakpoints — more thorough but no Playwright test project exists; adding one is out of scope for this fix.

---

## No NEEDS CLARIFICATION items remain

All unknowns were resolved during `/speckit.clarify`:
- Accessibility: WCAG 2.1 AA ✓
- Tablet layout: 2-column for overview + detail ✓
- Wizard on mobile: one step per screen ✓
- Target browsers: iOS Safari + Android Chrome ✓
- Load performance: < 3 seconds on 4G ✓
