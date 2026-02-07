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
}
