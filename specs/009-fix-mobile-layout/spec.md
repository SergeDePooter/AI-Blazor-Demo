# Feature Specification: Mobile & Tablet Responsive Layout

**Feature Branch**: `009-fix-mobile-layout`
**Created**: 2026-03-02
**Status**: Draft
**Input**: User description: "The application should run on mobile devices as well. Now, the layout is not working on a mobile device or tablet. Fix this"

## Clarifications

### Session 2026-03-02

- Q: What accessibility compliance level must the responsive layout meet? → A: WCAG 2.1 AA — touch targets, focus indicators, colour contrast, screen reader labels.
- Q: Should tablet screens get a distinctly different multi-column layout or just wider single-column? → A: Multi-column for key screens — citytrip list and detail pages use a 2-column layout on tablet; all other screens remain single-column.
- Q: How should the multi-step citytrip create/edit wizard present on mobile? → A: One step per screen — each wizard step occupies the full screen; users progress with Next/Back controls.
- Q: Which mobile browsers are in scope for acceptance testing? → A: iOS Safari + Android Chrome — both platforms, covers ~95% of real mobile users.
- Q: Should a page-load time target on mobile networks be part of the acceptance criteria? → A: Under 3 seconds on a 4G connection.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Browse Citytrips on a Smartphone (Priority: P1)

A traveller opens the Citytrip Planner on their smartphone. They expect to see a well-structured list of available citytrips that is easy to scroll and read. Currently the layout overflows or collapses in ways that make content unreadable or inaccessible.

**Why this priority**: The browse/overview screen is the entry point for every user. If it is broken on mobile, all other functionality is effectively inaccessible to mobile users.

**Independent Test**: Can be fully tested by loading the citytrip overview on a real or simulated smartphone (portrait and landscape) and verifying that all content is readable, clickable, and no horizontal scroll bar appears.

**Acceptance Scenarios**:

1. **Given** a user on a smartphone in portrait mode, **When** they open the citytrip overview, **Then** all citytrip cards are displayed in a single-column layout that fits the screen width without horizontal overflow.
2. **Given** a user on a smartphone in landscape mode, **When** they browse the citytrip overview, **Then** the layout reflows appropriately and no content is cut off.
3. **Given** a user on a smartphone, **When** they tap any interactive element (button, link, card), **Then** the tap target is large enough to be operated accurately with a finger.

---

### User Story 2 - View Citytrip Details and Day Schedule on a Smartphone (Priority: P2)

A traveller selects a citytrip on their smartphone and navigates through the day-by-day schedule. They expect to read event details, see the map area, and scroll through the day plan without layout breakage.

**Why this priority**: After the overview, the detail and schedule views are the most-used screens. Fixing them ensures the core planning workflow is usable end-to-end on mobile.

**Independent Test**: Can be fully tested by opening a citytrip detail page and day schedule on a smartphone, verifying all sections are legible, scrollable, and no elements overlap or overflow.

**Acceptance Scenarios**:

1. **Given** a user on a smartphone, **When** they open a citytrip detail page, **Then** all sections (header, description, actions) are stacked vertically and fully visible without horizontal scrolling.
2. **Given** a user on a smartphone, **When** they view the day schedule, **Then** each event entry is displayed in a readable single-column format with no truncated or overlapping text.
3. **Given** a user on a smartphone, **When** a map is shown on the schedule page, **Then** the map area scales to fit the screen width and does not break surrounding layout.

---

### User Story 3 - Use the Application on a Tablet (Priority: P3)

A traveller uses the application on a tablet (portrait or landscape). They expect the citytrip overview and citytrip detail pages to use a two-column layout that takes advantage of the larger screen. All other screens adapt to the available width but remain single-column.

**Why this priority**: Tablet users represent a secondary audience. The layout should be functional and comfortable, making good use of the available space.

**Independent Test**: Can be fully tested by loading all main screens on a simulated tablet in both orientations and verifying that the layout is appropriately wider than the mobile view and free of overflow or misalignment.

**Acceptance Scenarios**:

1. **Given** a user on a tablet in portrait mode, **When** they open any screen, **Then** the layout adapts to the tablet width — content is wider than on mobile and no horizontal overflow occurs.
2. **Given** a user on a tablet in landscape mode, **When** they view the citytrip overview or detail page, **Then** the content is displayed in a two-column layout without elements overlapping.
3. **Given** a user switching between portrait and landscape on a tablet, **When** the orientation changes, **Then** the layout reflows cleanly without requiring a page refresh.

---

### User Story 4 - Navigate Between Pages on Any Device (Priority: P4)

A user on a smartphone or tablet expects the navigation menu/header to be accessible and usable regardless of screen size.

**Why this priority**: Navigation is required to move between all features. If it breaks on mobile or tablet, users are stranded on a single page.

