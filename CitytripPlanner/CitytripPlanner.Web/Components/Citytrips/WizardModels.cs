namespace CitytripPlanner.Web.Components.Citytrips;

public class WizardStep1Model
{
    public string Title { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
    public string? Description { get; set; }
    public int? MaxParticipants { get; set; }
    public string? ImageUrl { get; set; }
}

public class DayPlanInputModel
{
    public int DayNumber { get; set; }
    public DateOnly Date { get; set; }
    public List<ScheduledEventInputModel> Events { get; set; } = new();
}

public class ScheduledEventInputModel
{
    public string EventType { get; set; } = "";
    public string Name { get; set; } = "";
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Description { get; set; }
    public PlaceInputModel? Place { get; set; }
}

public class PlaceInputModel
{
    public string Name { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
