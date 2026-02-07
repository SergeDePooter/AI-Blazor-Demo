using MediatR;

namespace CitytripPlanner.Features.Citytrips.GetMyTrips;

public record GetMyTripsQuery(string CreatorId) : IRequest<List<MyTripItem>>;
