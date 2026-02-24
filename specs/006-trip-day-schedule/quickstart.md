# Quickstart: Citytrip Detail Page with Day Schedule and Map

**Feature**: 006-trip-day-schedule
**Date**: 2026-02-24
**TDD Workflow**: Red → Green → Refactor, strictly enforced

---

## Prerequisites

- .NET 10 SDK installed
- Solution builds cleanly on `006-trip-day-schedule` branch
- Google Maps API key available (set in `appsettings.Development.json` or environment variable `GoogleMaps__ApiKey`)

---

## TDD Implementation Order

Follow this order strictly. Do NOT write production code before the corresponding failing test exists.

---

### Step 1: Domain — Place entity

**Test first** (`CitytripPlanner.Tests/Citytrips/Domain/PlaceTests.cs`):
```csharp
[Fact]
public void Place_WithValidCoordinates_Creates()
{
    var place = new Place("Eiffel Tower", 48.8584, 2.2945);
    Assert.Equal("Eiffel Tower", place.Name);
}

[Theory]
[InlineData(-91, 0)]
[InlineData(91, 0)]
[InlineData(0, -181)]
[InlineData(0, 181)]
public void Place_WithInvalidCoordinates_Throws(double lat, double lng)
    => Assert.Throws<ArgumentOutOfRangeException>(() => new Place("X", lat, lng));

[Fact]
public void Place_WithEmptyName_Throws()
    => Assert.Throws<ArgumentException>(() => new Place("", 48.0, 2.0));
```

**Then implement** `CitytripPlanner.Features/Citytrips/Domain/Place.cs`.

---

### Step 2: Domain — ScheduledEvent entity

**Test first** (`CitytripPlanner.Tests/Citytrips/Domain/ScheduledEventTests.cs`):
```csharp
[Fact]
public void ScheduledEvent_WithValidData_Creates()
{
    var ev = new ScheduledEvent("Museum", "Louvre", new TimeOnly(14, 0));
    Assert.Equal("Museum", ev.EventType);
    Assert.Equal("Louvre", ev.Name);
    Assert.Null(ev.EndTime);
    Assert.Null(ev.Place);
}

[Fact]
public void ScheduledEvent_EndTimeBeforeStartTime_Throws()
    => Assert.Throws<ArgumentException>(() =>
        new ScheduledEvent("Museum", "Louvre", new TimeOnly(14, 0), new TimeOnly(13, 0)));

[Theory]
[InlineData("", "Name")]
[InlineData("Type", "")]
public void ScheduledEvent_WithEmptyRequiredField_Throws(string type, string name)
    => Assert.Throws<ArgumentException>(() =>
        new ScheduledEvent(type, name, new TimeOnly(9, 0)));

[Fact]
public void ScheduledEvent_WithPlace_ExposesPlace()
{
    var place = new Place("Tower", 48.85, 2.29);
    var ev = new ScheduledEvent("Landmark", "Eiffel", new TimeOnly(9, 0), place: place);
    Assert.NotNull(ev.Place);
}
```

**Then implement** `CitytripPlanner.Features/Citytrips/Domain/ScheduledEvent.cs`.

---

### Step 3: Domain — DayPlan extension

**Test first** (`CitytripPlanner.Tests/Citytrips/Domain/DayPlanTests.cs` — extend existing file):
```csharp
[Fact]
public void DayPlan_WithNoEvents_HasEmptyEventsList()
{
    var dp = new DayPlan(1, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>());
    Assert.Empty(dp.Events);
}

[Fact]
public void DayPlan_WithEvents_ExposesEvents()
{
    var events = new List<ScheduledEvent>
    {
        new ScheduledEvent("Museum", "Louvre", new TimeOnly(14, 0))
    };
    var dp = new DayPlan(1, new DateOnly(2026, 6, 1), "Afternoon", new List<Attraction>(), events);
    Assert.Single(dp.Events);
}
```

**Then implement** the `Events` parameter extension in `DayPlan.cs`.

---

### Step 4: Response DTOs

No test needed for pure data records. Add `ScheduledEventDetail` and `PlaceDetail` to `CitytripDetailResponse.cs`, and extend `DayPlanDetail` with `List<ScheduledEventDetail> Events`.

---

### Step 5: Handler — projection

