using System.Collections.Concurrent;
using CitytripPlanner.Features.Citytrips.Domain;

namespace CitytripPlanner.Infrastructure.Citytrips;

public class InMemoryCitytripRepository : ICitytripRepository
{
    private readonly ConcurrentDictionary<int, Citytrip> _trips = new();
    private int _nextId;

    public InMemoryCitytripRepository()
    {
        // Sample itinerary for Paris trip
        var parisItinerary = new List<DayPlan>
        {
            new DayPlan(
                dayNumber: 1,
                date: new DateOnly(2026, 6, 1),
                timeframe: "Morning 9:00-12:00",
                attractions: new List<Attraction>
                {
                    new Attraction(
                        name: "Eiffel Tower",
                        description: "Iconic iron lattice tower with observation decks offering panoramic city views",
                        websiteUrl: "https://www.toureiffel.paris/en",
                        transportationOptions: new List<string>
                        {
                            "Metro Line 6 to Bir-Hakeim",
                            "RER C to Champ de Mars"
                        }
                    ),
                    new Attraction(
                        name: "Trocadéro Gardens",
                        description: "Beautiful gardens with stunning Eiffel Tower views",
                        websiteUrl: null,
                        transportationOptions: new List<string>
                        {
                            "Walking from Eiffel Tower (10 min)"
                        }
                    )
                }
            ),
            new DayPlan(
                dayNumber: 2,
                date: new DateOnly(2026, 6, 2),
                timeframe: "Afternoon 14:00-18:00",
                attractions: new List<Attraction>
                {
                    new Attraction(
                        name: "Louvre Museum",
                        description: "World's largest art museum, home to the Mona Lisa",
                        websiteUrl: "https://www.louvre.fr/en",
                        transportationOptions: new List<string>
                        {
                            "Metro Line 1 to Palais Royal - Musée du Louvre"
                        }
                    )
                }
            ),
            new DayPlan(
                dayNumber: 3,
                date: new DateOnly(2026, 6, 3),
                timeframe: "All day 10:00-20:00",
                attractions: new List<Attraction>
                {
                    new Attraction(
                        name: "Notre-Dame Cathedral",
                        description: "Gothic masterpiece on Île de la Cité (exterior viewing)",
                        websiteUrl: "https://www.notredamedeparis.fr/en/",
                        transportationOptions: new List<string>
                        {
                            "Metro Line 4 to Cité"
                        }
                    ),
                    new Attraction(
                        name: "Sainte-Chapelle",
                        description: "13th-century Gothic chapel with stunning stained glass windows",
                        websiteUrl: "https://www.sainte-chapelle.fr/en",
                        transportationOptions: new List<string>
                        {
                            "Walking from Notre-Dame (5 min)"
                        }
                    )
                }
            )
        };

        var seedData = new List<Citytrip>
        {
            new(1, "Paris", "Paris", "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=600",
                new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5), "seed-user", "The City of Light awaits", null, parisItinerary),
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

    public Task<Citytrip?> GetByIdWithItineraryAsync(int id)
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
