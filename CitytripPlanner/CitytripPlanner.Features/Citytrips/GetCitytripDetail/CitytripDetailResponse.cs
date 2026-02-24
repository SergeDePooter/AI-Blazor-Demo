namespace CitytripPlanner.Features.Citytrips.GetCitytripDetail;

public record CitytripDetailResponse(
    int Id,
    string Title,
    string Destination,
    string ImageUrl,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description,
    int? MaxParticipants,
    List<DayPlanDetail>? DayPlans);

public record DayPlanDetail(
    int DayNumber,
    DateOnly Date,
    string Timeframe,
    List<AttractionDetail> Attractions,
    List<ScheduledEventDetail> Events);

public record AttractionDetail(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);

public record ScheduledEventDetail(
    string EventType,
    string Name,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    string? Description,
    PlaceDetail? Place);

public record PlaceDetail(
    string Name,
    double Latitude,
    double Longitude,
    string? Address);
