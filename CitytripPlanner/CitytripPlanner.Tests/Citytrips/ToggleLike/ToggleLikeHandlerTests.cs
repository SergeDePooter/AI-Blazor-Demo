using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.ToggleLike;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.ToggleLike;

public class ToggleLikeHandlerTests
{
    private readonly InMemoryCitytripRepository _repository = new();
    private readonly InMemoryUserInteractionStore _store = new();

    [Fact]
    public async Task Handle_TogglesIsLikedFromFalseToTrue()
    {
        var handler = new ToggleLikeHandler(_repository, _store);

        var result = await handler.Handle(
            new ToggleLikeCommand(1), CancellationToken.None);

        Assert.True(result);
        Assert.True(_store.GetInteraction(1).IsLiked);
    }

    [Fact]
    public async Task Handle_TogglesIsLikedFromTrueToFalse()
    {
        _store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsLiked = true
        });
        var handler = new ToggleLikeHandler(_repository, _store);

        var result = await handler.Handle(
            new ToggleLikeCommand(1), CancellationToken.None);

        Assert.False(result);
        Assert.False(_store.GetInteraction(1).IsLiked);
    }

    [Fact]
    public async Task Handle_ThrowsForNonExistentCitytripId()
    {
        var handler = new ToggleLikeHandler(_repository, _store);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(
                new ToggleLikeCommand(999), CancellationToken.None));
    }
}
