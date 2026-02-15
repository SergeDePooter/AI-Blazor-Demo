namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a single day within a citytrip itinerary.
/// </summary>
public record DayPlan
{
    public int DayNumber { get; init; }
    public DateOnly Date { get; init; }
    public string Timeframe { get; init; }
    public List<Attraction> Attractions { get; init; }

    public DayPlan(
        int dayNumber,
        DateOnly date,
        string timeframe,
        List<Attraction> attractions)
    {
        if (dayNumber <= 0)
            throw new ArgumentException("DayNumber must be positive", nameof(dayNumber));
        if (attractions == null)
            throw new ArgumentNullException(nameof(attractions));

        DayNumber = dayNumber;
        Date = date;
        Timeframe = timeframe;
        Attractions = attractions;
    }
}
