using CitytripPlanner.Features.Citytrips.Domain;

namespace CitytripPlanner.Infrastructure.Citytrips;

public class InMemoryUserInteractionStore : IUserInteractionStore
{
    private readonly Dictionary<int, UserTripInteraction> _interactions = new();

    public UserTripInteraction GetInteraction(int citytripId)
    {
        if (!_interactions.TryGetValue(citytripId, out var interaction))
        {
            interaction = new UserTripInteraction { CitytripId = citytripId };
            _interactions[citytripId] = interaction;
        }
        return interaction;
    }

    public Dictionary<int, UserTripInteraction> GetAllInteractions()
        => new(_interactions);

    public void SaveInteraction(int citytripId, UserTripInteraction interaction)
        => _interactions[citytripId] = interaction;
}
