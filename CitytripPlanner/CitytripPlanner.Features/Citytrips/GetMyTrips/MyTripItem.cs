namespace CitytripPlanner.Features.Citytrips.GetMyTrips;

public record MyTripItem(
    int Id,
    string Title,
    string Destination,
    string? ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description,
    int? MaxParticipants,
    int EnlistedCount);
