namespace CitytripPlanner.Features.Citytrips.GetEnlistedTrips;

public record EnlistedTripItem(
    int Id,
    string Title,
    string Destination,
    string? ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description,
    string CreatorName);
