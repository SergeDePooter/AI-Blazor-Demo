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
    List<AttractionDetail> Attractions);

public record AttractionDetail(
    string Name,
    string Description,
    string? WebsiteUrl,
    List<string> TransportationOptions);
