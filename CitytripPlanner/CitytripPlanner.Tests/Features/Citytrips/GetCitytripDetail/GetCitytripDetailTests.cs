namespace CitytripPlanner.Tests.Features.Citytrips.GetCitytripDetail;

using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using FluentAssertions;
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
            Title: "Paris Adventure",
            Destination: "Paris, France",
            ImageUrl: "https://example.com/paris.jpg",
            StartDate: new DateOnly(2026, 6, 1),
            EndDate: new DateOnly(2026, 6, 3),
            CreatorId: "user123",
            Description: "Explore the City of Light",
            MaxParticipants: 10,
            DayPlans: new List<DayPlan>
            {
                new DayPlan(1, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>())
            }
        );
        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        var query = new GetCitytripDetailQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Paris Adventure");
        result.Destination.Should().Be("Paris, France");
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
    public async Task Handle_DayPlans_OrderedByDayNumber()
    {
        // Arrange
        var citytrip = new Citytrip(
            Id: 1,
            Title: "Test Trip",
            Destination: "Test",
            ImageUrl: "url",
            StartDate: new DateOnly(2026, 6, 1),
            EndDate: new DateOnly(2026, 6, 3),
            CreatorId: "user",
            DayPlans: new List<DayPlan>
            {
                new DayPlan(3, new DateOnly(2026, 6, 3), "Day 3", new List<Attraction>()),
                new DayPlan(1, new DateOnly(2026, 6, 1), "Day 1", new List<Attraction>()),
                new DayPlan(2, new DateOnly(2026, 6, 2), "Day 2", new List<Attraction>())
            }
        );
        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        var query = new GetCitytripDetailQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DayPlans.Select(d => d.DayNumber).Should().BeInAscendingOrder();
        result.DayPlans.Select(d => d.DayNumber).Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task Handle_NoDayPlans_ReturnsEmptyList()
    {
        // Arrange
        var citytrip = new Citytrip(
            Id: 1,
            Title: "No Itinerary Trip",
            Destination: "Test",
            ImageUrl: "url",
            StartDate: new DateOnly(2026, 6, 1),
            EndDate: new DateOnly(2026, 6, 3),
            CreatorId: "user",
            DayPlans: null
        );
        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        var query = new GetCitytripDetailQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DayPlans.Should().BeEmpty();
    }

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

        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        // Act
        var result = await _handler.Handle(new GetCitytripDetailQuery(1), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var day = result!.DayPlans!.First();
        day.Events.Should().HaveCount(1);
        day.Events[0].EventType.Should().Be("Landmark");
        day.Events[0].Place.Should().NotBeNull();
        day.Events[0].Place!.Name.Should().Be("Eiffel Tower");
        day.Events[0].Place!.Latitude.Should().Be(48.8584);
    }

    [Fact]
    public async Task Handle_TripWithEvents_OrderedByStartTime()
    {
        // Arrange
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

        _repository.GetByIdWithItineraryAsync(1).Returns(citytrip);

        // Act
        var result = await _handler.Handle(new GetCitytripDetailQuery(1), CancellationToken.None);

        // Assert
        var day = result!.DayPlans!.First();
        day.Events[0].Name.Should().Be("Early Museum");
        day.Events[1].Name.Should().Be("Late Market");
    }

    [Fact]
    public async Task Handle_ValidId_MapsAllProperties()
    {
        // Arrange
        var citytrip = new Citytrip(
            Id: 42,
            Title: "Barcelona",
            Destination: "Barcelona, Spain",
            ImageUrl: "https://example.com/bcn.jpg",
            StartDate: new DateOnly(2026, 7, 1),
            EndDate: new DateOnly(2026, 7, 5),
            CreatorId: "creator",
            Description: "Beach and architecture",
            MaxParticipants: 15,
            DayPlans: new List<DayPlan>()
        );
        _repository.GetByIdWithItineraryAsync(42).Returns(citytrip);

        var query = new GetCitytripDetailQuery(42);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Title.Should().Be("Barcelona");
        result.Destination.Should().Be("Barcelona, Spain");
        result.ImageUrl.Should().Be("https://example.com/bcn.jpg");
        result.StartDate.Should().Be(new DateOnly(2026, 7, 1));
        result.EndDate.Should().Be(new DateOnly(2026, 7, 5));
        result.Description.Should().Be("Beach and architecture");
        result.MaxParticipants.Should().Be(15);
    }
}
