namespace CitytripPlanner.Features.Citytrips.Domain;

public record Citytrip(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string CreatorId,
    string? Description = null,
    int? MaxParticipants = null,
    List<DayPlan>? DayPlans = null);
