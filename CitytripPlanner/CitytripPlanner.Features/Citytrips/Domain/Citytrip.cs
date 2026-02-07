namespace CitytripPlanner.Features.Citytrips.Domain;

public record Citytrip(
    int Id,
    string CityName,
    string ImageUrl,
    int DurationInDays,
    string? Description = null);
