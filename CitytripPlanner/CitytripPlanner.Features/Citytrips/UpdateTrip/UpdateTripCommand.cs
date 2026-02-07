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
    int? MaxParticipants = null) : IRequest<bool>;
