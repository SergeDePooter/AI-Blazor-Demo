using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetEnlistedTrips;

public class GetEnlistedTripsHandler(
    ICitytripRepository repository,
    IUserInteractionStore interactionStore)
    : IRequestHandler<GetEnlistedTripsQuery, List<EnlistedTripItem>>
{
    public async Task<List<EnlistedTripItem>> Handle(
        GetEnlistedTripsQuery request, CancellationToken cancellationToken)
    {
        var interactions = interactionStore.GetAllInteractions();
        var enlistedTripIds = interactions.Values
            .Where(i => i.IsEnlisted)
            .Select(i => i.CitytripId)
            .ToHashSet();

        var allTrips = await repository.GetAllAsync();

        return allTrips
            .Where(t => enlistedTripIds.Contains(t.Id) && t.CreatorId != request.UserId)
            .Select(t => new EnlistedTripItem(
                t.Id,
                t.Title,
                t.Destination,
                t.ImageUrl,
                t.StartDate,
                t.EndDate,
                t.Description,
                t.CreatorId))
            .ToList();
    }
}
