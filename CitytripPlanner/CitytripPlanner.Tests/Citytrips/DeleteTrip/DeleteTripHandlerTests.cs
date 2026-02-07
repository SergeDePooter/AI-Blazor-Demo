using CitytripPlanner.Features.Citytrips.DeleteTrip;
using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.DeleteTrip;

public class DeleteTripHandlerTests
{
    [Fact]
    public async Task Handle_DeletesTripWithValidData()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "To Delete", "Berlin", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "user-1"));
        var handler = new DeleteTripHandler(repository);

        var result = await handler.Handle(
            new DeleteTripCommand(created.Id, "user-1"), CancellationToken.None);

        Assert.True(result);
        var deleted = await repository.GetByIdAsync(created.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_ThrowsForNonExistentTrip()
    {
        var repository = new InMemoryCitytripRepository();
        var handler = new DeleteTripHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(
                new DeleteTripCommand(999, "user-1"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenUserIdDoesNotMatchCreatorId()
    {
        var repository = new InMemoryCitytripRepository();
        var created = await repository.AddAsync(new Citytrip(0, "Trip", "City", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "owner"));
        var handler = new DeleteTripHandler(repository);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(
                new DeleteTripCommand(created.Id, "intruder"), CancellationToken.None));
    }
}
