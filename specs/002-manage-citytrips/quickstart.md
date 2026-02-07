# Quickstart: Manage Citytrips

**Feature**: 002-manage-citytrips
**Date**: 2026-02-07

## Prerequisites

- .NET 10 SDK
- A terminal / IDE with .NET support

## Run the Application

```bash
cd CitytripPlanner/CitytripPlanner.Web
dotnet run
```

Navigate to `https://localhost:<port>/my-citytrips` to see the new page.

## Run Tests

```bash
cd CitytripPlanner/CitytripPlanner.Tests
dotnet test
```

## New Feature Slices

| Slice | Type | Location |
|-------|------|----------|
| CreateTrip | Command | `Features/Citytrips/CreateTrip/` |
| UpdateTrip | Command | `Features/Citytrips/UpdateTrip/` |
| DeleteTrip | Command | `Features/Citytrips/DeleteTrip/` |
| GetMyTrips | Query | `Features/Citytrips/GetMyTrips/` |
| GetEnlistedTrips | Query | `Features/Citytrips/GetEnlistedTrips/` |

## New UI Components

| Component | Location | Purpose |
|-----------|----------|---------|
| MyCitytrips.razor | `Web/Components/Pages/` | Main page with tab toggle |
| TripFormModal.razor | `Web/Components/Shared/` | Create/Edit modal dialog |
| DeleteConfirmModal.razor | `Web/Components/Shared/` | Delete confirmation dialog |

## Key Design Decisions

1. **User Identity**: Hardcoded `ICurrentUserService` (no real auth) — returns a fixed demo user
2. **Modal dialogs**: Pure CSS overlays, no JS interop
3. **Tab toggle**: Simple enum state (`MyTrips` / `EnlistedTrips`), conditional rendering
4. **Domain evolution**: Citytrip record gets new fields (StartDate, EndDate, MaxParticipants, CreatorId); CityName renamed to Title + Destination
5. **Backward compatibility**: Feature 001 browse page updated to use new field names
