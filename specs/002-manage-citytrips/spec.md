# Feature Specification: Manage Citytrips

**Feature Branch**: `002-manage-citytrips`
**Created**: 2026-02-07
**Status**: Draft
**Input**: User description: "The users in the application have their own page where they can manage their own created citytrips (create, edit or delete). On this page they can also view their enlisted citytrips. The design of the page is split up in 2 parts, so either their own citytrips or their enlisted citytrips are shown."

## Clarifications

### Session 2026-02-07

- Q: What are the complete fields for creating/editing a citytrip? → A: Title, destination, start date, end date, description, max participants.
- Q: How should the create/edit form be presented? → A: Modal/dialog overlay on the current page.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View My Created Citytrips (Priority: P1)

As a user, I can navigate to my personal citytrips page and see a list of all citytrips I have created. The page defaults to showing my own created citytrips. Each citytrip displays its key information (title, destination, dates, number of participants) so I can quickly assess my trips.

**Why this priority**: This is the foundation of the page — users need to see their own citytrips before they can manage them. Without this view, no other actions are possible.

**Independent Test**: Can be fully tested by navigating to the "My Citytrips" page and verifying that only the logged-in user's created citytrips are displayed with relevant details.

**Acceptance Scenarios**:

1. **Given** I am a logged-in user with 3 created citytrips, **When** I navigate to the "My Citytrips" page, **Then** I see all 3 of my created citytrips with their title, destination, and dates.
2. **Given** I am a logged-in user with no created citytrips, **When** I navigate to the "My Citytrips" page, **Then** I see an empty state message encouraging me to create my first citytrip.
3. **Given** I am a logged-in user, **When** I navigate to the "My Citytrips" page, **Then** the "My Citytrips" section is shown by default (not the enlisted section).

---

### User Story 2 - Create a New Citytrip (Priority: P1)

As a user, I can create a new citytrip from my personal page. I provide the essential details for the citytrip and it is saved and appears in my list of created citytrips.

**Why this priority**: Creating citytrips is a core action — users must be able to add new trips to have content to manage.

**Independent Test**: Can be fully tested by clicking a "Create Citytrip" action, filling in the required fields, submitting, and verifying the new citytrip appears in the list.

**Acceptance Scenarios**:

1. **Given** I am on the "My Citytrips" page, **When** I initiate the create citytrip action, **Then** a modal dialog opens with a form to enter citytrip details (title, destination, start date, end date, description, max participants).
2. **Given** I have filled in all required citytrip fields, **When** I submit the form, **Then** the citytrip is created and appears in my created citytrips list.
3. **Given** I have left required fields empty, **When** I submit the form, **Then** I see clear validation messages indicating which fields need to be completed.

---

### User Story 3 - Edit an Existing Citytrip (Priority: P2)

As a user, I can edit a citytrip I have created. I can update any of its details and save the changes.

**Why this priority**: Editing allows users to correct mistakes and keep trip information up to date, which is essential for a good management experience.

**Independent Test**: Can be fully tested by selecting an existing citytrip, modifying its details, saving, and verifying the changes are persisted.

**Acceptance Scenarios**:

1. **Given** I am viewing my created citytrips, **When** I choose to edit a citytrip, **Then** a modal dialog opens with a form pre-filled with the current citytrip details.
2. **Given** I have modified citytrip details, **When** I save the changes, **Then** the updated information is reflected in my citytrips list.
3. **Given** I am editing a citytrip, **When** I cancel the edit, **Then** no changes are saved and I return to the citytrips list.

---

### User Story 4 - Delete a Citytrip (Priority: P2)

As a user, I can delete a citytrip I have created. The system asks for confirmation before permanently removing the citytrip.

**Why this priority**: Deletion is needed for housekeeping, but is less frequent than viewing or editing. Confirmation prevents accidental data loss.

**Independent Test**: Can be fully tested by selecting a citytrip for deletion, confirming, and verifying it no longer appears in the list.

**Acceptance Scenarios**:

1. **Given** I am viewing my created citytrips, **When** I choose to delete a citytrip, **Then** I am asked to confirm the deletion.
2. **Given** I confirm the deletion, **When** the system processes the request, **Then** the citytrip is permanently removed from my list.
3. **Given** I cancel the deletion confirmation, **When** the dialog closes, **Then** the citytrip remains unchanged in my list.

---

### User Story 5 - View My Enlisted Citytrips (Priority: P2)

As a user, I can switch to the "Enlisted Citytrips" section to see all citytrips I have enlisted in (created by other users). This section is read-only — I can view trip details but cannot edit or delete them.

