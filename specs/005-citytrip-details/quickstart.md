# Quickstart: Citytrip Detail View Implementation

**Feature**: 005-citytrip-details
**Date**: 2026-02-14
**Prerequisites**: .NET 10 SDK, MediatR 14.x, bUnit 2.5.3
**Estimated Time**: 3-4 hours (following TDD strictly)

## Overview

This guide walks through implementing the citytrip detail view feature using Test-Driven Development (TDD). Each step follows the **Red-Green-Refactor** cycle mandated by the project constitution.

## Architecture Summary

```
User clicks citytrip card
    ↓
Blazor Router navigates to /citytrips/{id}
    ↓
CitytripDetail.razor page loads
    ↓
MediatR sends GetCitytripDetailQuery
    ↓
GetCitytripDetailHandler queries repository
    ↓
Response mapped to DTOs
    ↓
Blazor components render detail view
```

## Implementation Steps

### Phase 0: Setup (5 minutes)

1. **Ensure on correct branch**:
   ```bash
   git checkout 005-citytrip-details
   git pull origin 005-citytrip-details
   ```

2. **Verify build**:
   ```bash
   dotnet build CitytripPlanner/CitytripPlanner.sln
   ```

3. **Run existing tests** (should all pass):
   ```bash
   dotnet test CitytripPlanner/CitytripPlanner.Tests
   ```

---

### Phase 1: Domain Models (TDD - 30 minutes)

#### Step 1.1: DayPlan Domain Model

**RED - Write failing test**:

Create: `CitytripPlanner.Tests/Features/Citytrips/Domain/DayPlanTests.cs`

```csharp
namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;

public class DayPlanTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesDayPlan()
    {
        // Arrange
        var dayNumber = 1;
        var date = new DateOnly(2026, 6, 1);
        var timeframe = "Morning 9:00-12:00";
        var attractions = new List<Attraction>();

        // Act
        var dayPlan = new DayPlan(dayNumber, date, timeframe, attractions);

        // Assert
        dayPlan.DayNumber.Should().Be(dayNumber);
        dayPlan.Date.Should().Be(date);
        dayPlan.Timeframe.Should().Be(timeframe);
        dayPlan.Attractions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_NegativeDayNumber_ThrowsArgumentException()
    {
        // Arrange
        var invalidDayNumber = -1;

        // Act & Assert
        var act = () => new DayPlan(invalidDayNumber, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>());
        act.Should().Throw<ArgumentException>().WithMessage("*DayNumber*");
    }

    [Fact]
    public void Constructor_NullAttractions_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new DayPlan(1, new DateOnly(2026, 6, 1), "Morning", null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("Attractions");
    }
}
```

**Run test**: `dotnet test` - should FAIL (DayPlan doesn't exist yet)

**GREEN - Implement minimum code**:

Create: `CitytripPlanner.Features/Citytrips/Domain/DayPlan.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

public record DayPlan(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<Attraction> Attractions)
{
    public DayPlan
    {
        if (DayNumber <= 0)
            throw new ArgumentException("DayNumber must be positive", nameof(DayNumber));
        if (Attractions == null)
            throw new ArgumentNullException(nameof(Attractions));
    }
}
```

**Run test**: `dotnet test` - should PASS

**REFACTOR**: No refactoring needed (simple record)

---

#### Step 1.2: Attraction Domain Model

**RED - Write failing test**:

Create: `CitytripPlanner.Tests/Features/Citytrips/Domain/AttractionTests.cs`

```csharp
namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;

public class AttractionTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesAttraction()
    {
        // Arrange & Act
        var attraction = new Attraction(
            "Eiffel Tower",
            "Iconic landmark",
            "https://www.toureiffel.paris/en",
            new List<string> { "Metro Line 6" }
        );

        // Assert
        attraction.Name.Should().Be("Eiffel Tower");
        attraction.WebsiteUrl.Should().Be("https://www.toureiffel.paris/en");
        attraction.TransportationOptions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Act & Assert
        var act = () => new Attraction(invalidName!, "Desc", null, new List<string>());
        act.Should().Throw<ArgumentException>().WithMessage("*Name*");
    }

    [Fact]
    public void Constructor_InvalidWebsiteUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new Attraction("Name", "Desc", "not-a-url", new List<string>());
        act.Should().Throw<ArgumentException>().WithMessage("*WebsiteUrl*");
    }
}
```

**Run test**: Should FAIL

**GREEN - Implement**:

Create: `CitytripPlanner.Features/Citytrips/Domain/Attraction.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.Domain;

public record Attraction(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions)
{
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

**Run test**: Should PASS

---

#### Step 1.3: Extend Citytrip Domain Model

**Update**: `CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs`

Add `DayPlans` property:

```csharp
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
    List<DayPlan>? DayPlans = null);  // NEW
