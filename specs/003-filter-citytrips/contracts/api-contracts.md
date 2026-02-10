# API Contracts: Filter Citytrips

## Overview

This feature does **not** introduce any new MediatR commands or queries. All filtering is performed client-side on the existing `ListCitytripsQuery` results. This document describes the internal contracts (C# types) used for filtering.

## Internal Contracts

### CitytripFilterCriteria (Value Object)

```csharp
namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public record CitytripFilterCriteria(
    string? SearchText = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null);
```

### CitytripFilter (Static Helper)

```csharp
namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public static class CitytripFilter
{
    public static List<CitytripCard> Apply(
        List<CitytripCard> trips,
        CitytripFilterCriteria criteria);
}
```

**Behavior**:
- Returns all trips when no criteria are set (all fields null/empty)
- Text filter: case-insensitive `Contains` on Title OR Destination
- FromDate: keeps trips where `EndDate >= FromDate`
- ToDate: keeps trips where `StartDate <= ToDate`
- All active criteria are ANDed

### Existing Query (unchanged)

```csharp
// No changes — still used to load all trips
public record ListCitytripsQuery : IRequest<List<CitytripCard>>;
```

## No New Endpoints

No new HTTP endpoints or MediatR handlers are introduced. The Blazor component calls `CitytripFilter.Apply()` directly on the client-side data.
