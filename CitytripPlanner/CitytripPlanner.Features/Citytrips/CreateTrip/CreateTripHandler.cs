using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public class CreateTripHandler(ICitytripRepository repository)
    : IRequestHandler<CreateTripCommand, int>
{
    public async Task<int> Handle(
        CreateTripCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateTripValidator();
        var errors = validator.Validate(request);
        if (errors.Count > 0)
            throw new ArgumentException(string.Join(" ", errors));

        var dayPlans = request.DayPlans?
            .Select(dp => new DayPlan(
                dp.DayNumber,
                dp.Date,
                timeframe: "",
                attractions: new List<Attraction>(),
                events: dp.Events
                    .OrderBy(e => e.StartTime)
                    .Select(e => new ScheduledEvent(
                        e.EventType,
                        e.Name,
                        e.StartTime,
                        e.EndTime,
                        e.Description,
                        e.Place is null ? null : new Place(e.Place.Name, e.Place.Latitude, e.Place.Longitude)))
                    .ToList()))
            .ToList();

        var trip = new Citytrip(
            0,
            request.Title,
            request.Destination,
            request.ImageUrl ?? "",
            request.StartDate,
            request.EndDate,
            request.CreatorId,
            request.Description,
            request.MaxParticipants,
            dayPlans);

        var created = await repository.AddAsync(trip);
        return created.Id;
    }
}