**Independent Test**: Can be fully tested by verifying the navigation header/menu is visible and operable on both small and large mobile screens in portrait and landscape mode.

**Acceptance Scenarios**:

1. **Given** a user on a smartphone, **When** they open any page, **Then** the navigation header is visible and all navigation links are reachable (collapsed menu, hamburger, or similar pattern if needed).
2. **Given** a user on a tablet, **When** they open any page, **Then** the navigation is displayed in a layout appropriate for the screen size and all links are accessible.
3. **Given** a user on any mobile device, **When** they navigate to a different page, **Then** the active page is clearly indicated in the navigation.

---

### Edge Cases

- What happens when the device is very narrow (320 px wide, e.g. iPhone SE)? All content must still be readable without horizontal scroll.
- How does the layout handle very long citytrip names or event descriptions on a small screen? Text must wrap rather than overflow or truncate silently.
- What if the user rotates their device mid-session (e.g. while viewing the map)? The layout must reflow without loss of context or data.
- What if a user zooms in on mobile (browser pinch-to-zoom or text size settings)? Content must remain accessible even at 150% zoom.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The application MUST display all pages correctly on screens as narrow as 320 px without horizontal scrolling or content overflow.
- **FR-002**: The application MUST adapt its layout to common mobile screen sizes (smartphones and tablets) using fluid, responsive layout behaviour.
- **FR-003**: The application MUST reflow its layout automatically when the device orientation changes between portrait and landscape without requiring a page reload.
- **FR-004**: All interactive elements (buttons, links, navigation items) MUST have touch targets large enough to be reliably tapped with a finger (minimum 44x44 points), and MUST comply with WCAG 2.1 AA — including visible focus indicators, sufficient colour contrast (4.5:1 for normal text, 3:1 for large text/UI components), and accessible labels readable by screen readers.
- **FR-005**: Navigation MUST remain accessible and fully operable on all supported screen sizes; if the full navigation bar does not fit, it MUST be presented in a collapsed or alternative form that users can open.
- **FR-006**: Text content MUST wrap within its container on small screens and MUST NOT overflow or be clipped.
- **FR-007**: Images and map areas MUST scale to fit the available screen width without breaking the surrounding layout.
- **FR-008**: The layout MUST function correctly on tablets (768 px to 1024 px wide) in both portrait and landscape orientations. The citytrip overview and citytrip detail pages MUST use a two-column layout on tablet viewports; all other pages MUST adapt to the available width while remaining single-column.
- **FR-009**: The application MUST maintain full functional parity on mobile and tablet — all features available on desktop MUST also be accessible on small screens.
- **FR-010**: On mobile viewports, the citytrip create/edit wizard MUST present one step at a time, each step occupying the full screen width, with clearly visible Next and Back controls that allow users to progress through or return to any step.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All pages load and display correctly on a 320 px wide screen (portrait) with no horizontal scroll bar and no clipped content — verified by visual inspection on at least three common smartphone screen sizes in both iOS Safari and Android Chrome simulators.
- **SC-002**: All pages display correctly on a tablet screen (768 px wide portrait; 1024 px wide landscape) with no layout overflow or misaligned elements.
- **SC-003**: Every interactive element on every page meets WCAG 2.1 AA standards on mobile viewports: minimum 44x44 pt touch targets, visible focus indicators, colour contrast ratios of at least 4.5:1 for body text and 3:1 for large text and UI components, and accessible labels present on all controls.
- **SC-004**: The navigation is fully operable on a smartphone without zooming or scrolling sideways — users can reach any page in at most 2 taps from any screen.
- **SC-005**: Rotating the device between portrait and landscape on any page produces a correct layout reflow within 1 second, with no content loss or page reload required.
- **SC-006**: 100% of the application's features available on a desktop-sized screen are also reachable and usable on a 375 px wide smartphone screen.
- **SC-007**: Every page of the application loads and becomes interactive within 3 seconds on a simulated 4G mobile connection, in both iOS Safari and Android Chrome.

## Assumptions

- The target mobile screen range is 320 px to 428 px wide (smartphones) and 768 px to 1024 px wide (tablets). Screens outside this range are out of scope for this fix.
- Desktop behaviour (screens wider than 1024 px) is currently working correctly and must not be regressed.
- The fix applies to all existing pages: citytrip overview, citytrip detail, day schedule, create/edit wizard, map view, and user profile. On mobile, the create/edit wizard presents one step per screen (full-screen per step with Next/Back navigation); the desktop multi-column wizard layout is not used on small screens.
- No new features or content changes are in scope — only layout and styling adjustments to make existing content respond to smaller screens.
- Physical device testing is not required; testing on browser-based device simulators at the defined viewport sizes is sufficient for acceptance. Simulators MUST cover iOS Safari and Android Chrome as the two target mobile browsers.
