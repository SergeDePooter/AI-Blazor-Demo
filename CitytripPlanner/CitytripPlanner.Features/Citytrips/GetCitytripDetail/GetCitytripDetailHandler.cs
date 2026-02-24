using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

public class GetCitytripDetailHandler(ICitytripRepository repository)
    : IRequestHandler<GetCitytripDetailQuery, CitytripDetailResponse?>
{
    public async Task<CitytripDetailResponse?> Handle(
        GetCitytripDetailQuery request, CancellationToken cancellationToken)
    {
        var trip = await repository.GetByIdWithItineraryAsync(request.Id);
        if (trip is null) return null;

        return new CitytripDetailResponse(
            trip.Id,
            trip.Title,
            trip.Destination,
            trip.ImageUrl,
            trip.StartDate,
            trip.EndDate,
            trip.Description,
            trip.MaxParticipants,
            (trip.DayPlans ?? new List<DayPlan>())
                .OrderBy(d => d.DayNumber)
                .Select(d => new DayPlanDetail(
                    d.DayNumber,
                    d.Date,
                    d.Timeframe,
                    d.Attractions.Select(a => new AttractionDetail(
                        a.Name,
                        a.Description,
                        a.WebsiteUrl,
                        a.TransportationOptions)).ToList()
                )).ToList()
        );
    }
}
