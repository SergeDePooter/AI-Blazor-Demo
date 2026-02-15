namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a place to visit during a citytrip day.
/// </summary>
public record Attraction
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string? WebsiteUrl { get; init; }
    public List<string> TransportationOptions { get; init; }

    public Attraction(
        string name,
        string description,
        string? websiteUrl,
        List<string> transportationOptions)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));
        if (websiteUrl != null && !Uri.TryCreate(websiteUrl, UriKind.Absolute, out _))
            throw new ArgumentException("WebsiteUrl must be a valid absolute URI", nameof(websiteUrl));
        if (transportationOptions == null)
            throw new ArgumentNullException(nameof(transportationOptions));

        Name = name;
        Description = description;
        WebsiteUrl = websiteUrl;
        TransportationOptions = transportationOptions;
    }
}
