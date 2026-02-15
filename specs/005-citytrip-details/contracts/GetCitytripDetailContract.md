# Contract: GetCitytripDetail

**Feature**: 005-citytrip-details
**Date**: 2026-02-14
**Type**: MediatR Query (CQRS Read Operation)
**Purpose**: Retrieve a single citytrip with full itinerary details for display on the detail page

## Query Contract

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailQuery.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

using MediatR;

/// <summary>
/// Query to retrieve a citytrip's details including full itinerary.
/// </summary>
/// <param name="CitytripId">The ID of the citytrip to retrieve</param>
public record GetCitytripDetailQuery(int CitytripId) : IRequest<CitytripDetailResponse?>;
```

**Properties**:
- `CitytripId` (int): Primary key of the citytrip to fetch

**Returns**: `CitytripDetailResponse?` (nullable)
- Non-null when citytrip exists
- Null when citytrip not found (ID doesn't exist)

**Spec Mapping**:
- FR-001: Enables navigation to detail page
- FR-010: Supports direct URL access (deep linking)

---

## Response Contract

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

/// <summary>
/// Response containing citytrip details and full itinerary.
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

**CitytripDetailResponse Properties**:
- `Id` (int): Citytrip identifier
- `Title` (string): Trip title (e.g., "Paris Adventure")
- `Destination` (string): City/country (e.g., "Paris, France")
- `ImageUrl` (string): Cover photo URL
- `StartDate` (DateOnly): Trip start date
- `EndDate` (DateOnly): Trip end date
- `Description` (string?): Optional trip description
- `MaxParticipants` (int?): Optional max participant count
- `DayPlans` (List<DayPlanDto>): Itinerary (empty list if no itinerary)

**DayPlanDto Properties**:
- `DayNumber` (int): Sequential day number (1-based)
- `Date` (DateOnly): Calendar date
- `Timeframe` (string): Time range (e.g., "Morning 9:00-12:00")
- `Attractions` (List<AttractionDto>): Places to visit this day

**AttractionDto Properties**:
- `Name` (string): Attraction name
- `Description` (string): Brief description
- `WebsiteUrl` (string?): Optional external link
- `TransportationOptions` (List<string>): How to get there

**Spec Mapping**:
- FR-002: All basic citytrip info included
- FR-003: Day-by-day itinerary via `DayPlans`
- FR-004: Timeframes included in `DayPlanDto`
- FR-005: Attractions with name/description
- FR-006: Optional website links
- FR-007: Transportation options list
- FR-008: Empty `DayPlans` list when no itinerary

---

## Handler Contract

**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

/// <summary>
/// Handles retrieval of citytrip details with itinerary.
/// </summary>
public class GetCitytripDetailHandler : IRequestHandler<GetCitytripDetailQuery, CitytripDetailResponse?>
{
    private readonly ICitytripRepository _repository;

    public GetCitytripDetailHandler(ICitytripRepository repository)
    {
        _repository = repository;
    }

    public async Task<CitytripDetailResponse?> Handle(
        GetCitytripDetailQuery request,
        CancellationToken cancellationToken)
    {
        var citytrip = await _repository.GetByIdWithItineraryAsync(request.CitytripId);

        if (citytrip is null)
            return null;

        return new CitytripDetailResponse(
            Id: citytrip.Id,
            Title: citytrip.Title,
            Destination: citytrip.Destination,
            ImageUrl: citytrip.ImageUrl,
            StartDate: citytrip.StartDate,
            EndDate: citytrip.EndDate,
            Description: citytrip.Description,
            MaxParticipants: citytrip.MaxParticipants,
            DayPlans: MapDayPlans(citytrip.DayPlans)
        );
    }

    private static List<DayPlanDto> MapDayPlans(List<DayPlan>? dayPlans)
    {
        if (dayPlans is null or { Count: 0 })
            return new List<DayPlanDto>();

        return dayPlans
            .OrderBy(dp => dp.DayNumber)
            .Select(dp => new DayPlanDto(
                DayNumber: dp.DayNumber,
                Date: dp.Date,
                Timeframe: dp.Timeframe,
                Attractions: dp.Attractions.Select(a => new AttractionDto(
                    Name: a.Name,
                    Description: a.Description,
                    WebsiteUrl: a.WebsiteUrl,
                    TransportationOptions: a.TransportationOptions
                )).ToList()
            ))
            .ToList();
    }
}
```

**Handler Behavior**:
1. Fetch citytrip from repository using `GetByIdWithItineraryAsync`
2. Return `null` if citytrip not found (404 scenario)
3. Map domain model to response DTO
4. Ensure day plans are sorted by `DayNumber` (chronological order - FR-003)
5. Return empty `DayPlans` list if no itinerary exists (FR-008)

