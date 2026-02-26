# Quickstart: Edit Citytrip via Wizard

Manual test walkthrough for the key user scenarios. Run these after implementing
each user story to verify end-to-end behaviour.

---

## Prerequisites

- App running locally (`dotnet run` in `CitytripPlanner.Web`)
- At least one trip owned by the current in-memory user ("user-1" or as seeded)
  — the Paris trip (Id=1) created by "seed-user" will NOT pass the ownership
  check; create a test trip via the Create wizard first if needed.
- Browser console open to catch JS errors.

---

## Scenario A — Edit Basics (User Story 1)

**Goal**: Verify Step 1 is pre-filled and changes are saved.

1. Navigate to `/my-citytrips`.
2. Click **Edit** on one of your own trips.
   - **Expected**: Redirected to `/citytrips/edit/{id}`.
   - **Expected**: Wizard opens at Step 1 with Title, Destination, Start Date, End Date,
     Description, Max Participants, and Image URL all populated with current values.
3. Change the Title to "Updated Title".
4. Click **Next**.
   - **Expected**: Step 2 shows. No confirmation dialog (dates unchanged).
5. Click **Next** on Step 2.
   - **Expected**: Step 3 shows the updated title in the summary.
6. Click **Save Changes**.
   - **Expected**: Redirected to `/my-citytrips`.
   - **Expected**: Trip card shows "Updated Title".
7. Click the trip to open the detail page.
   - **Expected**: Detail page shows "Updated Title".

---

## Scenario B — Edit Day Plans (User Story 2)

**Goal**: Verify existing events are pre-loaded and changes are saved.

1. Edit a trip that already has scheduled events (create one via the wizard if
   needed — add at least 2 events on Day 1).
2. Click **Edit** on that trip → wizard opens at Step 1.
3. Click **Next** without changing anything.
   - **Expected**: Step 2 shows each day's existing events pre-filled with their
     type, name, start time, end time, description, and place badge (if any).
4. Change the name of the first event on Day 1.
5. Click **Next**.
   - **Expected**: Step 3 review shows the updated event name.
6. Click **Save Changes**.
   - **Expected**: Trip detail page shows the updated event name.
7. Edit the trip again → Step 2.
   - **Expected**: The previously updated event name is still there.

---

## Scenario C — Date Range Change Warning and Day Preservation (User Story 3)

**Goal**: Verify the warning dialog and day-number event preservation.

**Setup**: Edit a trip that has events on at least Day 1 and Day 2.

### C1 — Extend trip (add a day at the end)

1. Open the edit wizard → Step 1.
2. Change End Date to one day later than current value.
3. Click **Next**.
   - **Expected**: A confirmation dialog appears:
     *"Changing the dates will adjust the schedule. Continue?"*
4. Click **OK** (confirm).
   - **Expected**: Step 2 shows one extra empty day slot at the end.
   - **Expected**: Events on Day 1 and Day 2 are still present on Day 1 and Day 2.

### C2 — Cancel the date change

1. Open the edit wizard → Step 1.
2. Change End Date.
3. Click **Next**.
   - **Expected**: Confirmation dialog appears.
4. Click **Cancel**.
   - **Expected**: Still on Step 1. The date field reverts to original value.
   - **Expected**: No step transition.

### C3 — Shorten trip (remove last day)

1. Edit a trip with events on Days 1, 2, and 3.
2. Change End Date to remove Day 3.
3. Click **Next** and confirm the warning.
   - **Expected**: Step 2 shows only 2 day slots.
   - **Expected**: Day 1 events are on Day 1, Day 2 events are on Day 2.
   - **Expected**: Day 3 events are gone.

---

## Scenario D — Ownership Guard

**Goal**: Verify non-owners cannot access the edit wizard.

1. Directly navigate to `/citytrips/edit/1` (Paris trip, owned by "seed-user").
   - **Expected**: Page shows an unauthorised / access denied message.
   - **Expected**: No wizard form is rendered.

---

## Scenario E — Non-existent Trip

1. Directly navigate to `/citytrips/edit/9999`.
   - **Expected**: Page shows "Trip not found." message.
   - **Expected**: No wizard form is rendered.

---

## Scenario F — Back Navigation Preserves Edits

1. Edit a trip → Step 1. Change the title.
2. Click **Next** → Step 2.
3. Click **Back** → Step 1.
   - **Expected**: Changed title is still in the Title field.
