namespace CitytripPlanner.Features.Citytrips.Domain;

public interface IUserInteractionStore
{
    UserTripInteraction GetInteraction(int citytripId);
    Dictionary<int, UserTripInteraction> GetAllInteractions();
    void SaveInteraction(int citytripId, UserTripInteraction interaction);
}
