using CitytripPlanner.Features.Citytrips.CreateTrip;

namespace CitytripPlanner.Tests.Citytrips.CreateTrip;

public class CreateTripValidatorTests
{
    private readonly CreateTripValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidInput()
    {
        var command = new CreateTripCommand(
            "Valid Trip", "Valid City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", "Description", 10);

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsEmptyTitle()
    {
        var command = new CreateTripCommand(
            "", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Title"));
    }

    [Fact]
    public void Validate_RejectsEmptyDestination()
    {
        var command = new CreateTripCommand(
            "Title", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Destination"));
    }

    [Fact]
    public void Validate_RejectsEndDateBeforeStartDate()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 1),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("End date"));
    }

    [Fact]
    public void Validate_RejectsMaxParticipantsLessThanOne()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", MaxParticipants: 0);

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Max participants"));
    }

    [Fact]
    public void Validate_AcceptsNullMaxParticipants()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsTitleLongerThan100()
    {
        var command = new CreateTripCommand(
            new string('A', 101), "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("100"));
    }

    // --- New tests for T008 ---

    [Fact]
    public void Validate_AcceptsValidImageUrl()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", ImageUrl: "https://example.com/img.jpg");

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsInvalidImageUrl()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", ImageUrl: "not-a-url");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Image URL"));
    }

    [Fact]
    public void Validate_AcceptsNullImageUrl()
    {
        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsEventEndTimeNotAfterStartTime()
    {
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("museum", "Visit", new TimeOnly(14, 0), EndTime: new TimeOnly(13, 0))
        });

        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("End time"));
    }

    [Fact]
    public void Validate_AcceptsEventWithNoEndTime()
    {
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("museum", "Visit", new TimeOnly(10, 0))
        });

        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsPlaceWithLatitudeOutOfRange()
    {
        var place = new PlaceInput("Somewhere", 91.0, 0.0);
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("museum", "Visit", new TimeOnly(10, 0), Place: place)
        });

        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Latitude"));
    }

    [Fact]
    public void Validate_RejectsPlaceWithLongitudeOutOfRange()
    {
        var place = new PlaceInput("Somewhere", 0.0, 181.0);
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
        {
            new ScheduledEventInput("museum", "Visit", new TimeOnly(10, 0), Place: place)
        });

        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Longitude"));
    }

    [Fact]
    public void Validate_RejectsDayPlanDateOutsideTripRange()
    {
        var dayPlan = new DayPlanInput(1, new DateOnly(2026, 5, 31), new List<ScheduledEventInput>());

        var command = new CreateTripCommand(
            "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: new List<DayPlanInput> { dayPlan });

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("date"));
    }
}
