# Data Model: Edit Citytrip via Wizard

## Overview

No new domain entities are introduced. This feature extends the existing
`UpdateTripCommand` slice with two additional fields (`ImageUrl`, `DayPlans`),
adds a web-layer orchestrator page (`EditCitytrip.razor`), and adds an
`InitialModel` parameter to the existing `WizardStep1` component.

---

## Existing Domain Entities (unchanged)

### Citytrip *(record, in-memory)*

| Field            | Type                | Required | Notes                              |
|------------------|---------------------|----------|------------------------------------|
| Id               | int                 | Yes      | Auto-assigned by repository        |
| Title            | string              | Yes      | Max 100 chars                      |
| Destination      | string              | Yes      | Max 100 chars                      |
| ImageUrl         | string              | Yes      | Empty string if not set            |
| StartDate        | DateOnly            | Yes      |                                    |
| EndDate          | DateOnly            | Yes      | Must be ≥ StartDate                |
| CreatorId        | string              | Yes      | Ownership identity                 |
| Description      | string?             | No       |                                    |
| MaxParticipants  | int?                | No       | Must be ≥ 1 if set                 |
| DayPlans         | List\<DayPlan\>?    | No       | Ordered by DayNumber               |

### DayPlan *(domain entity)*

| Field       | Type                       | Notes                    |
|-------------|----------------------------|--------------------------|
| DayNumber   | int                        | 1-based sequential       |
| Date        | DateOnly                   | Calendar date of the day |
| Timeframe   | string                     | Human description        |
| Attractions | List\<Attraction\>         | Reference info only      |
| Events      | List\<ScheduledEvent\>     | Ordered by StartTime     |

### ScheduledEvent *(domain entity)*

| Field      | Type      | Required | Notes                          |
|------------|-----------|----------|--------------------------------|
| EventType  | string    | Yes      | e.g., "landmark", "museum"     |
| Name       | string    | Yes      |                                |
| StartTime  | TimeOnly  | Yes      |                                |
| EndTime    | TimeOnly? | No       | Must be after StartTime if set |
| Description| string?   | No       |                                |
| Place      | Place?    | No       |                                |

### Place *(value object)*

| Field     | Type    | Required |
|-----------|---------|----------|
| Name      | string  | Yes      |
| Latitude  | double  | Yes      |
| Longitude | double  | Yes      |
| Address   | string? | No       |

---

## Modified: UpdateTripCommand

The existing `UpdateTripCommand` record gains two optional fields:

| Field           | Type                   | New? | Notes                                       |
|-----------------|------------------------|------|---------------------------------------------|
| TripId          | int                    |      | Identifies the trip to update               |
| Title           | string                 |      |                                             |
| Destination     | string                 |      |                                             |
| StartDate       | DateOnly               |      |                                             |
| EndDate         | DateOnly               |      |                                             |
| UserId          | string                 |      | Must match `CreatorId` for auth check       |
| Description     | string?                |      |                                             |
| MaxParticipants | int?                   |      |                                             |
| **ImageUrl**    | **string?**            | ✅   | Replaces trip's ImageUrl when non-null      |
| **DayPlans**    | **List\<DayPlanInput\>?** | ✅ | Replaces all day plans when non-null        |

`DayPlanInput`, `ScheduledEventInput`, and `PlaceInput` records already exist
in `CitytripPlanner.Features/Citytrips/CreateTrip/` and are reused directly —
no new types needed.

### Handler semantics change

When `DayPlans` is non-null the handler fully replaces the trip's day plans
list, mapping each `DayPlanInput` → `DayPlan` domain object (same mapping as
`CreateTripHandler`). When `DayPlans` is null the existing day plans are
preserved unchanged.

When `ImageUrl` is non-null it replaces the trip's image URL; when null the
existing ImageUrl is preserved.

---

## Modified: UpdateTripValidator

Two new validation rules added:

| Rule                                             | Error message                              |
|--------------------------------------------------|--------------------------------------------|
| If `ImageUrl` non-null & non-empty: must be absolute URI | "Image URL must be a valid URL."  |
| If any event has `EndTime < StartTime`           | "End time must be after start time."       |

---

## Modified: WizardStep1.razor

One new component parameter:

| Parameter    | Type                  | Required | Notes                                              |
|--------------|-----------------------|----------|----------------------------------------------------|
| **InitialModel** | **WizardStep1Model?** | No  | When set, field values are copied into `_model` in `OnInitialized`; backward-compatible (create flow unaffected) |

Pre-fill logic (in `OnInitialized`):
```text
if InitialModel is not null:
    _model.Title            = InitialModel.Title
    _model.Destination      = InitialModel.Destination
    _model.StartDate        = InitialModel.StartDate
    _model.EndDate          = InitialModel.EndDate
    _model.Description      = InitialModel.Description
    _model.MaxParticipants  = InitialModel.MaxParticipants
    _model.ImageUrl         = InitialModel.ImageUrl
```

---

## New: EditCitytrip.razor (web-layer page, no domain entity)

Web-layer state managed by this page — same model types as `CreateCitytrip.razor`:

| Field         | Type                      | Source                          |
|---------------|---------------------------|---------------------------------|
| `_tripId`     | int                       | Route parameter `{Id:int}`      |
| `_step`       | int (1–3)                 | Step tracking                   |
| `_step1Model` | WizardStep1Model?         | Pre-filled from GetCitytripDetail |
| `_dayPlans`   | List\<DayPlanInputModel\> | Pre-filled from GetCitytripDetail |
| `_errors`     | List\<string\>            | Validation / handler errors     |
| `_isSaving`   | bool                      | Guard against double-submit     |
| `_notFound`   | bool                      | Trip not found flag             |
| `_unauthorized` | bool                    | Ownership check flag            |

### Pre-fill mapping (GetCitytripDetail → page state)

```text
CitytripDetailResponse → WizardStep1Model:
  response.Title           → _step1Model.Title
  response.Destination     → _step1Model.Destination
  response.StartDate       → _step1Model.StartDate
  response.EndDate         → _step1Model.EndDate
  response.Description     → _step1Model.Description
  response.MaxParticipants → _step1Model.MaxParticipants
  response.ImageUrl        → _step1Model.ImageUrl

CitytripDetailResponse.DayPlans → List<DayPlanInputModel>:
  For each DayPlanDetail dp:
    DayPlanInputModel.DayNumber = dp.DayNumber
    DayPlanInputModel.Date      = dp.Date
    DayPlanInputModel.Events    = dp.Events.Select(e =>
      ScheduledEventInputModel:
        EventType   = e.EventType
        Name        = e.Name
        StartTime   = e.StartTime
        EndTime     = e.EndTime
        Description = e.Description
        Place       = e.Place is null ? null : PlaceInputModel:
          Name      = e.Place.Name
          Latitude  = e.Place.Latitude
          Longitude = e.Place.Longitude
```

### Date-change day-number preservation logic

```text
When owner changes start/end date and existing events are present:
1. Show JS confirm() — "Changing dates will adjust the schedule. Continue?"
2. If confirmed:
   a. oldPlans = _dayPlans (current in-memory list)
   b. newPlans = GenerateDayPlans(newStart, newEnd)  // empty slots
   c. For each slot in newPlans:
        if slot.DayNumber <= oldPlans.Count:
          slot.Events = oldPlans[slot.DayNumber - 1].Events
        else:
          slot.Events = []  (empty — new day beyond old length)
   d. _dayPlans = newPlans
3. If cancelled: restore previous StartDate/EndDate, do not advance step
```
