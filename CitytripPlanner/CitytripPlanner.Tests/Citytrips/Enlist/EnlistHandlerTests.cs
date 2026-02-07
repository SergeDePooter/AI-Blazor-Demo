using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.Enlist;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.Enlist;

public class EnlistHandlerTests
{
    private readonly InMemoryCitytripRepository _repository = new();
    private readonly InMemoryUserInteractionStore _store = new();

    [Fact]
    public async Task Handle_SetsIsEnlistedToTrue()
    {
        var handler = new EnlistHandler(_repository, _store);

        var result = await handler.Handle(
            new EnlistCommand(1), CancellationToken.None);

        Assert.True(result);
        Assert.True(_store.GetInteraction(1).IsEnlisted);
    }

    [Fact]
    public async Task Handle_ReturnsTrueWhenAlreadyEnlisted()
    {
        _store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsEnlisted = true
        });
        var handler = new EnlistHandler(_repository, _store);

        var result = await handler.Handle(
            new EnlistCommand(1), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ThrowsForNonExistentCitytripId()
    {
        var handler = new EnlistHandler(_repository, _store);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(
                new EnlistCommand(999), CancellationToken.None));
    }
}