**Test first** (`CitytripPlanner.Tests/Citytrips/GetCitytripDetail/GetCitytripDetailTests.cs` — extend existing):
```csharp
[Fact]
public async Task Handle_TripWithEvents_ReturnsEventsInResponse()
{
    // Arrange
    var place = new Place("Eiffel Tower", 48.8584, 2.2945);
    var ev = new ScheduledEvent("Landmark", "Eiffel Tower Visit", new TimeOnly(9, 0),
        endTime: new TimeOnly(11, 0), place: place);
    var dayPlan = new DayPlan(1, new DateOnly(2026, 6, 1), "Morning",
        new List<Attraction>(), new List<ScheduledEvent> { ev });
    var citytrip = new Citytrip(1, "Paris", "Paris, France", "",
        new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 2), "user",
        DayPlans: new List<DayPlan> { dayPlan });

    var repo = Substitute.For<ICitytripRepository>();
    repo.GetByIdWithItineraryAsync(1).Returns(citytrip);
    var handler = new GetCitytripDetailHandler(repo);

    // Act
    var result = await handler.Handle(new GetCitytripDetailQuery(1), CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    var day = result!.DayPlans!.First();
    Assert.Single(day.Events);
    Assert.Equal("Landmark", day.Events[0].EventType);
    Assert.Equal("Eiffel Tower", day.Events[0].Place!.Name);
    Assert.Equal(48.8584, day.Events[0].Place!.Latitude);
}

[Fact]
public async Task Handle_TripWithEventsOrderedByStartTime()
{
    var events = new List<ScheduledEvent>
    {
        new ScheduledEvent("Market", "Late Market", new TimeOnly(11, 0)),
        new ScheduledEvent("Museum", "Early Museum", new TimeOnly(9, 0))
    };
    var dayPlan = new DayPlan(1, new DateOnly(2026, 6, 1), "Morning",
        new List<Attraction>(), events);
    var citytrip = new Citytrip(1, "Paris", "Paris, France", "",
        new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 2), "user",
        DayPlans: new List<DayPlan> { dayPlan });

    var repo = Substitute.For<ICitytripRepository>();
    repo.GetByIdWithItineraryAsync(1).Returns(citytrip);
    var handler = new GetCitytripDetailHandler(repo);

    var result = await handler.Handle(new GetCitytripDetailQuery(1), CancellationToken.None);

    var day = result!.DayPlans!.First();
    Assert.Equal("Early Museum", day.Events[0].Name);   // ordered by StartTime
    Assert.Equal("Late Market", day.Events[1].Name);
}
```

**Then implement** the Events projection in `GetCitytripDetailHandler.cs` (order events by `StartTime`).

---

### Step 6: Seed data

Extend `InMemoryCitytripRepository.cs` Paris trip with `Events` on each `DayPlan`. See `data-model.md` for the full seed data example. No test needed (infrastructure seed data verified via integration/handler tests above).

---

### Step 7: Blazor — ScheduledEventCard component

**Test first** (`CitytripPlanner.Tests/Web/Citytrips/ScheduledEventCardTests.cs`):
```csharp
[Fact]
public void ScheduledEventCard_RendersEventType()
{
    var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
        null, null, null, null);
    var cut = ctx.RenderComponent<ScheduledEventCard>(p =>
        p.Add(c => c.Event, ev));

    cut.MarkupMatches(/*...contains "Museum" and "Louvre"...*/);
}

[Fact]
public void ScheduledEventCard_WithPlace_RendersLocationName()
{
    var place = new PlaceDetail("Louvre Museum", 48.8606, 2.3376, null);
    var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
        null, null, null, place);
    var cut = ctx.RenderComponent<ScheduledEventCard>(p =>
        p.Add(c => c.Event, ev));

    Assert.Contains("Louvre Museum", cut.Markup);
}
```

**Then implement** `CitytripPlanner.Web/Components/Citytrips/ScheduledEventCard.razor`.

---

### Step 8: Blazor — DaySchedulePanel component

