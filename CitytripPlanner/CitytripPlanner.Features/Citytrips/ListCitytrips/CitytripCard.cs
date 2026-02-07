namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public record CitytripCard(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsLiked,
    bool IsEnlisted);