```

**Test**: No new tests needed (backward compatible change)

---

### Phase 2: Repository Extension (TDD - 20 minutes)

#### Step 2.1: Update Repository Interface

**Update**: `CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs`

```csharp
Task<Citytrip?> GetByIdWithItineraryAsync(int id);
```

#### Step 2.2: Implement Repository Method

**Update**: `CitytripPlanner.Infrastructure/Persistence/InMemoryCitytripRepository.cs`

**First, add test**:

Create: `CitytripPlanner.Tests/Infrastructure/InMemoryCitytripRepositoryTests.cs`

```csharp
[Fact]
public async Task GetByIdWithItineraryAsync_ExistingId_ReturnsItinerary()
{
    // Arrange
    var repository = new InMemoryCitytripRepository();

    // Act
    var citytrip = await repository.GetByIdWithItineraryAsync(1);

    // Assert
    citytrip.Should().NotBeNull();
    citytrip!.DayPlans.Should().NotBeNull();
    // Add specific assertions based on seed data
}
```

**Then implement**:

```csharp
public Task<Citytrip?> GetByIdWithItineraryAsync(int id)
{
    var citytrip = _citytrips.FirstOrDefault(c => c.Id == id);
    return Task.FromResult(citytrip);
}
```

#### Step 2.3: Add Seed Data with Itinerary

Update `InMemoryCitytripRepository` constructor to add sample itinerary (see `data-model.md` for example).

---

### Phase 3: MediatR Query & Handler (TDD - 40 minutes)

#### Step 3.1: Create DTOs

Create: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/CitytripDetailResponse.cs`

(Copy from `contracts/GetCitytripDetailContract.md`)