**Test first** (`CitytripPlanner.Tests/Web/Citytrips/DaySchedulePanelTests.cs`):
```csharp
[Fact]
public void DaySchedulePanel_RendersDayHeaders()
{
    var dayPlans = new List<DayPlanDetail>
    {
        new(1, new DateOnly(2026, 6, 1), "Morning", new(), new()),
        new(2, new DateOnly(2026, 6, 2), "Afternoon", new(), new())
    };
    var cut = ctx.RenderComponent<DaySchedulePanel>(p =>
        p.Add(c => c.DayPlans, dayPlans));

    Assert.Contains("Day 1", cut.Markup);
    Assert.Contains("Day 2", cut.Markup);
}

[Fact]
public void DaySchedulePanel_RendersDataDayAttributes()
{
    var dayPlans = new List<DayPlanDetail>
    {
        new(1, new DateOnly(2026, 6, 1), "Morning", new(), new())
    };
    var cut = ctx.RenderComponent<DaySchedulePanel>(p =>
        p.Add(c => c.DayPlans, dayPlans));

    // data-day attribute is required for IntersectionObserver
    Assert.Contains("data-day=\"1\"", cut.Markup);
}
```

**Then implement** `CitytripPlanner.Web/Components/Citytrips/DaySchedulePanel.razor`.

---

### Step 9: Blazor — TripMapSidebar component

**Test first** (`CitytripPlanner.Tests/Web/Citytrips/TripMapSidebarTests.cs`):
```csharp
[Fact]
public void TripMapSidebar_WithNoMarkers_RendersMapContainer()
{
    var cut = ctx.RenderComponent<TripMapSidebar>(p =>
        p.Add(c => c.Markers, new List<PlaceDetail>()));

    // map div must always render (JS will init it)
    cut.Find("[id^='trip-map']");
}
```

**Then implement** `CitytripPlanner.Web/Components/Citytrips/TripMapSidebar.razor` and `wwwroot/js/trip-map.js`.

---

### Step 10: Blazor — CitytripDetail page update

Update `CitytripDetail.razor`:
1. Change layout to two-column grid (`detail-layout` CSS class)
2. Render `<DaySchedulePanel>` in the schedule column, passing `OnDayVisible` callback
3. Render `<TripMapSidebar>` in the map column, passing markers for the active day
4. Add `[JSInvokable] OnDayChanged(int dayNumber)` method
5. In `OnAfterRenderAsync(firstRender: true)`, call `trip-map.js` → `observeDaySections`

No new bUnit tests for the page itself (the component composition is tested by child component tests). Verify manually by running the app.

---

## Running Tests

```bash
dotnet test CitytripPlanner/CitytripPlanner.Tests/CitytripPlanner.Tests.csproj
```

All tests must pass before opening a pull request.

---

## Running the App

```bash
# Set your Google Maps API key
export GoogleMaps__ApiKey="YOUR_KEY_HERE"

dotnet run --project CitytripPlanner/CitytripPlanner.Web/CitytripPlanner.Web.csproj
```

Navigate to `http://localhost:5000/citytrips/1` to see the Paris trip detail page with the day schedule and sticky map sidebar.

---

## Key Files to Create / Modify

| File | Action | Layer |
|------|--------|-------|
| `Features/Citytrips/Domain/Place.cs` | Create | Features |
| `Features/Citytrips/Domain/ScheduledEvent.cs` | Create | Features |
| `Features/Citytrips/Domain/DayPlan.cs` | Extend (`Events` param) | Features |
| `Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs` | Extend (2 new records) | Features |
| `Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs` | Extend projection | Features |
| `Infrastructure/Citytrips/InMemoryCitytripRepository.cs` | Extend seed data | Infrastructure |
| `Web/Components/Citytrips/ScheduledEventCard.razor` + `.css` | Create | Web |
| `Web/Components/Citytrips/DaySchedulePanel.razor` + `.css` | Create | Web |
| `Web/Components/Citytrips/TripMapSidebar.razor` + `.css` | Create | Web |
| `Web/Components/Pages/CitytripDetail.razor` | Update (two-column layout) | Web |
| `Web/Components/Pages/CitytripDetail.razor.css` | Update (sticky sidebar CSS) | Web |
| `Web/wwwroot/js/trip-map.js` | Create | Web |
| `Tests/Citytrips/Domain/PlaceTests.cs` | Create | Tests |
| `Tests/Citytrips/Domain/ScheduledEventTests.cs` | Create | Tests |
| `Tests/Citytrips/GetCitytripDetail/GetCitytripDetailTests.cs` | Extend | Tests |
| `Tests/Web/Citytrips/ScheduledEventCardTests.cs` | Create | Tests |
| `Tests/Web/Citytrips/TripMapSidebarTests.cs` | Create | Tests |
