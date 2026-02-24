# Data Model: Citytrip Detail Page with Day Schedule and Map

**Feature**: 006-trip-day-schedule
**Date**: 2026-02-24
**Purpose**: Define new domain entities, DTO extensions, and seed data for the generic day schedule and map integration.

---

## Domain Entities

Domain entities live in `CitytripPlanner.Features/Citytrips/Domain/`. All new entities follow the existing pattern of C# records with constructor validation.

---

### New: ScheduledEvent

**Location**: `CitytripPlanner.Features/Citytrips/Domain/ScheduledEvent.cs` (new)

```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a generic timed event in a day schedule.
/// Supports any event type via a free-form label (e.g. "Museum", "Market", "Stadium").
/// </summary>
public record ScheduledEvent
{
    public string EventType { get; init; }
    public string Name { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly? EndTime { get; init; }
    public string? Description { get; init; }
    public string? Notes { get; init; }
    public Place? Place { get; init; }

    public ScheduledEvent(
        string eventType,
        string name,
        TimeOnly startTime,
        TimeOnly? endTime = null,
        string? description = null,
        string? notes = null,
        Place? place = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType is required.", nameof(eventType));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (endTime.HasValue && endTime.Value <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.", nameof(endTime));

        EventType = eventType;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Notes = notes;
        Place = place;
    }
}
```

**Properties**:
- `EventType` (string, required): Free-form label (e.g., "Museum", "Market", "Stadium", "Restaurant")
- `Name` (string, required): Display name of the event
- `StartTime` (TimeOnly, required): When the event begins
- `EndTime` (TimeOnly?, optional): When the event ends; must be after StartTime if provided
- `Description` (string?, optional): Short description of the event
- `Notes` (string?, optional): Free-text notes for the traveller
- `Place` (Place?, optional): Geographic location; null means the event has no map marker

**Validation Rules** (spec FR-004, FR-005, FR-006, FR-009):
- `EventType` must not be null or whitespace
- `Name` must not be null or whitespace
- `EndTime`, if provided, must be strictly after `StartTime`

**Relationships**:
- Owned by `DayPlan` (composition)
- Optionally owns a `Place` (composition)

---

### New: Place

**Location**: `CitytripPlanner.Features/Citytrips/Domain/Place.cs` (new)

```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// A geographic point of interest associated with a ScheduledEvent.
/// Provides the coordinates required to place a marker on the map.
/// </summary>
public record Place
{
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }

    public Place(string name, double latitude, double longitude, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Place Name is required.", nameof(name));
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }
}
```

**Properties**:
- `Name` (string, required): Human-readable place name shown in map marker popup
- `Latitude` (double, required): Valid WGS-84 latitude (-90 to 90)
- `Longitude` (double, required): Valid WGS-84 longitude (-180 to 180)
- `Address` (string?, optional): Street address or free-text location description

**Validation Rules**:
- `Name` must not be null or whitespace
- `Latitude` in range [-90, 90]
- `Longitude` in range [-180, 180]

**Relationships**:
- Owned by `ScheduledEvent` (composition, 0..1)

---

### Extended: DayPlan

**Location**: `CitytripPlanner.Features/Citytrips/Domain/DayPlan.cs` (existing — modified)

**Add** optional `Events` property:

```csharp
public record DayPlan
{
    public int DayNumber { get; init; }
    public DateOnly Date { get; init; }
    public string Timeframe { get; init; }
    public List<Attraction> Attractions { get; init; }
    public List<ScheduledEvent> Events { get; init; }   // NEW

    public DayPlan(
        int dayNumber,
        DateOnly date,
        string timeframe,
        List<Attraction> attractions,
        List<ScheduledEvent>? events = null)            // NEW (optional, defaults to empty list)
    {
        if (dayNumber <= 0)
            throw new ArgumentException("DayNumber must be positive.", nameof(dayNumber));
        ArgumentNullException.ThrowIfNull(attractions);

        DayNumber = dayNumber;
        Date = date;
        Timeframe = timeframe;
        Attractions = attractions;
        Events = events ?? new List<ScheduledEvent>();  // NEW
    }
}
```

**Change summary**:
- Added `List<ScheduledEvent> Events` property (non-nullable in object; constructor parameter is optional to preserve backward compatibility)
- Existing `Attractions` property and all validation unchanged

---

## Response DTOs

DTOs live in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/`. The existing `CitytripDetailResponse.cs` is extended with two new nested records.

---

### Extended: CitytripDetailResponse / DayPlanDetail

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs` (existing — modified)

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

public record CitytripDetailResponse(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description,
    int? MaxParticipants,
    List<DayPlanDetail>? DayPlans);

public record DayPlanDetail(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<AttractionDetail> Attractions,
    List<ScheduledEventDetail> Events);        // NEW

public record AttractionDetail(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);

// NEW:
public record ScheduledEventDetail(
    string EventType,
    string Name,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    string? Description,
    string? Notes,
    PlaceDetail? Place);

// NEW:
public record PlaceDetail(
    string Name,
    double Latitude,
    double Longitude,
    string? Address);
```

---

### Handler Projection Extension

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs` (existing — modified)

Extend the existing `DayPlan → DayPlanDetail` mapping:

