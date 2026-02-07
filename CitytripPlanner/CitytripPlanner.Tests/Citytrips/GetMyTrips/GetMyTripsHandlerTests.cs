using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.GetMyTrips;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.GetMyTrips;

public class GetMyTripsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyTripsMatchingCreatorId()
    {
        var repository = new InMemoryCitytripRepository();
        await repository.AddAsync(new Citytrip(0, "My Trip", "Berlin", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "user-a"));
        await repository.AddAsync(new Citytrip(0, "Other Trip", "Vienna", "",
            new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3), "user-b"));
        var store = new InMemoryUserInteractionStore();

        var handler = new GetMyTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetMyTripsQuery("user-a"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("My Trip", result[0].Title);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyListWhenNoTrips()
    {
        var repository = new InMemoryCitytripRepository();
        var store = new InMemoryUserInteractionStore();

        var handler = new GetMyTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetMyTripsQuery("nobody"), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_MapsAllFieldsCorrectly()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Test", "Dest", "img.jpg",
            new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 5), "user-x",
            "A description", 10));
        var store = new InMemoryUserInteractionStore();

        var handler = new GetMyTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetMyTripsQuery("user-x"), CancellationToken.None);

        var item = Assert.Single(result);
        Assert.Equal(created.Id, item.Id);
        Assert.Equal("Test", item.Title);
        Assert.Equal("Dest", item.Destination);
        Assert.Equal("img.jpg", item.ImageUrl);
        Assert.Equal(new DateOnly(2026, 3, 1), item.StartDate);
        Assert.Equal(new DateOnly(2026, 3, 5), item.EndDate);
        Assert.Equal("A description", item.Description);
        Assert.Equal(10, item.MaxParticipants);
    }

    [Fact]
    public async Task Handle_IncludesEnlistedCount()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Popular", "Paris", "",
            new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 5), "creator"));
        var store = new InMemoryUserInteractionStore();
        store.SaveInteraction(created.Id, new UserTripInteraction
        {
            CitytripId = created.Id, IsEnlisted = true
        });

        var handler = new GetMyTripsHandler(repository, store);
        var result = await handler.Handle(
            new GetMyTripsQuery("creator"), CancellationToken.None);

        Assert.Equal(1, result[0].EnlistedCount);
    }
}
