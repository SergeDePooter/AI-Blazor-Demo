using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.ListCitytrips;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.ListCitytrips;

public class ListCitytripsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllTripsMergedWithInteractions()
    {
        var repository = new InMemoryCitytripRepository();
        var store = new InMemoryUserInteractionStore();
        store.SaveInteraction(1, new UserTripInteraction
        {
            CitytripId = 1, IsLiked = true, IsEnlisted = false
        });

        var handler = new ListCitytripsHandler(repository, store);
        var result = await handler.Handle(new ListCitytripsQuery(), CancellationToken.None);

        Assert.NotEmpty(result);
        var paris = result.First(c => c.Id == 1);
        Assert.True(paris.IsLiked);
        Assert.False(paris.IsEnlisted);

        var barcelona = result.First(c => c.Id == 2);
        Assert.False(barcelona.IsLiked);
        Assert.False(barcelona.IsEnlisted);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyListWhenNoTrips()
    {
        var repository = new EmptyCitytripRepository();
        var store = new InMemoryUserInteractionStore();

        var handler = new ListCitytripsHandler(repository, store);
        var result = await handler.Handle(new ListCitytripsQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    private class EmptyCitytripRepository : ICitytripRepository
    {
        public Task<List<Citytrip>> GetAllAsync() => Task.FromResult(new List<Citytrip>());
        public Task<Citytrip?> GetByIdAsync(int id) => Task.FromResult<Citytrip?>(null);
        public Task<Citytrip?> GetByIdWithItineraryAsync(int id) => Task.FromResult<Citytrip?>(null);
        public Task<Citytrip> AddAsync(Citytrip trip) => Task.FromResult(trip);
        public Task<Citytrip> UpdateAsync(Citytrip trip) => Task.FromResult(trip);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(false);
        public Task<List<Citytrip>> GetByCreatorAsync(string creatorId) => Task.FromResult(new List<Citytrip>());
    }
}