```csharp
// Within the existing Select for DayPlans:
new DayPlanDetail(
    DayNumber: dp.DayNumber,
    Date:      dp.Date,
    Timeframe: dp.Timeframe,
    Attractions: dp.Attractions.Select(a => new AttractionDetail(
        a.Name, a.Description, a.WebsiteUrl, a.TransportationOptions
    )).ToList(),
    Events: dp.Events.Select(e => new ScheduledEventDetail(   // NEW
        EventType:   e.EventType,
        Name:        e.Name,
        StartTime:   e.StartTime,
        EndTime:     e.EndTime,
        Description: e.Description,
        Notes:       e.Notes,
        Place:       e.Place is null ? null : new PlaceDetail(
                         e.Place.Name,
                         e.Place.Latitude,
                         e.Place.Longitude,
                         e.Place.Address)
    )).ToList()
)
```

---

## Data Relationships Diagram

```
Citytrip (aggregate root)
│
├─ Id, Title, Destination, ImageUrl, StartDate, EndDate, CreatorId, Description?, MaxParticipants?
│
└─ DayPlans: List<DayPlan>? (0..*)
    │
    ├─ DayNumber, Date, Timeframe
    │
    ├─ Attractions: List<Attraction> (0..*) [existing]
    │   └─ Name, Description, WebsiteUrl?, TransportationOptions: List<string>
    │
    └─ Events: List<ScheduledEvent> (0..*) [NEW]
        │
        ├─ EventType (free-form label)
        ├─ Name
        ├─ StartTime (TimeOnly)
        ├─ EndTime? (TimeOnly)
        ├─ Description?
        ├─ Notes?
        │
        └─ Place? (0..1) [NEW]
            ├─ Name
            ├─ Latitude (double)
            ├─ Longitude (double)
            └─ Address?
```

---

## Seed Data Extension

**Location**: `CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs` (existing — modified)

Extend the Paris trip (ID 1) Day 1 with `Events` to demonstrate the schedule. The existing `Attractions` on Day 1 remain unchanged.

```csharp
// Paris trip, Day 1 — add Events alongside existing Attractions:
new DayPlan(
    dayNumber: 1,
    date: new DateOnly(2026, 6, 1),
    timeframe: "Morning 9:00-12:00",
    attractions: new List<Attraction> { /* existing Eiffel Tower, Trocadéro */ },
    events: new List<ScheduledEvent>
    {
        new ScheduledEvent(
            eventType: "Landmark",
            name: "Eiffel Tower Visit",
            startTime: new TimeOnly(9, 0),
            endTime: new TimeOnly(11, 0),
            description: "Ascend the iconic iron lattice tower for panoramic city views.",
            notes: "Book skip-the-line tickets in advance.",
            place: new Place("Eiffel Tower", 48.8584, 2.2945, "Champ de Mars, 5 Av. Anatole France, 75007 Paris")),

        new ScheduledEvent(
            eventType: "Market",
            name: "Marché Saxe-Breteuil",
            startTime: new TimeOnly(11, 15),
            endTime: new TimeOnly(12, 30),
            description: "Traditional Parisian open-air market with local produce and flowers.",
            place: new Place("Marché Saxe-Breteuil", 48.8501, 2.3072, "Av. de Saxe, 75007 Paris")),
    }
),

// Paris trip, Day 2 — new Events for museum and cathedral:
new DayPlan(
    dayNumber: 2,
    date: new DateOnly(2026, 6, 2),
    timeframe: "Afternoon 14:00-18:00",
    attractions: new List<Attraction> { /* existing Louvre */ },
    events: new List<ScheduledEvent>
    {
        new ScheduledEvent(
            eventType: "Museum",
            name: "Louvre Museum",
            startTime: new TimeOnly(14, 0),
            endTime: new TimeOnly(17, 30),
            description: "World's largest art museum, home to the Mona Lisa and Venus de Milo.",
            notes: "Closed on Tuesdays.",
            place: new Place("Louvre Museum", 48.8606, 2.3376, "Rue de Rivoli, 75001 Paris")),

        new ScheduledEvent(
            eventType: "Cathedral",
            name: "Sainte-Chapelle",
            startTime: new TimeOnly(17, 45),
            endTime: new TimeOnly(18, 30),
            description: "Gothic royal chapel famous for its stunning stained glass windows.",
            place: new Place("Sainte-Chapelle", 48.8554, 2.3450, "8 Bd du Palais, 75001 Paris")),
    }
)
```

---

## Validation Summary

| Entity | Rule | Spec Reference |
|--------|------|----------------|
| **ScheduledEvent** | EventType not null/whitespace | FR-004, FR-006 |
| **ScheduledEvent** | Name not null/whitespace | FR-004 |
| **ScheduledEvent** | EndTime > StartTime if provided | FR-004, FR-005 |
| **Place** | Name not null/whitespace | FR-008 |
| **Place** | Latitude in [-90, 90] | FR-007 |
| **Place** | Longitude in [-180, 180] | FR-007 |
| **DayPlan** | Existing validations unchanged | — |

---

## Next Steps

1. Implement `ScheduledEvent.cs` and `Place.cs` in `Features/Citytrips/Domain/`
2. Extend `DayPlan.cs` with the `Events` parameter
3. Extend `CitytripDetailResponse.cs` with new DTO records
4. Extend `GetCitytripDetailHandler.cs` projection
5. Extend seed data in `InMemoryCitytripRepository.cs`
6. Build Blazor components and JS interop file

See `contracts/` and `quickstart.md` for further guidance.