**Why this priority**: Viewing enlisted trips is an important part of the page, giving users a complete picture of all their trip involvement, but it is secondary to managing their own trips.

**Independent Test**: Can be fully tested by switching to the "Enlisted" section and verifying that only citytrips the user has enlisted in are displayed, with no edit/delete actions available.

**Acceptance Scenarios**:

1. **Given** I am on the "My Citytrips" page showing my created trips, **When** I switch to the "Enlisted Citytrips" section, **Then** I see a list of citytrips I have enlisted in.
2. **Given** I am viewing my enlisted citytrips, **When** I look at a trip entry, **Then** I can see its details but no edit or delete options are available.
3. **Given** I have no enlisted citytrips, **When** I switch to the "Enlisted" section, **Then** I see an empty state message indicating I haven't enlisted in any trips yet.

---

### User Story 6 - Toggle Between Sections (Priority: P1)

As a user, I can switch between the "My Citytrips" and "Enlisted Citytrips" sections. The page displays one section at a time, making the interface clean and focused.

**Why this priority**: The toggle mechanism is fundamental to the page layout — it's the primary navigation within this page and is needed for all other stories to function together.

**Independent Test**: Can be fully tested by switching between the two sections and verifying only the selected section's content is visible.

**Acceptance Scenarios**:

1. **Given** I am viewing the "My Citytrips" section, **When** I click on the "Enlisted Citytrips" toggle/tab, **Then** the page switches to show only my enlisted citytrips.
2. **Given** I am viewing the "Enlisted Citytrips" section, **When** I click on the "My Citytrips" toggle/tab, **Then** the page switches to show only my created citytrips.
3. **Given** I switch between sections, **When** the new section loads, **Then** the active section indicator clearly shows which section I am viewing.

---

### Edge Cases

- What happens when a user tries to delete a citytrip that has enlisted participants? The system should still allow deletion but warn the creator that participants will be affected.
- What happens when a user is viewing their enlisted trips and the trip creator deletes the trip? The trip should disappear from the enlisted list on next load, with no error.
- What happens if the user has a very large number of citytrips? The list should remain usable through pagination or scrolling.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a dedicated "My Citytrips" page accessible to authenticated users.
- **FR-002**: The page MUST be divided into two mutually exclusive sections: "My Citytrips" (created by the user) and "Enlisted Citytrips" (trips the user has joined).
- **FR-003**: Users MUST be able to toggle between the two sections, with only one section visible at a time.
- **FR-004**: The "My Citytrips" section MUST be the default view when the page loads.
- **FR-005**: Users MUST be able to create a new citytrip by providing: title (required), destination (required), start date (required), end date (required), description (optional), and max participants (optional).
- **FR-006**: Users MUST be able to edit any citytrip they have created via a modal dialog pre-filled with current values.
- **FR-007**: Users MUST be able to delete any citytrip they have created, with a confirmation step before deletion.
- **FR-008**: The "Enlisted Citytrips" section MUST display citytrips the user has joined, in a read-only format (no edit or delete actions).
- **FR-009**: System MUST display appropriate empty state messages when either section contains no citytrips.
- **FR-010**: System MUST validate required fields when creating or editing a citytrip and display clear error messages for invalid input.
- **FR-011**: Only the creator of a citytrip MUST be able to edit or delete it.

### Key Entities

- **Citytrip**: A trip created by a user, containing: title, destination, start date, end date, description, and max participants. Has one creator and can have multiple enlisted participants.
- **User**: An authenticated person who can create citytrips and enlist in citytrips created by others.
- **Enlistment**: The relationship between a user and a citytrip they have joined (but did not create).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can create a new citytrip in under 1 minute.
- **SC-002**: Users can switch between "My Citytrips" and "Enlisted Citytrips" sections in under 1 second.
- **SC-003**: Users can find and edit any of their citytrips within 3 clicks from the "My Citytrips" page.
- **SC-004**: 95% of users successfully create, edit, or delete a citytrip on their first attempt without errors.
- **SC-005**: The page correctly separates created and enlisted citytrips with zero data overlap between sections.

## Assumptions

- Users are already authenticated; this feature does not handle login/registration.
- The citytrip entity already exists in the system from feature 001 (browse citytrips).
- Enlistment functionality (joining a trip) exists or will exist separately; this feature only displays enlisted trips, it does not handle the enlistment action itself.
- Standard web performance expectations apply (page loads under 2 seconds, interactions respond within 1 second).
