# Feature Specification: Citytrip Detail Page with Day Schedule and Map

**Feature Branch**: `006-trip-day-schedule`
**Created**: 2026-02-24
**Status**: Draft
**Input**: User description: "When users click on a citytrip, the app should navigate to a detail page. On the detail page, a day schedule will be listed in fashionly manner. This can include visits to places likes markets but also musea, stadions, etc. Make it generic so any type of event can be entered. There should also be a small google maps display with points on the places described in the day planning."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Navigate to Citytrip Detail (Priority: P1)

A user browsing the citytrip overview clicks on any trip card and is immediately taken to a dedicated detail page for that trip. The page displays the trip's key information prominently, confirming to the user they are viewing the correct trip.

**Why this priority**: Navigation to the detail page is the entry point for all other functionality. Without it, no other part of this feature is accessible.

**Independent Test**: Can be fully tested by clicking a citytrip card and verifying the correct detail page loads with the trip's name and destination.

**Acceptance Scenarios**:

1. **Given** a user is on the citytrip overview page, **When** they click on a citytrip, **Then** they are navigated to that trip's dedicated detail page
2. **Given** a user is on the detail page, **When** the page loads, **Then** the trip's name, destination, and date range are clearly displayed
3. **Given** a user is on the detail page, **When** they want to return, **Then** a back navigation option to the citytrip list is available

---

### User Story 2 - View Day Schedule (Priority: P2)

A user on the citytrip detail page sees a visually appealing, single scrollable page listing all days of the trip in sequence. Each day is introduced by a clearly styled date header (e.g. "Day 1 · Sat 15 Mar"), followed by all events for that day in chronological order. Each event card clearly shows what it is, when it happens, where it takes place, and what type of activity it is. The schedule works for any category of event — markets, museums, stadiums, restaurants, parks, and anything else.

**Why this priority**: The day schedule is the primary content and core value of the detail page. It is the main reason users navigate here.

**Independent Test**: Can be fully tested by viewing the detail page of a trip with pre-populated schedule events and verifying all events display in time order with type, name, time, and location visible.

**Acceptance Scenarios**:

1. **Given** a citytrip has scheduled events, **When** the detail page loads, **Then** all events are displayed in chronological order by start time
2. **Given** an event of any type (market, museum, stadium, restaurant, park, etc.), **When** displayed in the schedule, **Then** its event type label, name, start time, and location name are all visible
3. **Given** a citytrip with no scheduled events, **When** the detail page loads, **Then** a friendly empty-state message is shown indicating no events are planned yet
4. **Given** events with optional details (end time, description, notes), **When** those details are present, **Then** they are displayed alongside the required fields

---

### User Story 3 - View Map with Place Markers (Priority: P3)

A user on the detail page sees a compact, embedded map fixed as a sticky sidebar alongside the scrollable schedule. As the user scrolls through the day sections, the map dynamically updates to highlight only the markers for the day currently in view, giving a focused and continuously visible geographic reference while reading the schedule.

**Why this priority**: The map enriches the schedule view with spatial context but the schedule is fully usable without it, making it the third priority.

**Independent Test**: Can be fully tested by viewing a detail page with events that have location data and verifying the map renders with a correct marker for each location.

**Acceptance Scenarios**:

1. **Given** a citytrip detail page loads, **When** the page first renders, **Then** the map displays markers only for the first day's event locations
2. **Given** a user scrolls down so that a new day section enters the viewport, **When** the day section becomes the active section, **Then** the map updates to show only that day's event location markers
3. **Given** multiple events in the visible day at different locations, **When** viewing the map, **Then** each unique location has a distinct, visible marker
4. **Given** a marker on the map, **When** the user interacts with it, **Then** the name of the associated place or event is shown
5. **Given** an event in the visible day without geographic location data, **When** the map renders for that day, **Then** that event is omitted from the map without causing errors and still appears in the schedule
6. **Given** the map service is unavailable, **When** the page loads, **Then** the day schedule remains fully functional and a fallback message is shown in the map area

---

### Edge Cases

