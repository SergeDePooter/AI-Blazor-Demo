# Data Model: Create Citytrip with Full Fields and Map-Based Location Picker

**Branch**: `007-create-citytrip-map` | **Date**: 2026-02-24

---

## Existing Domain Entities (unchanged)

The following domain entities already exist in `CitytripPlanner.Features/Citytrips/Domain/` and are **not modified** by this feature.

### `Place` record
```
Name        : string  (required, non-empty)
Latitude    : double  (-90..90)
Longitude   : double  (-180..180)
Address     : string? (optional)
```

### `ScheduledEvent` record
```
EventType   : string   (required, non-empty)
Name        : string   (required, non-empty)
StartTime   : TimeOnly (required)
EndTime     : TimeOnly? (optional; if set, must be > StartTime)
Description : string?  (optional)
Place       : Place?   (optional)
```

### `DayPlan` record
```
DayNumber   : int
Date        : DateOnly
Timeframe   : string
Attractions : List<Attraction>
Events      : List<ScheduledEvent>
```

### `Citytrip` record
```
Id              : int
Title           : string
Destination     : string
ImageUrl        : string
StartDate       : DateOnly
EndDate         : DateOnly
CreatorId       : string
Description     : string?
MaxParticipants : int?
DayPlans        : List<DayPlan>?
```

---

## New Command-Side Input Records (feature 007)

These records live in `CitytripPlanner.Features/Citytrips/CreateTrip/` alongside the command. They are **not** domain entities — they carry raw user input before validation and domain object construction.

### `PlaceInput` record
```
Name      : string   (required; comes from reverse geocoding or manual entry)
Latitude  : double   (set when user places pin on map)
Longitude : double   (set when user places pin on map)
```

**Validation** (in `CreateTripValidator`):
- Name must not be null or whitespace
- Latitude must be in range [-90, 90]
- Longitude must be in range [-180, 180]

### `ScheduledEventInput` record
```
EventType   : string    (required)
Name        : string    (required)
StartTime   : TimeOnly  (required)
EndTime     : TimeOnly? (optional)
Description : string?   (optional)
Place       : PlaceInput? (optional)
```

**Validation** (in `CreateTripValidator`):
- EventType must not be null or whitespace
- Name must not be null or whitespace
- EndTime, when provided, must be strictly after StartTime

### `DayPlanInput` record
```
DayNumber : int
Date      : DateOnly
Events    : List<ScheduledEventInput>
```

**Notes**:
- `Timeframe` and `Attractions` are not set during creation — the handler supplies an empty `Attractions` list and a default `Timeframe` of `""`.
- Events list may be empty (user saved a day with no events).

---

## Extended Command (feature 007)

### `CreateTripCommand` record (extended)

| Field | Type | Required | Change |
|---|---|---|---|
| Title | string | ✅ | Existing |
| Destination | string | ✅ | Existing |
| StartDate | DateOnly | ✅ | Existing |
| EndDate | DateOnly | ✅ | Existing |
| CreatorId | string | ✅ | Existing |
| Description | string? | — | Existing |
| MaxParticipants | int? | — | Existing |
| ImageUrl | string? | — | **New** — optional image URL |
| DayPlans | List\<DayPlanInput\>? | — | **New** — optional; empty means no events entered |

**Validation additions** (in `CreateTripValidator`):
- ImageUrl, when provided, must be a valid absolute URL
- Each `DayPlanInput.Date` must fall between `StartDate` and `EndDate` (inclusive)
- Each `ScheduledEventInput` within a `DayPlanInput` must pass its own validation rules

---

## Handler Construction

The `CreateTripHandler` maps command inputs to domain objects:

```
CreateTripCommand
  → Citytrip(
      Id: 0,
      Title, Destination, ImageUrl ?? "",
      StartDate, EndDate, CreatorId,
      Description, MaxParticipants,
      DayPlans: command.DayPlans?.Select(dp =>
        new DayPlan(
          dp.DayNumber, dp.Date,
          timeframe: "",
          attractions: [],
          events: dp.Events.Select(e =>
            new ScheduledEvent(
              e.EventType, e.Name, e.StartTime, e.EndTime, e.Description,
              place: e.Place is null ? null : new Place(e.Place.Name, e.Place.Latitude, e.Place.Longitude)
            )).ToList()
        )).ToList()
    )
```

---

## Entity Relationship Summary

```
CreateTripCommand
  ├── DayPlanInput[]
  │     └── ScheduledEventInput[]
  │           └── PlaceInput?
  │
  ↓ (handler maps to)
  │
Citytrip (domain)
  └── DayPlan[]
        └── ScheduledEvent[]
              └── Place?
```

No new persistence layer changes are required — `InMemoryCitytripRepository.AddAsync` stores the full `Citytrip` object graph as-is.
