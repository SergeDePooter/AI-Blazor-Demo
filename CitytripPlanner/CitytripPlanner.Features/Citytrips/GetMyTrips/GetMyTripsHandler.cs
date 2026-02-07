using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetMyTrips;

public class GetMyTripsHandler(
    ICitytripRepository repository,
    IUserInteractionStore interactionStore)
    : IRequestHandler<GetMyTripsQuery, List<MyTripItem>>
{
    public async Task<List<MyTripItem>> Handle(
        GetMyTripsQuery request, CancellationToken cancellationToken)
    {
        var trips = await repository.GetByCreatorAsync(request.CreatorId);
        var interactions = interactionStore.GetAllInteractions();

        return trips.Select(t =>
        {
            var enlistedCount = interactions.Values
                .Count(i => i.CitytripId == t.Id && i.IsEnlisted);

            return new MyTripItem(
                t.Id,
                t.Title,
                t.Destination,
                t.ImageUrl,
                t.StartDate,
                t.EndDate,
                t.Description,
                t.MaxParticipants,
                enlistedCount);
        }).ToList();
    }
}
