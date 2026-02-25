namespace CitytripPlanner.Features.Citytrips.CreateTrip;

public record DayPlanInput(
    int DayNumber,
    DateOnly Date,
    List<ScheduledEventInput> Events);
