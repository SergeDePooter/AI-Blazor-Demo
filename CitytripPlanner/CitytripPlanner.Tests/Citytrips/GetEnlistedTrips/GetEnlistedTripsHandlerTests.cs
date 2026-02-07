using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.GetEnlistedTrips;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.GetEnlistedTrips;

public class GetEnlistedTripsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyEnlistedTripsNotCreatedByUser()
    {
        var repository = new InMemoryCitytripRepository();
        // Seed data is created by "seed-user"
        var store = new InMemoryUserInteractionStore();
        store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsEnlisted = true
        });
        store.SaveInteraction(2, new UserTripInteraction
        {
            CitytripId = 2, IsEnlisted = true
        });

        var handler = new GetEnlistedTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetEnlistedTripsQuery("demo-user"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.NotEqual("demo-user", t.CreatorName));
    }

    [Fact]
    public async Task Handle_ExcludesTripsCreatedByUser()
    {
        var repository = new InMemoryCitytripRepository();
        var myTrip = await repository.AddAsync(new Citytrip(0, "My Trip", "Berlin", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "demo-user"));
        var store = new InMemoryUserInteractionStore();
        store.SaveInteraction(myTrip.Id, new UserTripInteraction
        {
            CitytripId = myTrip.Id, IsEnlisted = true
        });

        var handler = new GetEnlistedTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetEnlistedTripsQuery("demo-user"), CancellationToken.None);

        Assert.DoesNotContain(result, t => t.Title == "My Trip");
    }

    [Fact]
    public async Task Handle_ReturnsEmptyListWhenNoEnlistedTrips()
    {
        var repository = new InMemoryCitytripRepository();
        var store = new InMemoryUserInteractionStore();

        var handler = new GetEnlistedTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetEnlistedTripsQuery("demo-user"), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_MapsAllFieldsCorrectly()
    {
        var repository = new InMemoryCitytripRepository();
        var store = new InMemoryUserInteractionStore();
        store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsEnlisted = true
        });

        var handler = new GetEnlistedTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetEnlistedTripsQuery("demo-user"), CancellationToken.None);

        var paris = result.First(t => t.Id == 1);
        Assert.Equal("Paris", paris.Title);
        Assert.Equal("Paris", paris.Destination);
        Assert.Equal("seed-user", paris.CreatorName);
        Assert.NotNull(paris.ImageUrl);
    }
}
