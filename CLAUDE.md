# demo-speckit-ai Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-02-07

## Active Technologies
- C# / .NET 10 + MediatR 14.0.0, Blazor Server (built-in), bUnit 2.5.3 (testing) (002-manage-citytrips)
- In-memory (extending existing InMemoryCitytripRepository pattern) (002-manage-citytrips)
- C# / .NET 10 + Blazor Server (built-in), MediatR 14.x (existing) (003-filter-citytrips)
- N/A — filtering is client-side on existing in-memory data (003-filter-citytrips)
- In-memory (new InMemoryUserProfileRepository following existing patterns) (004-user-profile-page)
- C# / .NET 10 + Blazor Server (built-in), MediatR 14.0.0 (existing), IJSRuntime (built-in Blazor, for Google Maps interop) (006-trip-day-schedule)
- In-memory (extending existing `InMemoryCitytripRepository` seed data with `ScheduledEvent` and `Place` data) (006-trip-day-schedule)
- C# / .NET 10 + Blazor Server (built-in), MediatR 14.x (existing), bUnit 2.5.3 (tests), xUnit (tests), FluentAssertions (tests) (007-create-citytrip-map)
- In-memory (`InMemoryCitytripRepository` — no changes needed) (007-create-citytrip-map)
- In-memory (`InMemoryCitytripRepository` — `UpdateAsync` already replaces the full record) (008-edit-citytrip-wizard)
- C# / .NET 10, Blazor Server + Bootstrap 5.x (already in wwwroot/lib), scoped Blazor CSS isolation (009-fix-mobile-layout)
- N/A — layout fix only (009-fix-mobile-layout)

- C# / .NET 10 + Blazor Server (built-in), MediatR (001-browse-citytrips)

## Project Structure

```text
backend/
frontend/
tests/
```

## Commands

# Add commands for C# / .NET 10

## Code Style

C# / .NET 10: Follow standard conventions

## Recent Changes
- 009-fix-mobile-layout: Added C# / .NET 10, Blazor Server + Bootstrap 5.x (already in wwwroot/lib), scoped Blazor CSS isolation
- 008-edit-citytrip-wizard: Added C# / .NET 10 + Blazor Server (built-in), MediatR 14.x (existing), bUnit 2.5.3 (tests), xUnit (tests), FluentAssertions (tests)
- 007-create-citytrip-map: Added C# / .NET 10 + Blazor Server (built-in), MediatR 14.x (existing), bUnit 2.5.3 (tests), xUnit (tests), FluentAssertions (tests)


<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
