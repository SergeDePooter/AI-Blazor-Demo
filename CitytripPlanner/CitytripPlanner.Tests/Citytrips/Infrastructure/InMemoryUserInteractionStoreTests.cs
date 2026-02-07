using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.Infrastructure;

public class InMemoryUserInteractionStoreTests
{
    private readonly InMemoryUserInteractionStore _store = new();

    [Fact]
    public void GetInteraction_ReturnsDefaultForNewId()
    {
        var interaction = _store.GetInteraction(1);

        Assert.NotNull(interaction);
        Assert.Equal(1, interaction.CitytripId);
        Assert.False(interaction.IsLiked);
        Assert.False(interaction.IsEnlisted);
    }

    [Fact]
    public void SaveInteraction_PersistsState()
    {
        var interaction = new UserTripInteraction
        {
            CitytripId = 1,
            IsLiked = true,
            IsEnlisted = false
        };

        _store.SaveInteraction(1, interaction);
        var retrieved = _store.GetInteraction(1);

        Assert.True(retrieved.IsLiked);
        Assert.False(retrieved.IsEnlisted);
    }

    [Fact]
    public void GetAllInteractions_ReturnsAllSaved()
    {
        _store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsLiked = true, IsEnlisted = false
        });
        _store.SaveInteraction(2, new UserTripInteraction
        {
            CitytripId = 2, IsLiked = false, IsEnlisted = true
        });

        var all = _store.GetAllInteractions();

        Assert.Equal(2, all.Count);
        Assert.True(all[1].IsLiked);
        Assert.True(all[2].IsEnlisted);
    }
}
