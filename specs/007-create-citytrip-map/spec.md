# Feature Specification: Create Citytrip with Full Detail Fields and Map-Based Location Picker

**Feature Branch**: `007-create-citytrip-map`
**Created**: 2026-02-24
**Status**: Draft
**Input**: User description: "When creating a new citytrip, make sure the same fields as the details page can be entered by the user. It should also allow to locate locations on the map to put a pin on it. Use 007 as feature number"

## Clarifications

### Session 2026-02-24

- Q: What is the form layout for the Create Citytrip form? → A: Multi-step wizard — Step 1: trip basics, Step 2: day plans and scheduled events, Step 3: review and confirm.
- Q: When the user changes the date range in Step 1 after events have been entered in Step 2, what happens? → A: Show a confirmation prompt before discarding entered events ("Changing the dates will clear your event schedule. Continue?").
- Q: How is the map picker presented when the user clicks "Pick on map" in Step 2? → A: Modal overlay — a dedicated large modal opens on top of Step 2, keeping the event list undisturbed.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Fill in Citytrip Basics (Priority: P1)

As a user creating a new citytrip, I want to enter the same rich set of fields that appear on the detail page — title, destination, date range, description, max participants, and image — so that the trip is fully described from the moment it is created.

**Why this priority**: This is the foundational creation flow (wizard Step 1). Without a complete set of core fields, trips created through the form would appear incomplete on the detail page. All other stories depend on a trip existing.

**Independent Test**: Navigate to the "My Citytrips" page, start the Create wizard, fill in all Step 1 basic fields (title, destination, start/end date, description, max participants, image URL), proceed to Step 3 and confirm, then verify the new trip appears in the list and the detail page shows all entered values correctly.

**Acceptance Scenarios**:

1. **Given** the Create wizard Step 1 is open, **When** the user fills in title, destination, start date, end date, description, max participants, and image URL and advances, **Then** the inputs are retained and the wizard moves to Step 2.
2. **Given** Step 1 of the wizard, **When** the user omits a required field (title, destination, or start date) and tries to advance, **Then** a validation message is shown and the wizard does not advance.
3. **Given** Step 1 of the wizard, **When** the user enters an end date earlier than the start date and tries to advance, **Then** a validation message is shown and the wizard does not advance.
4. **Given** a completed wizard that was confirmed on Step 3, **When** the user navigates to the new trip's detail page, **Then** all fields entered in Step 1 are displayed correctly.

---

### User Story 2 - Add Day Plans with Scheduled Events (Priority: P2)

As a user creating a new citytrip, I want to use Step 2 of the wizard to add scheduled events to each day plan — providing event type, name, start time, optional end time, and optional description — so that the trip itinerary is structured from the start.

**Why this priority**: The day schedule is the primary content of a trip's detail page (feature 006). Wizard Step 2 keeps this content separate from the basic fields and is only reachable after Step 1 is valid.

**Independent Test**: Complete Step 1 of the wizard, advance to Step 2, and add at least one event to two different days (providing type, name, and start time). Proceed to Step 3, confirm, and verify the detail page shows both day sections with correct event data in chronological order.

**Acceptance Scenarios**:

1. **Given** wizard Step 2 is active, **When** the user adds a scheduled event with type, name, and start time to a day, **Then** the event appears in that day's section of the form.
2. **Given** a day section in Step 2 with multiple events, **When** events are added in any order, **Then** they are displayed in start-time order within that day.
3. **Given** a scheduled event row in Step 2, **When** the user provides an end time earlier than or equal to the start time, **Then** a validation message is shown.
4. **Given** a scheduled event row with all optional fields (end time, description), **When** the wizard is confirmed and the detail page is opened, **Then** all optional event fields are displayed correctly.
5. **Given** a day slot in Step 2 with no events added, **When** the wizard is confirmed, **Then** the day section is still present on the detail page (empty state handled gracefully).
6. **Given** wizard Step 2, **When** the user clicks "Back", **Then** the wizard returns to Step 1 with all previously entered values intact.

---

### User Story 3 - Locate Places on the Map to Pin Coordinates (Priority: P3)

As a user adding a scheduled event in wizard Step 2, I want to open a map picker and place a pin at the event's location, so that the correct geographic coordinates are recorded and the place appears as a marker on the trip's detail map.

**Why this priority**: The map-based location picker adds significant value but requires US2 (events must exist in Step 2) and the Google Maps integration from feature 006. It can be deferred without blocking the core creation flow.

**Independent Test**: Complete Step 1 of the wizard. In Step 2, add one event and use the map picker to place a pin. Confirm at Step 3 and verify the detail page map shows a marker at the expected coordinates with the place name.

**Acceptance Scenarios**:

1. **Given** a scheduled event row in Step 2, **When** the user clicks "Pick on map", **Then** a modal overlay opens with a full map where the user can click to place a pin.
2. **Given** the map picker is open, **When** the user clicks a location on the map, **Then** a pin is placed and the place coordinates (latitude/longitude) and a place name are recorded for the event.
3. **Given** coordinates are recorded for an event, **When** the wizard is confirmed and the detail page is opened, **Then** the event's location appears as a marker on the detail-page map.
4. **Given** the map picker is open, **When** the Google Maps API is unavailable, **Then** a fallback message is shown and the user can still save the event without a location.
5. **Given** an event with a previously pinned location in Step 2, **When** the user re-opens the map picker modal, **Then** the existing pin is shown at the recorded coordinates.

