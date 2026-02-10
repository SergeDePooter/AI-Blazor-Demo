# Quickstart: Filter Citytrips

## Prerequisites

- .NET 10 SDK installed
- Project builds successfully: `dotnet build` from `CitytripPlanner/`

## Run the Application

```bash
cd CitytripPlanner/CitytripPlanner.Web
dotnet run
```

Navigate to `https://localhost:5001/` (or the port shown in console output).

## Verify the Feature

1. **Browse page loads**: The main page at `/` or `/citytrips` shows the list of citytrips with a filter area above the grid
2. **Text filter**: Type a destination name (e.g., "Paris") in the search input — the list narrows after a brief pause (~300ms)
3. **Date filter**: Select a "from" date — trips ending before that date disappear. Select a "to" date — trips starting after that date disappear
4. **Combined filters**: Apply both text and date filters — only trips matching all criteria are shown
5. **Empty state**: Apply filters that match no trips — an empty state message appears with a suggestion to clear filters
6. **Clear all**: Click "Clear all filters" — all inputs reset and the full list returns
7. **Existing interactions**: Like and enlist buttons still work correctly on filtered results

## Run Tests

```bash
cd CitytripPlanner/CitytripPlanner.Tests
dotnet test
```

All existing tests plus new `CitytripFilterTests` should pass.
