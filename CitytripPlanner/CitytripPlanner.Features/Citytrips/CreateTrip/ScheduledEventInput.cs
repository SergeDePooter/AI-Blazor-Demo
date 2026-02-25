namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public record ScheduledEventInput(
    string EventType,
    string Name,
    TimeOnly StartTime,
    TimeOnly? EndTime = null,
    string? Description = null,
    PlaceInput? Place = null);
