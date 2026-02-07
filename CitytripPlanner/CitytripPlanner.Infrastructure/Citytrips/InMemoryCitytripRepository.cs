using CitytripPlanner.Features.Citytrips.Domain;

namespace CitytripPlanner.Infrastructure.Citytrips;

public class InMemoryCitytripRepository : ICitytripRepository
{
    private static readonly List<Citytrip> SeedData =
    [
        new(1, "Paris", "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=600", 5, "The City of Light awaits"),
        new(2, "Barcelona", "https://images.unsplash.com/photo-1583422409516-2895a77efded?w=600", 4, "Gaudi's masterpiece city"),
        new(3, "Rome", "https://images.unsplash.com/photo-1552832230-c0197dd311b5?w=600", 3, "The Eternal City"),
        new(4, "Amsterdam", "https://images.unsplash.com/photo-1534351590666-13e3e96b5017?w=600", 2, "Canals and culture"),
        new(5, "Prague", "https://images.unsplash.com/photo-1519677100203-a0e668c92439?w=600", 4, "The Golden City"),
        new(6, "Lisbon", "https://images.unsplash.com/photo-1585208798174-6cedd86e019a?w=600", 5, "Hills, trams and pastéis")
    ];

    public Task<List<Citytrip>> GetAllAsync()
        => Task.FromResult(SeedData.ToList());

    public Task<Citytrip?> GetByIdAsync(int id)
        => Task.FromResult(SeedData.FirstOrDefault(t => t.Id == id));
}