**Dependencies**:
- `ICitytripRepository`: Injected via constructor (DI)

---

## Repository Contract Extension

**Location**: `CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs`

**Add Method**:
```csharp
/// <summary>
/// Gets a citytrip by ID including its full itinerary (day plans and attractions).
/// </summary>
/// <param name="id">Citytrip ID</param>
/// <returns>Citytrip with itinerary, or null if not found</returns>
Task<Citytrip?> GetByIdWithItineraryAsync(int id);
```

**Implementation** (in `InMemoryCitytripRepository`):
```csharp
public Task<Citytrip?> GetByIdWithItineraryAsync(int id)
{
    var citytrip = _citytrips.FirstOrDefault(c => c.Id == id);
    return Task.FromResult(citytrip);
}
```

---

## Error Scenarios

| Scenario | Query Input | Response | HTTP Status (if web API) |
|----------|-------------|----------|--------------------------|
| **Success** | Valid citytrip ID with itinerary | Full `CitytripDetailResponse` | 200 OK |
| **Success (no itinerary)** | Valid citytrip ID, no day plans | `CitytripDetailResponse` with empty `DayPlans` | 200 OK |
| **Not Found** | Non-existent citytrip ID | `null` | 404 Not Found |
| **Invalid ID** | Negative or zero ID | `null` (or validation error if validator added) | 404 Not Found |

**Note**: Blazor page component should handle `null` response by displaying "Citytrip not found" message.

---

## Blazor Component Integration

**Location**: `CitytripPlanner.Web/Pages/CitytripDetail.razor`

**Usage Pattern**:
```csharp
@page "/citytrips/{Id:int}"
@inject IMediator Mediator

<PageTitle>Citytrip Details</PageTitle>

@if (_citytrip is null)
{
    <p>Citytrip not found.</p>
}
else
{
    <!-- Render citytrip details -->
    <h1>@_citytrip.Title</h1>
    <!-- ... -->

    @if (_citytrip.DayPlans.Any())
    {
        @foreach (var day in _citytrip.DayPlans)
        {
            <DayPlanCard Day="@day" />
        }
    }
    else
    {
        <p>No itinerary available yet.</p>
    }
}

@code {
    [Parameter]
    public int Id { get; set; }

    private CitytripDetailResponse? _citytrip;

    protected override async Task OnParametersSetAsync()
    {
        var query = new GetCitytripDetailQuery(Id);
        _citytrip = await Mediator.Send(query);
    }
}
```

---

## Testing Scenarios

### Unit Tests (Handler)

**Location**: `CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetailTests.cs`

**Test Cases** (TDD - write these first):

1. **Given** valid citytrip ID with itinerary, **When** handling query, **Then** return full response with day plans
2. **Given** valid citytrip ID without itinerary, **When** handling query, **Then** return response with empty DayPlans
3. **Given** non-existent citytrip ID, **When** handling query, **Then** return null
4. **Given** multiple day plans, **When** handling query, **Then** day plans ordered by DayNumber

### Integration Tests (Blazor Component)

**Location**: `CitytripPlanner.Tests/Web/Pages/CitytripDetailPageTests.cs` (using bUnit)

**Test Cases**:

1. **Given** valid citytrip ID, **When** page loads, **Then** citytrip title displayed
2. **Given** citytrip with 2 day plans, **When** page renders, **Then** 2 DayPlanCard components rendered
3. **Given** non-existent citytrip ID, **When** page loads, **Then** "not found" message displayed
4. **Given** citytrip with no itinerary, **When** page renders, **Then** "no itinerary" message displayed

---

## Performance Considerations

- **Query Complexity**: Single repository call (`O(1)` for in-memory, indexed lookup for DB)
- **Data Size**: Itineraries with 30+ days and 100+ attractions should render without lag (see spec SC-006)
- **Caching**: Not needed for read-only in-memory data; consider for future DB implementation

---

## Future Considerations (Out of Scope)

- **Validation**: FluentValidation for `GetCitytripDetailQuery` (currently relies on route constraint)
- **Authorization**: Check if user has permission to view trip (public vs. private trips)
- **Localization**: Multi-language support for attraction descriptions
- **Real-time Updates**: SignalR for live itinerary updates (requires edit feature first)

---

## Summary

| Artifact | Location | Purpose |
|----------|----------|---------|
| **Query** | `Features/Citytrips/GetCitytripDetail/GetCitytripDetailQuery.cs` | Input contract (citytrip ID) |
| **Response** | `Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs` | Output contract (DTOs) |
| **Handler** | `Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs` | Business logic |
| **Repository** | `Features/Citytrips/Domain/ICitytripRepository.cs` | Data access interface |
| **Tests** | `Tests/Features/Citytrips/GetCitytripDetailTests.cs` | Unit tests (TDD) |
