using CitytripPlanner.Features.Citytrips.CreateTrip;
using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.UpdateTrip;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.UpdateTrip;

public class UpdateTripHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesTripWithValidData()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Original", "Berlin", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "user-1"));
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            created.Id, "Updated", "Munich",
            new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 5),
            "user-1", "New description", 20);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var updated = await repository.GetByIdAsync(created.Id);
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal("Munich", updated.Destination);
        Assert.Equal(20, updated.MaxParticipants);
    }

    [Fact]
    public async Task Handle_ThrowsForNonExistentTrip()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            999, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3),
            "user-1");

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenUserIdDoesNotMatchCreatorId()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "owner"));
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            created.Id, "Updated", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3),
            "intruder");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UpdatesTripWithValidImageUrl()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "old-url.jpg",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "user-1"));
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            created.Id, "Trip", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3),
            "user-1", ImageUrl: "https://example.com/new.jpg");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var updated = await repository.GetByIdAsync(created.Id);
        Assert.Equal("https://example.com/new.jpg", updated!.ImageUrl);
    }

    [Fact]
    public async Task Handle_PreservesExistingImageUrl_WhenImageUrlIsNull()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "https://example.com/existing.jpg",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "user-1"));
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            created.Id, "Trip", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3),
            "user-1", ImageUrl: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var updated = await repository.GetByIdAsync(created.Id);
        Assert.Equal("https://example.com/existing.jpg", updated!.ImageUrl);
    }

    [Fact]
    public async Task Handle_UpdatesDayPlansWhenProvided()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1), "user-1"));
        var handler = new UpdateTripHandler(repository);

        var dayPlans = new List<DayPlanInput>
        {
            new DayPlanInput(1, new DateOnly(2026, 8, 1), new List<ScheduledEventInput>
            {
                new ScheduledEventInput("museum", "Louvre", new TimeOnly(10, 0))
            })
        };
        var command = new UpdateTripCommand(
            created.Id, "Trip", "City",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1),
            "user-1", DayPlans: dayPlans);

        await handler.Handle(command, CancellationToken.None);

        var updated = await repository.GetByIdAsync(created.Id);
        Assert.NotNull(updated!.DayPlans);
        Assert.Single(updated.DayPlans);
        Assert.Equal("Louvre", updated.DayPlans[0].Events[0].Name);
    }

    [Fact]
    public async Task Handle_PreservesExistingDayPlans_WhenDayPlansIsNull()
    {
        var repository = new InMemoryCitytripRepository();
        var existingDayPlans = new List<DayPlan>
        {
            new DayPlan(1, new DateOnly(2026, 8, 1), "", new List<Attraction>(),
                new List<ScheduledEvent> { new ScheduledEvent("museum", "Louvre", new TimeOnly(10, 0)) })
        };
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1), "user-1",
            DayPlans: existingDayPlans));
        var handler = new UpdateTripHandler(repository);

        var command = new UpdateTripCommand(
            created.Id, "Trip", "City",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1),
            "user-1", DayPlans: null);

        await handler.Handle(command, CancellationToken.None);

        var updated = await repository.GetByIdAsync(created.Id);
        Assert.NotNull(updated!.DayPlans);
        Assert.Single(updated.DayPlans);
        Assert.Equal("Louvre", updated.DayPlans[0].Events[0].Name);
    }

    [Fact]
    public async Task Handle_UpdatesDayPlansWithPlace()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1), "user-1"));
        var handler = new UpdateTripHandler(repository);

        var dayPlans = new List<DayPlanInput>
        {
            new DayPlanInput(1, new DateOnly(2026, 8, 1), new List<ScheduledEventInput>
            {
                new ScheduledEventInput("museum", "Louvre", new TimeOnly(10, 0),
                    Place: new PlaceInput("Louvre Museum", 48.8606, 2.3376))
            })
        };
        var command = new UpdateTripCommand(
            created.Id, "Trip", "City",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 1),
            "user-1", DayPlans: dayPlans);

        await handler.Handle(command, CancellationToken.None);

        var updated = await repository.GetByIdAsync(created.Id);
        Assert.Equal("Louvre Museum", updated!.DayPlans![0].Events[0].Place!.Name);
        Assert.Equal(48.8606, updated.DayPlans[0].Events[0].Place.Latitude);
    }
}
