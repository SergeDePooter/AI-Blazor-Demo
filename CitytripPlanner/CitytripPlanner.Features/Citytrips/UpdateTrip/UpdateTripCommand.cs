using CitytripPlanner.Features.Citytrips.CreateTrip;
using MediatR;

namespace CitytripPlanner.Features.Citytrips.UpdateTrip;

public record UpdateTripCommand(
    int TripId,
    string Title,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    string UserId,
    string? Description = null,
    int? MaxParticipants = null,
    string? ImageUrl = null,
    List<DayPlanInput>? DayPlans = null) : IRequest<bool>;
