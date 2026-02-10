# Feature Specification: Filter Citytrips

**Feature Branch**: `003-filter-citytrips`
**Created**: 2026-02-07
**Status**: Draft
**Input**: User description: "Users can filter on the list of citytrips on the main page. The filter options are the same as the visible available information."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Filter by Text (Priority: P1)

As a user browsing citytrips on the main page, I want to filter the list by typing a search term so that I can quickly find trips matching a specific title or destination.

**Why this priority**: Text search is the most versatile filter and covers the two primary text fields visible on each trip card (title and destination). It delivers immediate value by narrowing down a potentially long list.

**Independent Test**: Can be fully tested by typing a search term into the filter input and verifying that only trips whose title or destination contain the term are displayed.

**Acceptance Scenarios**:

1. **Given** a list of citytrips is displayed, **When** the user types "Paris" in the text filter, **Then** only trips with "Paris" in their title or destination are shown
2. **Given** a filtered list is displayed, **When** the user clears the text filter, **Then** all citytrips are shown again
3. **Given** a list of citytrips is displayed, **When** the user types a term that matches no trips, **Then** an empty state message is shown indicating no results match the filter
4. **Given** a list of citytrips is displayed, **When** the user types "par", **Then** trips containing "par" (case-insensitive) in title or destination are shown (e.g., "Paris", "Citytrip Paradise")

---

### User Story 2 - Filter by Date Range (Priority: P2)

As a user planning a trip, I want to filter citytrips by a date range so that I can find trips that fit my available travel dates.

**Why this priority**: Date filtering allows users to narrow trips by timing, which is a key decision factor when choosing a citytrip to enlist in.

**Independent Test**: Can be tested by selecting a start date and/or end date filter and verifying that only trips overlapping with the selected date range are displayed.

**Acceptance Scenarios**:

1. **Given** a list of citytrips is displayed, **When** the user selects a "from" date, **Then** only trips whose end date is on or after the selected date are shown
2. **Given** a list of citytrips is displayed, **When** the user selects a "to" date, **Then** only trips whose start date is on or before the selected date are shown
3. **Given** a list of citytrips is displayed, **When** the user selects both a "from" and "to" date, **Then** only trips that overlap with the selected date range are shown
4. **Given** date filters are applied, **When** the user clears the date filters, **Then** all citytrips are shown again

---

### User Story 3 - Combine Multiple Filters (Priority: P3)

As a user, I want to apply text and date filters together so that I can narrow down trips using multiple criteria simultaneously.

**Why this priority**: Combined filtering provides the most precise results but depends on individual filters (US1 and US2) being in place first.

**Independent Test**: Can be tested by entering a text term and a date range simultaneously and verifying that only trips matching all active criteria are displayed.

**Acceptance Scenarios**:

1. **Given** a list of citytrips is displayed, **When** the user types "Rome" and selects a date range of June 2026, **Then** only trips with "Rome" in the title or destination AND overlapping with June 2026 are shown
2. **Given** combined filters are applied showing no results, **When** the user removes one filter, **Then** the list updates to show trips matching the remaining filter(s)

---

### User Story 4 - Clear All Filters (Priority: P3)

As a user, I want a single action to reset all active filters so that I can quickly return to the full unfiltered list.

**Why this priority**: Convenience feature that improves usability when multiple filters are active.

**Independent Test**: Can be tested by applying multiple filters, clicking "Clear all", and verifying all filters are reset and the full list is displayed.

**Acceptance Scenarios**:

1. **Given** one or more filters are active, **When** the user clicks "Clear all filters", **Then** all filter inputs are reset and the full citytrip list is displayed
2. **Given** no filters are active, **Then** the "Clear all filters" action is not prominently shown or is disabled

---

### Edge Cases

- What happens when the user types only whitespace in the text filter? The system trims the input and treats it as empty (no filter applied).
- What happens when the "from" date is after the "to" date? The system prevents this by ensuring the "to" date cannot be earlier than the "from" date.
- What happens when filters result in zero matches? An empty state message is displayed indicating no trips match the current filters, with a suggestion to adjust or clear filters.
- What happens when the trip list is empty (no trips exist at all)? The existing empty state is shown; the filter controls may still be visible but have no effect.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display a filter area above the citytrip list on the main browse page
- **FR-002**: System MUST provide a text input that filters trips by matching against title and destination (case-insensitive, partial match)
- **FR-003**: System MUST provide a "from" date picker that excludes trips ending before the selected date
- **FR-004**: System MUST provide a "to" date picker that excludes trips starting after the selected date
- **FR-005**: System MUST apply all active filters together (AND logic) to narrow the displayed list
- **FR-006**: System MUST update the displayed list after a brief debounce delay (~300ms) once the user stops typing in the text filter; date picker changes apply immediately
- **FR-007**: System MUST provide a "Clear all filters" action that resets all filter inputs and shows the full list
- **FR-008**: System MUST show an empty state message when no trips match the active filters
- **FR-009**: System MUST prevent the user from selecting a "to" date earlier than the "from" date
- **FR-010**: System MUST preserve existing trip card interactions (like, enlist) while filters are active

### Key Entities

- **Filter Criteria**: Represents the active filter state consisting of an optional text term, an optional "from" date, and an optional "to" date. Applied client-side to the already-loaded trip list.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can locate a specific trip by name or destination within 5 seconds using the text filter
- **SC-002**: Filtered results update within 200 milliseconds of the user changing a filter value
- **SC-003**: 100% of existing trip card functionality (like, enlist, display) continues to work correctly when filters are active
- **SC-004**: Users can reset all filters to see the full list with a single action

## Clarifications

### Session 2026-02-07

- Q: Should the text filter apply live on every keystroke, debounced after a pause, or on explicit submit? → A: Debounced filtering (~300ms delay after user stops typing)

## Assumptions

- Filtering is performed client-side on the already-loaded list of trips (no server round-trip needed for filtering)
- The visible information on each trip card consists of: title, destination, start date, and end date — these are the filterable fields
- The trip card image is not considered a filterable field
- Like/enlist status are interaction states, not filter criteria
- The filter area is always visible on the main browse page, not hidden behind a toggle
