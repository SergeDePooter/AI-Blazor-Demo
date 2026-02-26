# Contract: UpdateTripCommand (Extended)

**Slice**: `CitytripPlanner.Features/Citytrips/UpdateTrip/`
**Handler**: `UpdateTripHandler`

---

## Command Signature (after extension)

```csharp
public record UpdateTripCommand(
    int TripId,
    string Title,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    string UserId,
    string? Description = null,
    int? MaxParticipants = null,
    string? ImageUrl = null,          // NEW — replaces trip image when non-null
    List<DayPlanInput>? DayPlans = null  // NEW — replaces all day plans when non-null
) : IRequest<bool>;
```

`DayPlanInput`, `ScheduledEventInput`, and `PlaceInput` are reused from
`CitytripPlanner.Features/Citytrips/CreateTrip/` — no new types.

---

## Validation Rules

All existing rules remain:

| Rule | Error |
|------|-------|
| TripId > 0 | "Trip ID is required." |
| Title non-empty, ≤ 100 chars | "Title is required." / "Title must be 100 characters or less." |
| Destination non-empty, ≤ 100 chars | "Destination is required." / "Destination must be 100 characters or less." |
| EndDate ≥ StartDate | "End date must be on or after start date." |
| MaxParticipants ≥ 1 if set | "Max participants must be at least 1." |

New rules:

| Rule | Error |
|------|-------|
| ImageUrl must be absolute URI if non-null and non-empty | "Image URL must be a valid URL." |
| Any event EndTime must be after StartTime if both set | "End time must be after start time." |

---

## Handler Behaviour

```text
1. Validate command → throw ArgumentException if any errors
2. Load trip = repository.GetByIdAsync(TripId)
   → throw ArgumentException if not found
3. Check trip.CreatorId == UserId
   → throw UnauthorizedAccessException if not owner
4. Build updated record:
   - Always apply: Title, Destination, StartDate, EndDate, Description, MaxParticipants
   - If ImageUrl != null: apply ImageUrl; else keep existing
   - If DayPlans != null: map DayPlanInput list → List<DayPlan> domain objects, apply
     else: keep existing DayPlans unchanged
5. repository.UpdateAsync(updated)
6. return true
```

### DayPlanInput → DayPlan mapping

```text
For each DayPlanInput dp in command.DayPlans:
  new DayPlan(
    dp.DayNumber,
    dp.Date,
    timeframe: "",          // reset — not user-editable in wizard
    attractions: [],        // reset — not user-editable in wizard
    events: dp.Events
      .OrderBy(e => e.StartTime)
      .Select(e => new ScheduledEvent(
          e.EventType, e.Name, e.StartTime, e.EndTime, e.Description,
          e.Place is null ? null : new Place(e.Place.Name, e.Place.Latitude, e.Place.Longitude, null)
      ))
      .ToList()
  )
```

---

## Test Scenarios

### Handler Tests (`UpdateTripHandlerTests.cs` — extended)

| # | Scenario | Expected |
|---|----------|----------|
| 1 | *(existing)* Valid basics update (no ImageUrl, no DayPlans) | Returns true; Title/Destination updated; DayPlans preserved |
| 2 | *(existing)* TripId not found | throws `ArgumentException` |
| 3 | *(existing)* UserId is not owner | throws `UnauthorizedAccessException` |
| 4 | Update with valid ImageUrl — replaces existing | Returns true; ImageUrl set to new value |
| 5 | Update with DayPlans — replaces existing day plans | Returns true; trip has new DayPlan list with mapped events |
| 6 | Update with DayPlans = null — preserves existing day plans | Returns true; existing DayPlans unchanged |
| 7 | Update with ImageUrl = null — preserves existing image | Returns true; existing ImageUrl unchanged |
| 8 | DayPlan with Place → Place is stored on ScheduledEvent | Returns true; Place coords accessible on retrieved trip |

### Validator Tests (`UpdateTripValidatorTests.cs` — extended)

| # | Scenario | Expected |
|---|----------|----------|
| 1 | *(existing)* All valid inputs | Empty errors |
| 2 | ImageUrl = "not-a-url" | Contains "Image URL must be a valid URL." |
| 3 | ImageUrl = null | Valid (no error) |
| 4 | ImageUrl = "" | Valid (no error — empty string treated as "not provided") |
| 5 | Event with EndTime < StartTime | Contains "End time must be after start time." |
| 6 | Event with EndTime = null | Valid (no error) |
