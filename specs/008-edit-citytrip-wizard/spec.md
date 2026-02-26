# Feature Specification: Edit Citytrip via Wizard

**Feature Branch**: `008-edit-citytrip-wizard`
**Created**: 2026-02-25
**Status**: Draft
**Input**: User description: "When a user wants to edit a citytrip, the application should provide the same wizard as creating one, with the known data already filled in."

## Clarifications

### Session 2026-02-25

- Q: When the date range changes, how should surviving events be re-mapped to the new day slots? → A: Preserve events by day number — Day 1 events always stay on Day 1, Day 2 on Day 2, etc., regardless of which calendar date those day numbers now represent. Events on day numbers that exceed the new trip length are discarded.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Edit Citytrip Basics (Priority: P1)

A trip owner navigates to their trip list, clicks "Edit" on one of their trips, and is taken to the same multi-step wizard used for creating a trip. Step 1 (Basics) is pre-filled with the trip's existing title, destination, dates, description, max participants, and image URL. The owner can change any field and advance through the wizard. On the Review step they see a summary of all their changes and can confirm to save.

**Why this priority**: Editing the basic trip details is the most common edit action. Without this, users must delete and recreate a trip to fix even a typo. Delivering this alone provides immediate user value.

**Independent Test**: Navigate to My Citytrips → click Edit on any existing trip → wizard opens at Step 1 with all basic fields pre-filled → change the title → advance to Step 3 → confirm → verify the trip card on My Citytrips shows the updated title.

**Acceptance Scenarios**:

1. **Given** a trip owner on the My Citytrips page, **When** they click "Edit" on a trip, **Then** the wizard opens at Step 1 with title, destination, start date, end date, description, max participants, and image URL all pre-filled with the trip's current values.
2. **Given** the wizard is open in edit mode at Step 1, **When** the owner changes the title and clicks Next, **Then** they advance to Step 2 with their change retained.
3. **Given** the owner is on the Review step (Step 3), **When** they click "Save Changes", **Then** the trip is updated and they are returned to the My Citytrips page showing the updated values.
4. **Given** the wizard is open in edit mode, **When** the owner clicks Back from Step 2 to Step 1, **Then** all previously entered changes are still present in the form.

---

### User Story 2 - Edit Day Plans and Events (Priority: P2)

A trip owner editing a trip can also view and modify the scheduled events across all day plans in Step 2. Existing events for each day are pre-loaded in the editor. The owner can add new events, remove existing ones, and update event details. Step 3 reflects all changes before saving.

**Why this priority**: Day plans contain the itinerary — a core part of the trip data. Owners need to be able to adjust their schedule without losing other event data. Editing basics alone is not sufficient for a full edit experience.

**Independent Test**: Edit a trip that already has events → open Step 2 → verify each day's events are pre-filled → change one event's name → advance to Step 3 → review shows the updated event name → confirm → detail page shows the updated event.

**Acceptance Scenarios**:

1. **Given** a trip with existing day plan events, **When** the owner opens the edit wizard at Step 2, **Then** all existing events appear in the correct day slots, pre-filled with their type, name, start time, end time, and description.
2. **Given** an event row is displayed in Step 2, **When** the owner changes the event name and advances to Step 3, **Then** the review shows the updated event name.
3. **Given** an event row is displayed in Step 2, **When** the owner clicks the remove button, **Then** that event row is removed from the day slot.
4. **Given** a day with no events, **When** the owner clicks "Add Event", **Then** a new empty event row is added to that day.
5. **Given** an event has a place pin, **When** the owner views that event in the editor, **Then** the place badge is shown and the owner can update it using the map picker.

---

### User Story 3 - Date Range Change Warning on Edit (Priority: P3)

When a trip owner changes the trip's start or end date in Step 1 of the edit wizard, and the trip already has scheduled events, the system warns them that changing the dates will affect the day plan structure. The owner must confirm before proceeding; if they cancel, the original dates are restored.

**Why this priority**: Changing dates restructures the day plan slots. Without a warning, owners could accidentally wipe event data they spent time entering. This is a safety guard, not core edit functionality.

**Independent Test**: Edit a trip with events → change the end date to add or remove a day → confirm the warning prompt → advance to Step 2 → verify the day structure matches the new date range (and events on removed days are gone).

**Acceptance Scenarios**:

