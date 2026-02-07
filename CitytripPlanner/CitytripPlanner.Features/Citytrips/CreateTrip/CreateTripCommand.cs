using MediatR;

namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public record CreateTripCommand(
    string Title,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatorId,
    string? Description = null,
    int? MaxParticipants = null) : IRequest<int>;
