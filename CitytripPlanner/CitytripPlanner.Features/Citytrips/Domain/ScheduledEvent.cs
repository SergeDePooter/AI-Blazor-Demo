namespace CitytripPlanner.Features.Citytrips.Domain;

/// <summary>
/// Represents a generic scheduled event within a day plan (museum visit, market, stadium, etc.).
/// </summary>
public record ScheduledEvent
{
    public string EventType { get; init; }
    public string Name { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly? EndTime { get; init; }
    public string? Description { get; init; }
    public Place? Place { get; init; }

    public ScheduledEvent(
        string eventType,
        string name,
        TimeOnly startTime,
        TimeOnly? endTime = null,
        string? description = null,
        Place? place = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("EventType must not be empty.", nameof(eventType));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));
        if (endTime.HasValue && endTime.Value <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        EventType = eventType;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
        Place = place;
    }
}
