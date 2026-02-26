using CitytripPlanner.Features.Citytrips.CreateTrip;
using CitytripPlanner.Features.Citytrips.UpdateTrip;

namespace CitytripPlanner.Tests.Citytrips.UpdateTrip;

public class UpdateTripValidatorTests
{
    private readonly UpdateTripValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidInput()
    {
        var command = new UpdateTripCommand(
            1, "Valid Trip", "Valid City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", "Description", 10);

        var errors = _validator.Validate(command);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsInvalidTripId()
    {
        var command = new UpdateTripCommand(
            0, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Trip ID"));
    }

    [Fact]
    public void Validate_RejectsEmptyTitle()
    {
        var command = new UpdateTripCommand(
            1, "", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Title"));
    }

    [Fact]
    public void Validate_RejectsEndDateBeforeStartDate()
    {
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 1),
            "user-1");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("End date"));
    }

    [Fact]
    public void Validate_RejectsMaxParticipantsLessThanOne()
    {
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", MaxParticipants: 0);

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Max participants"));
    }

    [Fact]
    public void Validate_InvalidImageUrl_ReturnsError()
    {
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", ImageUrl: "not-a-valid-url");

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("Image URL"));
    }

    [Fact]
    public void Validate_NullImageUrl_IsValid()
    {
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", ImageUrl: null);

        var errors = _validator.Validate(command);

        Assert.DoesNotContain(errors, e => e.Contains("Image URL"));
    }

    [Fact]
    public void Validate_EmptyImageUrl_IsValid()
    {
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", ImageUrl: "");

        var errors = _validator.Validate(command);

        Assert.DoesNotContain(errors, e => e.Contains("Image URL"));
    }

    [Fact]
    public void Validate_EventEndTimeBeforeStartTime_ReturnsError()
    {
        var dayPlans = new List<DayPlanInput>
        {
            new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
            {
                new ScheduledEventInput("museum", "Test Event",
                    new TimeOnly(14, 0), new TimeOnly(10, 0))  // EndTime before StartTime
            })
        };
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: dayPlans);

        var errors = _validator.Validate(command);

        Assert.Contains(errors, e => e.Contains("End time"));
    }

    [Fact]
    public void Validate_EventNullEndTime_IsValid()
    {
        var dayPlans = new List<DayPlanInput>
        {
            new DayPlanInput(1, new DateOnly(2026, 6, 1), new List<ScheduledEventInput>
            {
                new ScheduledEventInput("museum", "Test Event", new TimeOnly(10, 0))
            })
        };
        var command = new UpdateTripCommand(
            1, "Title", "City",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5),
            "user-1", DayPlans: dayPlans);

        var errors = _validator.Validate(command);

        Assert.DoesNotContain(errors, e => e.Contains("End time"));
    }
}
