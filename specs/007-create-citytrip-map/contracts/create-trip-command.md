# Contract: Extended CreateTripCommand

**Feature**: 007-create-citytrip-map
**Type**: MediatR Command (Blazor Server — no HTTP endpoint)
**Slice**: `CitytripPlanner.Features/Citytrips/CreateTrip/`

---

## Command Definition

```csharp
public record CreateTripCommand(
    string Title,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatorId,
    string? Description = null,
    int? MaxParticipants = null,
    string? ImageUrl = null,           // NEW
    List<DayPlanInput>? DayPlans = null // NEW
) : IRequest<int>;
```

---

## Input Records

```csharp
public record DayPlanInput(
    int DayNumber,
    DateOnly Date,
    List<ScheduledEventInput> Events);

public record ScheduledEventInput(
    string EventType,
    string Name,
    TimeOnly StartTime,
    TimeOnly? EndTime = null,
    string? Description = null,
    PlaceInput? Place = null);

public record PlaceInput(
    string Name,
    double Latitude,
    double Longitude);
```

---

## Validation Rules

All rules enforced by `CreateTripValidator.Validate(CreateTripCommand)` before domain object construction.

### Trip-level
| Field | Rule | Error message |
|---|---|---|
| Title | Required, ≤100 chars | "Title is required." / "Title must be 100 characters or less." |
| Destination | Required, ≤100 chars | "Destination is required." / "Destination must be 100 characters or less." |
| EndDate | ≥ StartDate | "End date must be on or after start date." |
| MaxParticipants | If provided, ≥1 | "Max participants must be at least 1." |
| ImageUrl | If provided, must be valid absolute URL | "Image URL must be a valid URL." |

### DayPlan-level (per DayPlanInput)
| Field | Rule | Error message |
|---|---|---|
| Date | Must be ≥ StartDate and ≤ EndDate | "Day {N} date is outside the trip date range." |

### ScheduledEvent-level (per ScheduledEventInput)
| Field | Rule | Error message |
|---|---|---|
| EventType | Required, non-empty | "Event type is required for day {N}, event {M}." |
| Name | Required, non-empty | "Event name is required for day {N}, event {M}." |
| EndTime | If provided, must be > StartTime | "End time must be after start time for day {N}, event {M}." |

### Place-level (per PlaceInput, when present)
| Field | Rule | Error message |
|---|---|---|
| Name | Required, non-empty | "Place name is required for day {N}, event {M}." |
| Latitude | -90 ≤ value ≤ 90 | "Latitude is out of range for day {N}, event {M}." |
| Longitude | -180 ≤ value ≤ 180 | "Longitude is out of range for day {N}, event {M}." |

---

## Response

Returns `int` — the newly assigned trip ID (auto-increment from `InMemoryCitytripRepository`).

---

## Handler Behaviour

1. Run `CreateTripValidator.Validate(command)` — throw `ArgumentException` with joined errors if any.
2. Construct domain `Citytrip` with full `DayPlan → ScheduledEvent → Place` object graph.
3. Call `repository.AddAsync(trip)` and return `created.Id`.

---

## Test Scenarios (for `CreateTripHandlerTests`)

| Scenario | Input | Expected |
|---|---|---|
| Basic trip without day plans | title + destination + dates | Returns new ID; trip has null/empty DayPlans |
| Trip with image URL | adds `ImageUrl = "https://example.com/img.jpg"` | Saved trip has correct ImageUrl |
| Trip with one day plan, no events | one DayPlanInput with empty Events | Returns ID; DayPlan saved with 0 events |
| Trip with one day plan, two events in order | events at 9:00 and 14:00 | Events ordered by StartTime in domain |
| Trip with event containing a Place | PlaceInput with valid coords | Place saved with correct lat/lng/name |
| Invalid image URL | `ImageUrl = "not-a-url"` | Throws `ArgumentException` |
| Event end time before start time | EndTime ≤ StartTime | Throws `ArgumentException` |
| Day date outside trip range | DayPlan.Date before StartDate | Throws `ArgumentException` |