---

### Edge Cases

- What happens when a user submits the form but the Google Maps API fails during location picking? The event is saved without a location; the map picker shows an error fallback.
- What happens when max participants is set to zero or a negative number? Validation rejects the value with an appropriate message on Step 1.
- How does the system handle duplicate day numbers? Day plan slots are auto-generated from the date range in sequential order; the UI prevents manual reordering or duplication.
- What happens when the user adds a day plan but no events to it and confirms? The day plan is saved with an empty event list; the detail page shows the day section with an empty-state message.
- What happens when an image URL is provided that is not a valid URL format? Validation shows an error on Step 1; invalid images are not saved.
- What happens when the user navigates back from Step 2 to Step 1 and changes the date range? Day plan slots in Step 2 are regenerated to match the new range; events already entered are discarded with a confirmation prompt.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Create Citytrip wizard MUST present trip basics in Step 1: trip title, destination, start date, end date, optional description, optional max participants (positive integer), and optional image URL.
- **FR-002**: The system MUST validate that title, destination, and start date are provided before advancing from Step 1.
- **FR-003**: The system MUST validate that end date is on or after start date before advancing from Step 1.
- **FR-004**: The system MUST validate that max participants, when provided, is a positive integer before advancing from Step 1.
- **FR-005**: Wizard Step 2 MUST display one day plan slot per calendar day between the start and end date entered in Step 1, allowing users to add events to each slot.
- **FR-006**: Each day plan slot in Step 2 MUST allow adding one or more scheduled events, each with: event type (free text), name, and start time as required fields; end time and description as optional fields.
- **FR-007**: The system MUST validate that end time, when provided for an event, is strictly after start time.
- **FR-008**: Scheduled events within a day plan slot MUST be displayed in start-time order.
- **FR-009**: Each scheduled event MAY have an associated place with a name and geographic coordinates (latitude/longitude).
- **FR-010**: Users MUST be able to open a map picker modal per event in Step 2 to place a pin and capture the place name and coordinates. The modal opens on top of Step 2 without disrupting the event list.
- **FR-011**: Placing a pin on the map MUST record the geographic coordinates and populate a place name (from reverse geocoding or manual entry).
- **FR-012**: The map picker MUST show an error fallback when the map service is unavailable; the event MUST still be saveable without a location.
- **FR-013**: Wizard Step 3 MUST show a read-only summary of all entered data (trip basics and day plan events) before the user confirms and saves.
- **FR-014**: When the wizard is confirmed on Step 3, all day plans and events (with or without places) MUST persist and appear correctly on the detail page.
- **FR-015**: The wizard MUST provide "Back" and "Next" navigation between steps, preserving all entered data when navigating back to a previous step.
- **FR-016**: If the user changes the date range in Step 1 (by navigating back), the day plan slots in Step 2 MUST be regenerated; if events have already been entered, the user MUST be prompted to confirm that existing event data will be discarded.

### Key Entities *(include if feature involves data)*

- **Citytrip**: The main entity being created. Has: title (required), destination (required), start date (required), end date (required), description (optional), max participants (optional, positive integer), image URL (optional), and a list of day plans.
- **DayPlan**: One plan per calendar day between start and end date. Has: day number (sequential), date, and a list of scheduled events.
- **ScheduledEvent**: An activity or point of interest within a day plan. Has: event type (free text, e.g., "museum"), name, start time (required), end time (optional), description (optional), and an optional associated place.
- **Place**: Geographic location for a scheduled event. Has: name, latitude, and longitude. Captured via map pin or manual entry.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can complete wizard Step 1 (all basic fields) in under 2 minutes.
- **SC-002**: A user can complete Step 2 with two scheduled events (one with a map-pinned location) in under 5 minutes.
- **SC-003**: 100% of fields visible on the detail page can be entered during the creation wizard — no fields require a separate edit step after creation.
- **SC-004**: All data entered across all wizard steps appears correctly on the detail page immediately after confirming, with no data loss.
- **SC-005**: The map location picker works correctly when the map service is available; when unavailable, the fallback is shown and trip creation can still complete without a location.
- **SC-006**: Form validation on each wizard step prevents advancing with invalid data, with clear, user-readable error messages for each violation.
- **SC-007**: Navigating back to a previous step in the wizard preserves all data entered in later steps (unless a date change forces regeneration with user confirmation).

## Assumptions

- The Google Maps API key configured in `appsettings.Development.json` is used for both the detail-page map and the creation-form map picker — no additional configuration is needed.
- Day plan slots in Step 2 are auto-generated from the date range entered in Step 1 (one slot per calendar day). Users do not create day plans manually.
- Place name is either provided by reverse geocoding (if the map API supports it) or entered manually by the user after placing a pin. If reverse geocoding is not available, a text field for the place name is shown alongside the map.
- The image URL field accepts any valid URL; no file upload is supported in this feature.
- The creator is determined from the current session (no explicit creator field in the wizard).
- Max participants is optional; if left blank, the trip has no participant cap.
- The existing `TripFormModal` is replaced by the new multi-step wizard route; the wizard is reached from the same "Create" entry point on the My Citytrips page.
