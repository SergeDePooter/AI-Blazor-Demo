# Quickstart: Create Citytrip Wizard — Integration Scenarios

**Feature**: 007-create-citytrip-map | **Date**: 2026-02-24

This document describes the end-to-end scenarios for manually validating the feature after implementation. These scenarios map directly to the spec's user stories and acceptance criteria.

---

## Prerequisites

- App running locally on `https://localhost:5190`
- Google Maps API key configured (or not — fallback scenarios included)
- Logged in as any user

---

## Scenario 1: Complete wizard with all basic fields (US1 — P1)

**Goal**: Confirm all trip-level fields are saved and appear on the detail page.

1. Navigate to `/my-citytrips`.
2. Click **+ Create Citytrip**.
3. Verify you are navigated to `/citytrips/create` (not a modal).
4. Fill in Step 1:
   - Title: `Tokyo Adventure`
   - Destination: `Tokyo, Japan`
   - Start Date: `2026-09-01`
   - End Date: `2026-09-03`
   - Description: `A three-day exploration of Tokyo`
   - Max Participants: `4`
   - Image URL: `https://upload.wikimedia.org/wikipedia/commons/thumb/b/b2/Skyscrapers_of_Shinjuku_2009_January.jpg/1200px-Skyscrapers_of_Shinjuku_2009_January.jpg`
5. Click **Next**.
6. Verify wizard advances to Step 2 with 3 day slots (Sep 1, Sep 2, Sep 3).
7. Click **Next** without adding events.
8. Verify Step 3 shows a read-only summary with all Step 1 values.
9. Click **Confirm**.
10. Verify you are redirected to `/my-citytrips` and the new trip card shows the title, destination, and image.
11. Click the trip to open its detail page — verify all 5 fields display correctly.

---

## Scenario 2: Validation on Step 1

**Goal**: Confirm required field validation prevents advancing.

1. Open `/citytrips/create`.
2. Leave Title empty, fill all other fields. Click **Next**.
3. Verify error: "Title is required." Wizard stays on Step 1.
4. Fill Title. Set End Date before Start Date. Click **Next**.
5. Verify error: "End date must be on or after start date." Wizard stays on Step 1.
6. Fix dates. Click **Next** — wizard advances.

---

## Scenario 3: Add events in Step 2 and confirm (US2 — P2)

**Goal**: Confirm day plan events appear on the detail page in the correct order.

1. Complete Step 1 with a 2-day trip (Start: `2026-09-01`, End: `2026-09-02`).
2. Advance to Step 2. Verify 2 day slots appear (Day 1: Sep 1, Day 2: Sep 2).
3. In Day 1, click **+ Add Event**. Fill:
   - Event Type: `museum`
   - Name: `Shibuya Crossing Walk`
   - Start Time: `10:00`
   - End Time: `11:00`
4. Add a second event to Day 1:
   - Event Type: `restaurant`
   - Name: `Ramen Lunch`
   - Start Time: `12:30`
5. Verify events appear in start-time order (10:00 before 12:30) within Day 1.
6. In Day 2, add one event: `landmark` / `Senso-ji Temple` / `09:00`.
7. Click **Next** → Step 3 shows all events grouped by day.
8. Click **Confirm** → navigate to detail page.
9. Verify Day 1 shows both events in order; Day 2 shows the temple event.

---

## Scenario 4: Event end-time validation

**Goal**: Confirm end time before start time is rejected.

1. In Step 2, add an event with Start Time `14:00` and End Time `13:00`.
2. Click **Next** or **Save Event**.
3. Verify error: "End time must be after start time." Event row remains editable.

---

## Scenario 5: Back navigation preserves data

**Goal**: Confirm wizard data is not lost when navigating back.

1. Complete Step 1 and advance to Step 2.
2. Add one event to Day 1.
3. Click **Back** — verify you return to Step 1 with all previously entered values intact.
4. Change the description. Click **Next** — verify Step 2 still shows the event added earlier.
5. Click **Next** and **Confirm** — verify both the updated description and the event appear on the detail page.

---

## Scenario 6: Date range change prompts confirmation

**Goal**: Confirm changing dates in Step 1 after events are entered prompts the user.

1. Complete Step 1 (2-day trip), advance to Step 2, add one event.
2. Click **Back** to Step 1. Change End Date to add a third day.
3. Verify a confirmation prompt appears: "Changing the dates will clear your event schedule. Continue?"
4. Click **Cancel** — verify Step 1 reverts to original dates; Step 2 retains events.
5. Repeat step 2, this time click **Continue** — verify Step 2 regenerates with 3 day slots and no events.

---

## Scenario 7: Map picker — place a pin (US3 — P3, requires API key)

**Goal**: Confirm location picker records coordinates and they appear on the detail map.

*Requires: valid Google Maps API key in `appsettings.Development.json`.*

1. In Step 2, add an event. Click **Pick on map**.
2. Verify a modal overlay opens with a Google Map centred on Paris (default).
3. Click on a location (e.g., near "Eiffel Tower" area).
4. Verify a pin is placed and the Place Name field auto-populates (reverse geocoding) or remains empty for manual entry.
5. Type `Eiffel Tower` in the name field if not auto-filled.
6. Click **Confirm Location**.
7. Verify the event row shows the place name badge.
8. Complete the wizard. On the detail page, verify the map shows a marker with label "Eiffel Tower" at the correct location.

---

## Scenario 8: Map picker — API unavailable fallback (US3 — P3)

**Goal**: Confirm the picker falls back gracefully when Maps is unavailable.

*Requires: no Google Maps API key (or an invalid key).*

1. In Step 2, add an event. Click **Pick on map**.
2. Verify the modal opens and shows the fallback message: "Map unavailable. Enter a place name manually."
3. Type a place name manually and enter coordinates (or leave coordinates empty).
4. Click **Confirm Location** — verify the event row shows the typed place name.
5. Complete the wizard. On the detail page, verify no map marker appears (or the detail map shows its own fallback).

---

## Scenario 9: Re-opening picker with existing pin

**Goal**: Confirm existing pin is shown when re-opening the picker.

1. In Step 2, add an event and pick a location (pin placed).
2. Click **Pick on map** again on the same event.
3. Verify the modal opens with the pin already at the previously picked coordinates.
4. Click outside or **Cancel** — verify the original coordinates are unchanged.
