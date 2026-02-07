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
}
