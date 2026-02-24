# Research: Citytrip Detail Page with Day Schedule and Map

**Feature**: 006-trip-day-schedule
**Date**: 2026-02-24
**Purpose**: Document technical decisions for the generic day schedule, event-type icons, Google Maps integration, and scroll-sync sidebar.

## Overview

This feature extends the existing `CitytripDetail.razor` page. All research focuses on:
1. Extending the existing domain model cleanly
2. Integrating Google Maps via Blazor JS interop (no new NuGet package)
3. Scroll-sync between the schedule column and the map sidebar
4. Event-type icon strategy for generic free-form event types

---

## Research Area 1: Domain Model Extension Strategy

**Decision**: Add `ScheduledEvent` and `Place` as new optional entities on the existing `DayPlan` record.

**Rationale**:
- `ScheduledEvent` is semantically distinct from `Attraction` (timed, has a generic type label, has optional geographic coordinates).
- Adding `List<ScheduledEvent>? Events = null` to `DayPlan` is backward-compatible: existing code using `Attractions` continues to compile and work unchanged.
- The `GetCitytripDetailHandler` extends its projection to populate `List<ScheduledEventDetail>` in the response DTO alongside existing `List<AttractionDetail>`.
- A future migration to fully replace `Attractions` with `ScheduledEvent` is deferred (YAGNI: only one trip currently uses the itinerary, and the spec does not ask to remove Attractions).

**Alternatives Considered**:
- *Replace `Attraction` with `ScheduledEvent`*: Rejected — breaks existing tests and UI without a stated requirement to remove Attractions.
- *Separate domain model per DayPlan*: Rejected — adds a second aggregate; in-memory data is simple enough that a single `DayPlan` can own both lists.
- *Generic event type enum*: Rejected — spec explicitly requires free-form labels with no enforced category list.

---

## Research Area 2: Google Maps Integration via IJSRuntime

**Decision**: Use built-in `IJSRuntime.InvokeVoidAsync` and `InvokeAsync` to drive a small vanilla-JS Google Maps file (`wwwroot/js/trip-map.js`). No new NuGet package.

**Rationale**:
- `IJSRuntime` is already injected in every Blazor Server component.
- A hand-written JS file avoids a NuGet dependency that would need justification (Constitution IV).
- Blazor's `<script>` tag injection via `<HeadContent>` or static `index.html` loads the Maps script with the API key from server-side configuration (rendered into the HTML at startup via Blazor's `App.razor` or environment-variable placeholder).

**JS API surface** (`trip-map.js`):
```javascript
window.tripMap = {
  initMap(elementId, markers)   // markers: [{lat, lng, label}]
  updateMarkers(markers)         // replace visible markers
  destroyMap(elementId)          // cleanup on component dispose
}
```

**Blazor side** (`TripMapSidebar.razor`):
```csharp
@inject IJSRuntime JS
@implements IAsyncDisposable

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
        await JS.InvokeVoidAsync("tripMap.initMap", _elementId, _markers);
}

public async ValueTask DisposeAsync()
    => await JS.InvokeVoidAsync("tripMap.destroyMap", _elementId);
```

**API key delivery**: The Google Maps script URL contains the API key. In Blazor Server this is rendered server-side. The key is read from `IConfiguration` (`GoogleMaps:ApiKey`) and injected into `App.razor` or the `<head>` section via a Razor expression. It is never exposed in C# code or committed to source control.

**Alternatives Considered**:
- *BlazorGoogleMaps NuGet* — Rejected: requires new package justification, adds build complexity, and wraps the same Maps JS API.
- *Leaflet + OpenStreetMap* — Rejected: spec explicitly requests Google Maps.
- *iframe embed* — Rejected: does not support dynamic marker updates or scroll-sync.

---

## Research Area 3: Scroll-Sync via IntersectionObserver

**Decision**: Use the browser-native `IntersectionObserver` API in `trip-map.js` to detect when a day section enters the viewport and call back into Blazor via `DotNetObjectReference`.

**Rationale**:
- `IntersectionObserver` is supported in all modern browsers and requires zero dependencies.
- Calling `.NET` methods from JS via `DotNetObjectReference` is the official Blazor interop pattern for JS-to-.NET callbacks.
- The observer watches each `[data-day="N"]` element in the schedule column; when it intersects >50% of the viewport, it calls `dotNetRef.invokeMethodAsync('OnDayChanged', dayNumber)`.