- What happens when a citytrip has no events in its schedule? (Friendly empty-state message shown; map area is hidden or shows a placeholder)
- How does the system handle an event with no geographic location? (Excluded from map markers; still shown fully in the schedule)
- What if multiple events are at the same location? (A single marker is shown for that location, indicating multiple events if interacted with)
- What if the mapping service is unavailable or fails to load? (Schedule remains fully readable; map area shows a graceful fallback message)
- What happens when a user navigates directly to a detail page URL for a non-existent trip? (User-friendly error message with navigation back to the list)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST navigate the user to the citytrip detail page when they click on a citytrip in the overview list
- **FR-002**: Detail page MUST display the citytrip's name, destination, and date range
- **FR-003**: Detail page MUST display all days of the trip on a single scrollable page, each day preceded by a clearly styled date header (e.g. "Day 1 · Sat 15 Mar"), with events listed chronologically within each day
- **FR-004**: Each event MUST display: a type icon paired with the event type label, event name, start time, and location name; when no specific icon exists for a given type label, a generic fallback icon MUST be shown
- **FR-005**: Each event MAY additionally display: end time, a short description, and free-text notes
- **FR-006**: The event model MUST support any type of event using a free-form label, with no predefined list enforced by the system
- **FR-007**: Detail page MUST display an embedded map that shows geographic markers only for the day section currently visible in the user's viewport; as the user scrolls to a new day section, the map MUST update to show that day's event locations
- **FR-007a**: On initial page load, the map MUST show markers for the first day's events
- **FR-008**: Each map marker MUST, when interacted with, display the name of the associated place or event
- **FR-009**: Events without geographic location data MUST still appear in the day schedule and MUST NOT cause errors in the map display
- **FR-010**: Detail page MUST show a clear empty-state message when a citytrip has no scheduled events
- **FR-011**: Detail page MUST provide navigation back to the citytrip overview
- **FR-013**: The map MUST be rendered as a sticky sidebar that remains fixed in the user's viewport while the schedule scrolls; the schedule and map MUST be displayed side by side in a two-column layout
- **FR-012**: If the map service is unavailable, the schedule MUST remain fully readable and functional; the map sidebar area MUST show a graceful fallback message

### Key Entities

- **Citytrip**: The main trip record. Key attributes: unique identifier, name, destination city, start date, end date.
- **DaySchedule**: An ordered collection of events for a given date within a citytrip. A citytrip may have one or more day schedules (one per day of the trip).
- **ScheduledEvent**: A single item in a day schedule. Key attributes: event type (free-form label), name, start time, optional end time, optional description, optional notes, optional associated place.
- **Place**: A point of interest linked to a scheduled event. Key attributes: name, address or description, geographic coordinates (latitude and longitude).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can navigate from the citytrip overview to the correct detail page with a single click; the page fully loads within 2 seconds
- **SC-002**: 100% of scheduled events for a citytrip are displayed in correct chronological order on the detail page
- **SC-003**: Any event type (market, museum, stadium, restaurant, park, or other) displays correctly in the schedule layout without visual anomalies
- **SC-004**: All events with valid location data have a corresponding map marker; no errors occur when events lack location data
- **SC-005**: The map sidebar is visible at all times while the user reads the schedule; users can identify the location of any place in the current day's section without scrolling or switching views
- **SC-006**: The day schedule remains fully readable and functional when the map service is unavailable

## Clarifications

### Session 2026-02-24

- Q: When a citytrip spans multiple days, how does the user navigate between or see all days on the detail page? → A: Single scrollable page — all days listed sequentially, each preceded by a date header
- Q: What does the embedded map show when a citytrip spans multiple days? → A: Day-synced — map highlights markers for the day section currently in the user's viewport as they scroll
- Q: Do different event types get distinct visual cues (icon or colour), or is the type shown as a text label only? → A: Icon + label — each event shows a small icon representing its type alongside the free-form type label; unknown types fall back to a generic icon
- Q: Where is the map positioned relative to the schedule on the detail page? → A: Sticky sidebar — map is fixed alongside the schedule in a two-column layout; remains in view while the user scrolls

## Assumptions

- A citytrip may span multiple days; day schedules are grouped by date. The initial implementation may seed a single day's events, but the data model supports multiple days.
- The detail page is read-only for this feature. Adding, editing, or removing events is out of scope and handled by a future feature.
- A Google Maps-compatible mapping service is used for the embedded map. An API key or equivalent access credential is assumed to be available in the project environment as a configuration setting.
- Event types are free-form text labels (e.g., "Museum", "Market", "Stadium") — no enforced category list exists.
- Geographic coordinates for places are stored with the place data. Real-time address geocoding is out of scope for this feature.
- Initial schedule data will be seeded or mocked for development and testing, consistent with the existing in-memory repository patterns in the project.
