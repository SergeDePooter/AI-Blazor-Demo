namespace CitytripPlanner.Features.Citytrips.ListCitytrips;

public record CitytripCard(
    int Id,
    string CityName,
    string ImageUrl,
    int DurationInDays,
    bool IsLiked,
    bool IsEnlisted);
