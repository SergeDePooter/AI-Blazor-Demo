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
}
