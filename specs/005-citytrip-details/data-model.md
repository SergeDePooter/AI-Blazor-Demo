# Data Model: Citytrip Detail View

**Feature**: 005-citytrip-details
**Date**: 2026-02-14
**Purpose**: Define domain entities and DTOs for citytrip detail view with itinerary

## Domain Entities

Domain entities live in `CitytripPlanner.Features/Citytrips/Domain/` and represent the core business concepts. All entities use C# records for immutability and value-based equality.

### Extended: Citytrip

**Location**: `CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs` (existing)

**Modification**: Add `DayPlans` collection property

```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

public record Citytrip(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatorId,
    string? Description = null,
    int? MaxParticipants = null,
    List<DayPlan>? DayPlans = null);  // NEW: itinerary data
```

**Rationale**:
- Itinerary is tightly coupled to a citytrip (aggregate root pattern)
- Optional property for backward compatibility with existing features
- List allows multiple days per trip

---

### New: DayPlan

**Location**: `CitytripPlanner.Features/Citytrips/Domain/DayPlan.cs` (new)

**Definition**:
```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a single day within a citytrip itinerary.
/// </summary>
public record DayPlan(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<Attraction> Attractions)
{
    /// <summary>
    /// Validates that DayNumber is positive and Attractions list is not null.
    /// </summary>
    public DayPlan
    {
        if (DayNumber <= 0)
            throw new ArgumentException("DayNumber must be positive", nameof(DayNumber));
        if (Attractions == null)
            throw new ArgumentNullException(nameof(Attractions));
    }
}
```

**Properties**:
- `DayNumber` (int): Sequential day number (1-based) within the trip
- `Date` (DateOnly): Actual calendar date for this day
- `Timeframe` (string): Human-readable timeframe (e.g., "Morning 9:00-12:00", "All day")
- `Attractions` (List<Attraction>): Places to visit on this day

**Validation Rules** (spec FR-003, FR-004):
- DayNumber must be positive
- Attractions list must not be null (can be empty if no attractions)
- Date should fall within Citytrip.StartDate and EndDate range (validated in handler/repository)

**Relationships**:
- Owned by `Citytrip` (composition)
- Owns collection of `Attraction`

---

### New: Attraction

**Location**: `CitytripPlanner.Features/Citytrips/Domain/Attraction.cs` (new)

**Definition**:
```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a place to visit during a citytrip day.
/// </summary>
public record Attraction(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions)
{
    /// <summary>
    /// Validates required fields and URL format if provided.
    /// </summary>
    public Attraction
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required", nameof(Name));
        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description is required", nameof(Description));
        if (WebsiteUrl != null && !Uri.TryCreate(WebsiteUrl, UriKind.Absolute, out _))
            throw new ArgumentException("WebsiteUrl must be a valid absolute URI", nameof(WebsiteUrl));
        if (TransportationOptions == null)
            throw new ArgumentNullException(nameof(TransportationOptions));
    }
}
```

**Properties**:
- `Name` (string): Name of the attraction (e.g., "Eiffel Tower")
- `Description` (string): Brief description of the attraction
- `WebsiteUrl` (string?): Optional URL to attraction website (spec FR-006)
- `TransportationOptions` (List<string>): How to get there (spec FR-007)

**Validation Rules** (spec FR-005, FR-006, FR-007):
- Name and Description are required
- WebsiteUrl must be valid absolute URI if provided
- TransportationOptions list must not be null (can be empty)

**Relationships**:
- Owned by `DayPlan` (composition)

---

## Response DTOs

DTOs live in `CitytripPlanner.Features/Citytrips/GetCitytripDetail/` and shape data for the UI. DTOs flatten the domain model for easier Blazor component binding.

### CitytripDetailResponse

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs` (new)

**Definition**:
```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

/// <summary>
/// Response DTO for citytrip detail view including full itinerary.
/// </summary>
public record CitytripDetailResponse(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description,
    int? MaxParticipants,
    List<DayPlanDto> DayPlans);

/// <summary>
/// DTO representing a single day in the itinerary.
/// </summary>
public record DayPlanDto(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<AttractionDto> Attractions);

