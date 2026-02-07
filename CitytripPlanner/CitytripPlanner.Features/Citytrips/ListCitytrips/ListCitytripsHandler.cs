using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public class ListCitytripsHandler(
    ICitytripRepository repository,
    IUserInteractionStore interactionStore)
    : IRequestHandler<ListCitytripsQuery, List<CitytripCard>>
{
    public async Task<List<CitytripCard>> Handle(
        ListCitytripsQuery request, CancellationToken cancellationToken)
    {
        var trips = await repository.GetAllAsync();
        var interactions = interactionStore.GetAllInteractions();

        return trips.Select(t =>
        {
            interactions.TryGetValue(t.Id, out var interaction);
            return new CitytripCard(
                t.Id,
                t.Title,
                t.Destination,
                t.ImageUrl,
                t.StartDate,
                t.EndDate,
                interaction?.IsLiked ?? false,
                interaction?.IsEnlisted ?? false);
        }).ToList();
    }
}
