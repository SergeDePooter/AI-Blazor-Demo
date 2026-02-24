namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a geographic location associated with a scheduled event.
/// </summary>
public record Place
{
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }

    public Place(string name, double latitude, double longitude, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }
}