/// <summary>
/// DTO representing an attraction to visit.
/// </summary>
public record AttractionDto(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);
```

**Mapping from Domain**:
```csharp
// In GetCitytripDetailHandler.cs
var response = new CitytripDetailResponse(
    Id: citytrip.Id,
    Title: citytrip.Title,
    Destination: citytrip.Destination,
    ImageUrl: citytrip.ImageUrl,
    StartDate: citytrip.StartDate,
    EndDate: citytrip.EndDate,
    Description: citytrip.Description,
    MaxParticipants: citytrip.MaxParticipants,
    DayPlans: citytrip.DayPlans?.Select(dp => new DayPlanDto(
        DayNumber: dp.DayNumber,
        Date: dp.Date,
        Timeframe: dp.Timeframe,
        Attractions: dp.Attractions.Select(a => new AttractionDto(
            Name: a.Name,
            Description: a.Description,
            WebsiteUrl: a.WebsiteUrl,
            TransportationOptions: a.TransportationOptions
        )).ToList()
    )).ToList() ?? new List<DayPlanDto>()
);
```

**Rationale**:
- DTOs mirror domain structure for simplicity
- Nullable `DayPlans` in domain becomes empty list in DTO (easier UI rendering)
- No additional transformations needed (1:1 mapping)

---

## Repository Interface Extension

**Location**: `CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs` (existing)

**Add Method**:
```csharp
/// <summary>
/// Gets a citytrip by ID including its full itinerary (day plans and attractions).
/// </summary>
/// <param name="id">Citytrip ID</param>
/// <returns>Citytrip with itinerary, or null if not found</returns>
Task<Citytrip?> GetByIdWithItineraryAsync(int id);
```

**Implementation** (`InMemoryCitytripRepository.cs`):
```csharp
public Task<Citytrip?> GetByIdWithItineraryAsync(int id)
{
    var citytrip = _citytrips.FirstOrDefault(c => c.Id == id);
    return Task.FromResult(citytrip);
    // Note: In-memory data already includes DayPlans,
    // so this method just returns the full object.
    // Named differently to signal it includes itinerary.
}
```

---

## Data Relationships Diagram

```
Citytrip (aggregate root)
│
├─ Id: int
├─ Title: string
├─ Destination: string
├─ ImageUrl: string
├─ StartDate: DateOnly
├─ EndDate: DateOnly
├─ CreatorId: string
├─ Description: string?
├─ MaxParticipants: int?
│
└─ DayPlans: List<DayPlan>? (0..*)
    │
    ├─ DayNumber: int
    ├─ Date: DateOnly
    ├─ Timeframe: string
    │
    └─ Attractions: List<Attraction> (0..*)
        │
        ├─ Name: string
        ├─ Description: string
        ├─ WebsiteUrl: string?
        └─ TransportationOptions: List<string> (0..*)
```

---

## Seed Data Example

**Location**: `CitytripPlanner.Infrastructure/Persistence/InMemoryCitytripRepository.cs`

**Sample Itinerary**:
```csharp
new Citytrip(
    Id: 1,
    Title: "Paris Adventure",
    Destination: "Paris, France",
    ImageUrl: "https://images.unsplash.com/photo-1502602898657-3e91760cbb34",
    StartDate: new DateOnly(2026, 6, 1),
    EndDate: new DateOnly(2026, 6, 3),
    CreatorId: "user123",
    Description: "Explore the City of Light",
    MaxParticipants: 10,
    DayPlans: new List<DayPlan>
    {
        new DayPlan(
            DayNumber: 1,
            Date: new DateOnly(2026, 6, 1),
            Timeframe: "Morning 9:00-12:00",
            Attractions: new List<Attraction>
            {
                new Attraction(
                    Name: "Eiffel Tower",
                    Description: "Iconic iron lattice tower with observation decks",
                    WebsiteUrl: "https://www.toureiffel.paris/en",
                    TransportationOptions: new List<string>
                    {
                        "Metro Line 6 to Bir-Hakeim",
                        "RER C to Champ de Mars"
                    }
                ),
                new Attraction(
                    Name: "Trocadéro Gardens",
                    Description: "Beautiful gardens with Eiffel Tower views",
                    WebsiteUrl: null,
                    TransportationOptions: new List<string>
                    {
                        "Walking from Eiffel Tower (10 min)"
                    }
                )
            }
        ),
        new DayPlan(
            DayNumber: 2,
            Date: new DateOnly(2026, 6, 2),
            Timeframe: "Afternoon 14:00-18:00",
            Attractions: new List<Attraction>
            {
                new Attraction(
                    Name: "Louvre Museum",
                    Description: "World's largest art museum",
                    WebsiteUrl: "https://www.louvre.fr/en",
                    TransportationOptions: new List<string>
                    {
                        "Metro Line 1 to Palais Royal - Musée du Louvre"
                    }
                )
            }
        )
    }
)
```

---

## Validation Summary

| Entity | Validation Rules | Spec Reference |
|--------|------------------|----------------|
| **Citytrip** | Existing validations unchanged | N/A |
| **DayPlan** | DayNumber > 0, Attractions not null | FR-003, FR-004 |
| **Attraction** | Name/Description required, WebsiteUrl valid URI if present | FR-005, FR-006, FR-007 |

---

## Next Steps

1. Implement domain entities in `Features/Citytrips/Domain/`
2. Update `ICitytripRepository` interface
3. Implement repository method in `Infrastructure/Persistence/`
4. Create DTOs in `Features/Citytrips/GetCitytripDetail/`
5. Add seed data to `InMemoryCitytripRepository`

See `contracts/` and `quickstart.md` for implementation guidance.
