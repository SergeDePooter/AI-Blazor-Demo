using MediatR;

namespace CitytripPlanner.Features.Citytrips.DeleteTrip;

public record DeleteTripCommand(int TripId, string UserId) : IRequest<bool>;
