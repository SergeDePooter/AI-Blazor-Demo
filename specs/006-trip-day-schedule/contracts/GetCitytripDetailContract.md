# Contract: GetCitytripDetail (Extended)

**Feature**: 006-trip-day-schedule
**Date**: 2026-02-24
**Type**: MediatR Query (existing slice, extended)
**Location**: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/`

---

## Query (unchanged)

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

public record GetCitytripDetailQuery(int Id) : IRequest<CitytripDetailResponse?>;
```

**Used by**: `CitytripDetail.razor` on `OnParametersSetAsync`
**Returns**: `CitytripDetailResponse?` — null if trip not found (renders 404 message)

---

## Response (extended)

```csharp
// Root response — unchanged fields
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

// Extended: Events added
public record DayPlanDetail(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<AttractionDetail> Attractions,       // existing
    List<ScheduledEventDetail> Events);        // NEW

// Existing — unchanged
public record AttractionDetail(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);

// NEW
public record ScheduledEventDetail(
    string EventType,
    string Name,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    string? Description,
    string? Notes,
    PlaceDetail? Place);

// NEW
public record PlaceDetail(
    string Name,
    double Latitude,
    double Longitude,
    string? Address);
```

---

## Handler Contract

**Class**: `GetCitytripDetailHandler : IRequestHandler<GetCitytripDetailQuery, CitytripDetailResponse?>`

**Inputs**:
- `GetCitytripDetailQuery.Id` — citytrip ID (int, must be > 0)

**Process**:
1. Call `_repository.GetByIdWithItineraryAsync(query.Id)`
2. Return `null` if trip not found
3. Project `Citytrip → CitytripDetailResponse`, including:
   - `DayPlans` → `List<DayPlanDetail>` (ordered by `DayNumber`)
   - Each `DayPlan.Events` → `List<ScheduledEventDetail>` (ordered by `StartTime`)
   - Each `DayPlan.Attractions` → `List<AttractionDetail>` (existing, unchanged)

**Outputs**:
- `CitytripDetailResponse` with populated `Events` in each `DayPlanDetail`
- `null` if no trip with the given ID exists

**Error handling**:
- Handler does not throw; returns `null` for not-found case
- Blazor page renders a not-found message when response is `null`

---

## JS Interop Contract

**File**: `CitytripPlanner.Web/wwwroot/js/trip-map.js`

| JS Function | Called from | Parameters | Returns |
|-------------|-------------|------------|---------|
| `tripMap.initMap(elementId, markers)` | `TripMapSidebar.razor` (OnAfterRenderAsync) | `elementId: string`, `markers: MapMarker[]` | `void` |
| `tripMap.updateMarkers(markers)` | `TripMapSidebar.razor` (OnParametersSetAsync) | `markers: MapMarker[]` | `void` |
| `tripMap.destroyMap(elementId)` | `TripMapSidebar.razor` (DisposeAsync) | `elementId: string` | `void` |
| `tripMap.observeDaySections(dotNetRef)` | `CitytripDetail.razor` (OnAfterRenderAsync) | `dotNetRef: DotNetObjectReference` | `void` |

**MapMarker JS type**:
```typescript
interface MapMarker {
  lat: number;
  lng: number;
  label: string;   // event name shown in info window
}
```

**DotNet callback** (called from JS via `IntersectionObserver`):
```csharp
[JSInvokable]
public void OnDayChanged(int dayNumber)  // in CitytripDetail.razor
```

---

## Blazor Component Interface

### TripMapSidebar.razor

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Markers` | `IReadOnlyList<PlaceDetail>` | Yes | Places to pin on the map for the active day |

### DaySchedulePanel.razor

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `DayPlans` | `List<DayPlanDetail>` | Yes | All day plans for the trip |
| `OnDayVisible` | `EventCallback<int>` | Yes | Fired when a new day section enters the viewport (dayNumber) |

### ScheduledEventCard.razor

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Event` | `ScheduledEventDetail` | Yes | The event to render |