No tests needed for DTOs (they're just data containers).

---

#### Step 3.2: Create Query

Create: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailQuery.cs`

```csharp
namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

using MediatR;

public record GetCitytripDetailQuery(int CitytripId) : IRequest<CitytripDetailResponse?>;
```

---

#### Step 3.3: Create Handler (TDD)

**RED - Write failing test**:

Create: `CitytripPlanner.Tests/Features/Citytrips/GetCitytripDetailTests.cs`

```csharp
namespace CitytripPlanner.Tests.Features.Citytrips;

using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using NSubstitute;

public class GetCitytripDetailTests
{
    private readonly ICitytripRepository _repository;
    private readonly GetCitytripDetailHandler _handler;

    public GetCitytripDetailTests()
    {
        _repository = Substitute.For<ICitytripRepository>();
        _handler = new GetCitytripDetailHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsDetailResponse()
    {
        // Arrange
        var citytrip = new Citytrip(
            Id: 1,
            Title: "Paris",
            Destination: "France",
            ImageUrl: "url",
            StartDate: new DateOnly(2026, 6, 1),
            EndDate: new DateOnly(2026, 6, 3),
            CreatorId: "user",
            DayPlans: new List<DayPlan>
            {
                new(1, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>())
            }
        );
        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        var query = new GetCitytripDetailQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Paris");
        result.DayPlans.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNull()
    {
        // Arrange
        _repository.GetByIdWithItineraryAsync(999).Returns((Citytrip?)null);
        var query = new GetCitytripDetailQuery(999);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DayPlansOrderedByDayNumber()
    {
        // Arrange
        var citytrip = new Citytrip(
            Id: 1, Title: "Test", Destination: "Test", ImageUrl: "url",
            StartDate: new DateOnly(2026, 6, 1), EndDate: new DateOnly(2026, 6, 3),
            CreatorId: "user",
            DayPlans: new List<DayPlan>
            {
                new(3, new DateOnly(2026, 6, 3), "Day 3", new List<Attraction>()),
                new(1, new DateOnly(2026, 6, 1), "Day 1", new List<Attraction>()),
                new(2, new DateOnly(2026, 6, 2), "Day 2", new List<Attraction>())
            }
        );
        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        // Act
        var result = await _handler.Handle(new GetCitytripDetailQuery(1), CancellationToken.None);

        // Assert
        result!.DayPlans.Select(d => d.DayNumber).Should().BeInAscendingOrder();
    }
}
```

**Run test**: Should FAIL

**GREEN - Implement handler**:

Create: `CitytripPlanner.Features/Citytrips/GetCitytripDetail/GetCitytripDetailHandler.cs`

(Copy from `contracts/GetCitytripDetailContract.md`)

**Run test**: Should PASS

---

### Phase 4: Blazor UI (TDD with bUnit - 60 minutes)

#### Step 4.1: Create Detail Page (TDD)

**RED - Write failing bUnit test**:

Create: `CitytripPlanner.Tests/Web/Pages/CitytripDetailPageTests.cs`

```csharp
namespace CitytripPlanner.Tests.Web.Pages;

using Bunit;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using CitytripPlanner.Web.Pages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

public class CitytripDetailPageTests : TestContext
{
    private readonly IMediator _mediator;

    public CitytripDetailPageTests()
    {
        _mediator = Substitute.For<IMediator>();
        Services.AddSingleton(_mediator);
    }

    [Fact]
    public void Page_ValidId_DisplaysCitytripTitle()
    {
        // Arrange
        var response = new CitytripDetailResponse(
            Id: 1,
            Title: "Paris Adventure",
            Destination: "France",
            ImageUrl: "url",
            StartDate: new DateOnly(2026, 6, 1),
            EndDate: new DateOnly(2026, 6, 3),
            Description: "Test",
            MaxParticipants: 10,
            DayPlans: new List<DayPlanDto>()
        );
        _mediator.Send(Arg.Any<GetCitytripDetailQuery>()).Returns(response);

        // Act
        var cut = RenderComponent<CitytripDetail>(parameters => parameters
            .Add(p => p.Id, 1));

        // Assert
        cut.Find("h1").TextContent.Should().Contain("Paris Adventure");
    }

    [Fact]
    public void Page_NonExistentId_DisplaysNotFoundMessage()
    {
        // Arrange
        _mediator.Send(Arg.Any<GetCitytripDetailQuery>()).Returns((CitytripDetailResponse?)null);

        // Act
        var cut = RenderComponent<CitytripDetail>(parameters => parameters
            .Add(p => p.Id, 999));

        // Assert
        cut.Markup.Should().Contain("not found");
    }
}
```

**Run test**: Should FAIL

**GREEN - Implement page**:

Create: `CitytripPlanner.Web/Pages/CitytripDetail.razor`

```razor
@page "/citytrips/{Id:int}"
@inject IMediator Mediator

<PageTitle>@(_citytrip?.Title ?? "Citytrip Details")</PageTitle>

@if (_loading)
{
    <p>Loading...</p>
}
else if (_citytrip is null)
{
    <p>Citytrip not found.</p>
}
else
{
    <div class="citytrip-detail">
        <h1>@_citytrip.Title</h1>
        <p class="destination">@_citytrip.Destination</p>
        <img src="@_citytrip.ImageUrl" alt="@_citytrip.Title" />

        <div class="dates">
            <span>@_citytrip.StartDate.ToString("d") - @_citytrip.EndDate.ToString("d")</span>
        </div>

        @if (!string.IsNullOrEmpty(_citytrip.Description))
        {
            <p class="description">@_citytrip.Description</p>
        }

        <h2>Itinerary</h2>
        @if (_citytrip.DayPlans.Any())
        {
            @foreach (var day in _citytrip.DayPlans)
            {
                <DayPlanCard Day="@day" />
            }
        }
        else
        {
            <p class="no-itinerary">No itinerary available yet.</p>
        }
    </div>
}

@code {
    [Parameter]
    public int Id { get; set; }

    private CitytripDetailResponse? _citytrip;
    private bool _loading = true;

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        var query = new GetCitytripDetailQuery(Id);
        _citytrip = await Mediator.Send(query);
        _loading = false;
    }
}
```

**Run test**: Should PASS

---

#### Step 4.2: Create Child Components

Follow same TDD pattern for:

1. **DayPlanCard.razor**: Displays day header + attractions
2. **AttractionItem.razor**: Displays attraction details + transportation
3. **TransportationInfo.razor**: Displays transportation options list

(See `plan.md` project structure for file locations)

---

#### Step 4.3: Add Scoped CSS

Create: `CitytripPlanner.Web/Pages/CitytripDetail.razor.css`

```css
.citytrip-detail {
    max-width: 800px;
    margin: 0 auto;
    padding: 2rem;
}

.citytrip-detail img {
    width: 100%;
    height: 400px;
    object-fit: cover;
    border-radius: 8px;
}

.destination {
    color: #666;
    font-size: 1.2rem;
}

.no-itinerary {
    color: #999;
    font-style: italic;
}
```

---

### Phase 5: Integration & Manual Testing (20 minutes)

1. **Update navigation** (optional):
   - Modify `ListCitytrips` component to make cards clickable
   - Add `<a href="/citytrips/@trip.Id">` around CitytripCard

2. **Run application**:
   ```bash
   dotnet run --project CitytripPlanner/CitytripPlanner.Web
   ```

3. **Manual tests**:
   - Navigate to `/citytrips/1` (should show detail with itinerary)
   - Navigate to `/citytrips/999` (should show "not found")
   - Click back button (should work via browser navigation)
   - Check external links open in new tab

4. **Verify all tests pass**:
   ```bash
   dotnet test CitytripPlanner/CitytripPlanner.Tests
   ```

---

## TDD Checklist

- [ ] All tests written BEFORE implementation
- [ ] Each test fails initially (RED phase verified)
- [ ] Minimum code written to pass tests (GREEN phase)
- [ ] Code refactored for clarity (REFACTOR phase)
- [ ] No production code without corresponding test
- [ ] All tests passing before moving to next step

---

## Common Pitfalls

1. **Writing implementation before tests**: STOP. Write the test first.
2. **Over-engineering**: Implement only what's needed to pass current test.
3. **Skipping refactor**: Clean up duplication after tests pass.
4. **Missing edge cases**: Test null responses, empty lists, invalid IDs.
5. **Forgetting to register services**: Ensure MediatR handlers auto-registered.

---

## Troubleshooting

### Tests fail with "Handler not found"
- Check MediatR registration in `Program.cs`: `services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetCitytripDetailHandler>());`

### Blazor page shows white screen
- Check browser console for errors
- Verify route parameter syntax: `{Id:int}` (capital I, colon int)
- Ensure `@inject IMediator Mediator` directive present

### bUnit tests fail with DI errors
- Mock `IMediator` with NSubstitute
- Add mocked services to `Services` collection in test constructor

---

## Next Steps

After implementation complete:

1. Run `/speckit.tasks` to generate detailed task breakdown
2. Implement tasks in priority order (P1 → P2 → P3 → P4)
3. Create pull request when all tests pass
4. Request code review focusing on TDD adherence

---

## Resources

- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)
- [bUnit Documentation](https://bunit.dev/)
- [Blazor Routing](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/routing)
- [C# Records](https://learn.microsoft.com/dotnet/csharp/fundamentals/types/records)
