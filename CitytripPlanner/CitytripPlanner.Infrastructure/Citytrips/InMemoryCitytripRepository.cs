using System.Collections.Concurrent;
using CitytripPlanner.Features.Citytrips.Domain;

namespace CitytripPlanner.Infrastructure.Citytrips;

public class InMemoryCitytripRepository : ICitytripRepository
{
    private readonly ConcurrentDictionary<int, Citytrip> _trips = new();
    private int _nextId;

    public InMemoryCitytripRepository()
    {
        var seedData = new List<Citytrip>
        {
            new(1, "Paris", "Paris", "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=600",
                new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5), "seed-user", "The City of Light awaits"),
            new(2, "Barcelona", "Barcelona", "https://images.unsplash.com/photo-1583422409516-2895a77efded?w=600",
                new DateOnly(2026, 7, 10), new DateOnly(2026, 7, 13), "seed-user", "Gaudi's masterpiece city"),
            new(3, "Rome", "Rome", "https://images.unsplash.com/photo-1552832230-c0197dd311b5?w=600",
                new DateOnly(2026, 5, 15), new DateOnly(2026, 5, 17), "seed-user", "The Eternal City"),
            new(4, "Amsterdam", "Amsterdam", "https://images.unsplash.com/photo-1534351590666-13e3e96b5017?w=600",
                new DateOnly(2026, 8, 20), new DateOnly(2026, 8, 21), "seed-user", "Canals and culture"),
            new(5, "Prague", "Prague", "https://images.unsplash.com/photo-1519677100203-a0e668c92439?w=600",
                new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 8), "seed-user", "The Golden City"),
            new(6, "Lisbon", "Lisbon", "https://images.unsplash.com/photo-1585208798174-6cedd86e019a?w=600",
                new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 5), "seed-user", "Hills, trams and pastéis")
        };

        foreach (var trip in seedData)
            _trips[trip.Id] = trip;

        _nextId = seedData.Max(t => t.Id);
    }

    public Task<List<Citytrip>> GetAllAsync()
        => Task.FromResult(_trips.Values.ToList());

    public Task<Citytrip?> GetByIdAsync(int id)
        => Task.FromResult(_trips.GetValueOrDefault(id));

    public Task<Citytrip> AddAsync(Citytrip trip)
    {
        var id = Interlocked.Increment(ref _nextId);
        var created = trip with { Id = id };
        _trips[id] = created;
        return Task.FromResult(created);
    }

    public Task<Citytrip> UpdateAsync(Citytrip trip)
    {
        if (!_trips.ContainsKey(trip.Id))
            throw new ArgumentException($"Citytrip {trip.Id} not found.");

        _trips[trip.Id] = trip;
        return Task.FromResult(trip);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_trips.TryRemove(id, out _));
    }

    public Task<List<Citytrip>> GetByCreatorAsync(string creatorId)
        => Task.FromResult(_trips.Values.Where(t => t.CreatorId == creatorId).ToList());
}
