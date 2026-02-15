namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;
using FluentAssertions;

public class DayPlanTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesDayPlan()
    {
        // Arrange
        var dayNumber = 1;
        var date = new DateOnly(2026, 6, 1);
        var timeframe = "Morning 9:00-12:00";
        var attractions = new List<Attraction>();

        // Act
        var dayPlan = new DayPlan(dayNumber, date, timeframe, attractions);

        // Assert
        dayPlan.DayNumber.Should().Be(dayNumber);
        dayPlan.Date.Should().Be(date);
        dayPlan.Timeframe.Should().Be(timeframe);
        dayPlan.Attractions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_NegativeDayNumber_ThrowsArgumentException()
    {
        // Arrange
        var invalidDayNumber = -1;

        // Act & Assert
        var act = () => new DayPlan(invalidDayNumber, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>());
        act.Should().Throw<ArgumentException>().WithMessage("*DayNumber*");
    }

    [Fact]
    public void Constructor_ZeroDayNumber_ThrowsArgumentException()
    {
        // Arrange
        var invalidDayNumber = 0;

        // Act & Assert
        var act = () => new DayPlan(invalidDayNumber, new DateOnly(2026, 6, 1), "Morning", new List<Attraction>());
        act.Should().Throw<ArgumentException>().WithMessage("*DayNumber*");
    }

    [Fact]
    public void Constructor_NullAttractions_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new DayPlan(1, new DateOnly(2026, 6, 1), "Morning", null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("attractions");
    }
}
