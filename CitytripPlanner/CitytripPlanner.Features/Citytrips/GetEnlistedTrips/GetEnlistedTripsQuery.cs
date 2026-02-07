using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetEnlistedTrips;

public record GetEnlistedTripsQuery(string UserId) : IRequest<List<EnlistedTripItem>>;
