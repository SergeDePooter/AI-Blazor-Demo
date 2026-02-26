using CitytripPlanner.Features.Citytrips.CreateTrip;
using CitytripPlanner.Features.Citytrips.Domain;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.UpdateTrip;

public class UpdateTripHandler(ICitytripRepository repository)
    : IRequestHandler<UpdateTripCommand, bool>
{
    public async Task<bool> Handle(
        UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateTripValidator();
        var errors = validator.Validate(request);
        if (errors.Count > 0)
            throw new ArgumentException(string.Join(" ", errors));

        var existing = await repository.GetByIdAsync(request.TripId);
        if (existing is null)
            throw new ArgumentException($"Citytrip {request.TripId} not found.");

        if (existing.CreatorId != request.UserId)
            throw new UnauthorizedAccessException("You can only edit your own citytrips.");

        var updated = existing with
        {
            Title = request.Title,
            Destination = request.Destination,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            MaxParticipants = request.MaxParticipants,
            ImageUrl = request.ImageUrl ?? existing.ImageUrl,
            DayPlans = request.DayPlans is not null
                ? MapDayPlans(request.DayPlans)
                : existing.DayPlans
        };

        await repository.UpdateAsync(updated);
        return true;
    }

    private static List<DayPlan> MapDayPlans(List<DayPlanInput> inputs)
        => inputs.Select(dp => new DayPlan(
            dp.DayNumber,
            dp.Date,
            timeframe: "",
            attractions: [],
            events: dp.Events
                .OrderBy(e => e.StartTime)
                .Select(e => new ScheduledEvent(
                    e.EventType, e.Name, e.StartTime, e.EndTime, e.Description,
                    e.Place is null ? null : new Place(e.Place.Name, e.Place.Latitude, e.Place.Longitude)))
                .ToList()))
        .ToList();
}
