using CitytripPlanner.Features.Citytrips.CreateTrip;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.CreateTrip;

public class CreateTripHandlerTests
{
    [Fact]
    public async Task Handle_CreatesTrip_ReturnsNewId()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var command = new CreateTripCommand(
            "Weekend in Berlin", "Berlin",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 3),
            "user-1", "Fun trip", 5);

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.True(id > 0);
        var created = await repository.GetByIdAsync(id);
        Assert.NotNull(created);
        Assert.Equal("Weekend in Berlin", created.Title);
        Assert.Equal("Berlin", created.Destination);
        Assert.Equal("user-1", created.CreatorId);
        Assert.Equal(5, created.MaxParticipants);
    }

    [Fact]
    public async Task Handle_AssignsCreatorId()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var command = new CreateTripCommand(
            "Trip", "Place",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3),
            "specific-user");

        var id = await handler.Handle(command, CancellationToken.None);
        var created = await repository.GetByIdAsync(id);

        Assert.Equal("specific-user", created!.CreatorId);
    }

    [Fact]
    public async Task Handle_ThrowsOnInvalidInput()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var command = new CreateTripCommand(
            "", "",
            new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 1),
            "user-1");

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    // --- New tests for T010 ---

    [Fact]
    public async Task Handle_SavesImageUrl()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var command = new CreateTripCommand(
            "Tokyo Trip", "Tokyo",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3),
            "user-1", ImageUrl: "https://example.com/tokyo.jpg");

        var id = await handler.Handle(command, CancellationToken.None);
        var created = await repository.GetByIdAsync(id);

        Assert.Equal("https://example.com/tokyo.jpg", created!.ImageUrl);
    }

    [Fact]
    public async Task Handle_SavesDayPlanWithEventsOrderedByStartTime()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 9, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("restaurant", "Lunch", new TimeOnly(12, 0)),
            new ScheduledEventInput("museum", "Morning Visit", new TimeOnly(9, 0))
        });

        var command = new CreateTripCommand(
            "Tokyo Trip", "Tokyo",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var id = await handler.Handle(command, CancellationToken.None);
        var created = await repository.GetByIdAsync(id);

        Assert.NotNull(created!.DayPlans);
        Assert.Single(created.DayPlans!);
        var events = created.DayPlans![0].Events;
        Assert.Equal(2, events.Count);
        Assert.Equal(new TimeOnly(9, 0), events[0].StartTime);
        Assert.Equal(new TimeOnly(12, 0), events[1].StartTime);
    }

    [Fact]
    public async Task Handle_SavesEventWithPlace()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new CreateTripHandler(repository);

        var place = new PlaceInput("Eiffel Tower", 48.8584, 2.2945);
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 9, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("landmark", "Tower Visit", new TimeOnly(10, 0), Place: place)
        });

        var command = new CreateTripCommand(
            "Paris Trip", "Paris",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var id = await handler.Handle(command, CancellationToken.None);
        var created = await repository.GetByIdAsync(id);

        var savedPlace = created!.DayPlans![0].Events[0].Place;
        Assert.NotNull(savedPlace);
        Assert.Equal("Eiffel Tower", savedPlace!.Name);
        Assert.Equal(48.8584, savedPlace.Latitude);
        Assert.Equal(2.2945, savedPlace.Longitude);
    }
}