**JS pattern**:
```javascript
function observeDaySections(dotNetRef) {
  const observer = new IntersectionObserver(entries => {
    const visible = entries.find(e => e.isIntersecting);
    if (visible) {
      const day = visible.target.dataset.day;
      dotNetRef.invokeMethodAsync('OnDayChanged', parseInt(day));
    }
  }, { threshold: 0.5 });

  document.querySelectorAll('[data-day]').forEach(el => observer.observe(el));
}
```

**Blazor side** (`CitytripDetail.razor`):
```csharp
[JSInvokable]
public void OnDayChanged(int dayNumber)
{
    _activeDay = dayNumber;
    // Pass new marker set to TripMapSidebar via parameter
    StateHasChanged();
}
```

**Alternatives Considered**:
- *Scroll event listener* — Rejected: fires too frequently; requires manual throttle logic.
- *Blazor scroll tracking without JS* — Rejected: Blazor Server has no native scroll position API.
- *Day tab selector (manual click)* — Rejected: user selected scroll-sync in clarifications (Option C).

---

## Research Area 4: Event Type Icon Strategy

**Decision**: Use a `static readonly Dictionary<string, string>` in the Web layer mapping known event type labels (case-insensitive) to SVG icon strings or Unicode symbols. Unknown types fall back to a generic "pin" icon.

**Rationale**:
- Simple to implement, easy to extend, no runtime overhead.
- Keeps icon logic in the Web layer (presentation concern), not in Features (Constitution III).
- Free-form labels match case-insensitively to cover "museum", "Museum", "MUSEUM".

**Example mapping** (`EventTypeIconMap.cs` in `CitytripPlanner.Web/`):
```csharp
public static class EventTypeIconMap
{
    private static readonly Dictionary<string, string> _icons =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["museum"]   = "🏛",
            ["market"]   = "🛒",
            ["stadium"]  = "🏟",
            ["park"]     = "🌳",
            ["restaurant"] = "🍽",
            ["church"]   = "⛪",
            ["beach"]    = "🏖",
            ["gallery"]  = "🖼",
        };

    public static string GetIcon(string eventType)
        => _icons.TryGetValue(eventType, out var icon) ? icon : "📍";
}
```

**Alternatives Considered**:
- *Icon font (Font Awesome)* — Rejected: adds a CDN dependency; emoji Unicode is simpler and sufficient.
- *Predefined enum list* — Rejected: spec requires free-form labels with no enforced category.
- *SVG icon per type* — Deferred: can replace emoji with SVG in a future styling pass without changing the mapping structure.

---

## Research Area 5: Two-Column Sticky Layout

**Decision**: CSS `position: sticky` on the map sidebar column; the schedule column scrolls normally. Use CSS Grid or Flexbox for the two-column layout.

**Rationale**:
- Native CSS `position: sticky` requires zero JavaScript and is supported in all modern browsers.
- No new NuGet or JS library required (Constitution IV).
- The map sidebar gets `height: calc(100vh - <nav-height>)` and `overflow: hidden` to keep it fixed within the viewport.

**CSS pattern** (`CitytripDetail.razor.css`):
```css
.detail-layout {
    display: grid;
    grid-template-columns: 1fr 400px;
    gap: 1.5rem;
    align-items: start;
}

.schedule-column {
    /* scrolls with the page */
}

.map-column {
    position: sticky;
    top: 64px;              /* height of NavMenu */
    height: calc(100vh - 80px);
}
```

**Alternatives Considered**:
- *`position: fixed`* — Rejected: requires manual offset calculation and breaks when layout shifts.
- *JavaScript-based sticky* — Rejected: more complex, unnecessary given CSS support.

---

## Summary of Decisions

| Area | Decision | Key Benefit |
|------|----------|-------------|
| Domain extension | Add `ScheduledEvent` + `Place` to `DayPlan` (optional, alongside Attractions) | Backward-compatible; no breaking changes |
| Map integration | `IJSRuntime` + vanilla `trip-map.js` | No new NuGet; Constitution IV compliant |
| Scroll-sync | `IntersectionObserver` + `DotNetObjectReference` callback | Zero-dependency; browser-native |
| Event type icons | `Dictionary<string, string>` in Web layer, emoji fallback | Simple; extensible; no extra dependencies |
| Two-column sticky | CSS Grid + `position: sticky` on map column | Zero JavaScript; widely supported |

## Open Questions

None — all technical decisions resolved.

## Next Steps

Phase 1:
1. Generate `data-model.md` with concrete C# record definitions
2. Generate `contracts/GetCitytripDetailContract.md` with extended MediatR request/response
3. Generate `quickstart.md` with TDD workflow
