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
}