1. **Given** a trip with events in Step 1 of the edit wizard, **When** the owner changes the start or end date and clicks Next, **Then** a confirmation prompt appears warning that the schedule will be adjusted.
2. **Given** the date change warning prompt appears, **When** the owner cancels, **Then** the original dates are restored and no day plans are modified.
3. **Given** the date change warning prompt appears, **When** the owner confirms, **Then** the day plan structure is regenerated for the new date range; events are preserved by day number (Day 1 events stay on Day 1, Day 2 events stay on Day 2, etc.); events on day numbers that exceed the new trip length are discarded.

---

### Edge Cases

- What happens when a trip owner tries to edit a trip they do not own (e.g., via direct URL)? The system must reject the request and show an appropriate message.
- What happens if the owner changes both the start and end date, reducing the trip from 5 days to 2? Events on Day numbers 3–5 are discarded; events on Day 1 and Day 2 are preserved on their day number positions regardless of the new calendar dates.
- What happens if the owner navigates away from the wizard without saving? Changes are discarded; no partial save occurs.
- What if the trip has no existing day plans or events? Step 2 shows empty day slots for the current date range, ready to add events.
- What if the owner removes all events from a day? That day remains in the list with an empty state label.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow a trip owner to open the edit wizard from the My Citytrips page for any trip they own.
- **FR-002**: The edit wizard MUST present the same three-step structure (Basics → Itinerary → Review) as the create wizard.
- **FR-003**: Step 1 of the edit wizard MUST pre-fill all basic trip fields (title, destination, start date, end date, description, max participants, image URL) with the trip's current saved values.
- **FR-004**: Step 2 of the edit wizard MUST pre-fill all existing day plan slots and their scheduled events (type, name, start time, end time, description, and place if present).
- **FR-005**: The Review step MUST display a summary of all current field values (including any unsaved changes) before the owner confirms.
- **FR-006**: On confirmation, the system MUST save all changes (basics and day plans) to the existing trip record — not create a new trip.
- **FR-007**: System MUST restrict edit access to the trip owner only; attempts by other users to edit a trip they do not own MUST be rejected.
- **FR-008**: When the owner changes the trip's date range and existing events are present, the system MUST show a confirmation prompt before restructuring the day plans.
- **FR-009**: If the owner cancels the date change confirmation, the original dates MUST be restored with no changes to the day plan structure.
- **FR-010**: If the owner confirms the date change, day slots MUST be regenerated for the new date range; events are re-mapped by day number (Day N events remain on Day N); events on day numbers that exceed the new trip length MUST be discarded.
- **FR-011**: If the owner navigates away without confirming (closes wizard or clicks Cancel), all unsaved changes MUST be discarded and the trip data MUST remain unchanged.
- **FR-012**: All validation rules from the create wizard (required fields, date ordering, end time after start time, valid image URL) MUST apply equally in the edit wizard.

### Key Entities

- **Citytrip**: The trip being edited — identified by its ID. Fields: title, destination, start date, end date, description, max participants, image URL, owner ID.
- **DayPlan**: One slot per calendar day of the trip — identified by day number and date. Contains an ordered list of scheduled events.
- **ScheduledEvent**: A single event within a day — fields: type, name, start time, optional end time, optional description, optional place pin (name, latitude, longitude).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A trip owner can open the edit wizard, make changes to basics and events, and save — completing the full edit flow in under 3 minutes for a typical 3-day trip.
- **SC-002**: 100% of existing trip data is correctly pre-filled when the edit wizard opens; no field is blank when the original trip had a value for it.
- **SC-003**: After saving, 100% of the changed values are reflected on the My Citytrips page and the trip detail page without requiring a page refresh.
- **SC-004**: The date change warning prevents accidental data loss in all cases where events exist on days being removed.
- **SC-005**: Attempts by non-owners to access the edit wizard for a trip they do not own are blocked 100% of the time.

## Assumptions

- The edit wizard reuses the same step components (WizardStep1, WizardStep2, WizardStep3) as the create wizard; differences in behaviour (pre-fill, save vs. create) are handled by the page-level orchestrator.
- The "Edit" entry point is the existing Edit button on trip cards in the My Citytrips page; no additional entry points (e.g., from the detail page) are in scope for this feature.
- Concurrent editing (two owners editing the same trip simultaneously) is out of scope; last-write-wins is acceptable.
- Only the trip owner can edit a trip; enlisted participants cannot.
- The map picker (Step 2 place selection) behaves identically in edit mode as in create mode.
