# Quickstart: Browse Citytrips

**Feature Branch**: `001-browse-citytrips`
**Date**: 2026-02-06

## Prerequisites

- .NET 10 SDK installed
- A code editor (Visual Studio, Rider, or VS Code)

## Running the Application

1. Clone the repository and checkout the feature branch:
   ```bash
   git checkout 001-browse-citytrips
   ```

2. Restore dependencies:
   ```bash
   dotnet restore CitytripPlanner/CitytripPlanner.slnx
   ```

3. Run the web project:
   ```bash
   dotnet run --project CitytripPlanner/CitytripPlanner.Web
   ```

4. Open a browser and navigate to:
   - HTTPS: `https://localhost:7043`
   - HTTP: `http://localhost:5190`

## Verifying the Feature

### Browse Citytrips (US1)
1. Navigate to the home page
2. Verify citytrip cards are displayed in a masonry grid
3. Each card shows: image, city name, duration in days,
   like button, enlist button
4. If no trips loaded, verify empty-state message appears

### Horizontal Navigation (US2)
1. Verify the horizontal text-only menu at the top
2. Selected page label is orange with underline
3. Non-selected labels are orange at 75% opacity
4. No left sidebar is present
5. Click menu items to navigate between pages

### Like a Citytrip (US3)
1. Click the like button on any trip card
2. Verify the button toggles to a liked state
3. Click again to unlike

### Enlist for a Citytrip (US4)
1. Click the enlist button on any trip card
2. Verify a toast notification confirms enlistment
3. Verify the enlist button shows enlisted state
4. Refresh the page — state resets (session-only)

## Running Tests

```bash
dotnet test CitytripPlanner/CitytripPlanner.slnx
```

## Project Structure

```
CitytripPlanner/
├── CitytripPlanner.Web/          # Blazor Server pages & layout
│   └── Components/
│       ├── Layout/               # MainLayout, NavMenu (horizontal)
│       └── Pages/                # Citytrips page
├── CitytripPlanner.Features/     # CQRS handlers & domain
│   └── Citytrips/
│       ├── Domain/               # Entities, interfaces
│       ├── ListCitytrips/        # Query slice
│       ├── ToggleLike/           # Command slice
│       └── Enlist/               # Command slice
└── CitytripPlanner.Infrastructure/ # Data access
    └── Citytrips/                # In-memory repositories
```
