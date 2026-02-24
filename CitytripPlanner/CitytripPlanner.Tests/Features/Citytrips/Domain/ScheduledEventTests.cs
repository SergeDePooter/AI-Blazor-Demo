namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;
using FluentAssertions;

public class ScheduledEventTests
{
    [Fact]
    public void Constructor_ValidData_Creates()
    {
        var ev = new ScheduledEvent("Museum", "Louvre", new TimeOnly(14, 0));

        ev.EventType.Should().Be("Museum");
        ev.Name.Should().Be("Louvre");
        ev.StartTime.Should().Be(new TimeOnly(14, 0));
        ev.EndTime.Should().BeNull();
        ev.Place.Should().BeNull();
    }

    [Fact]
    public void Constructor_EndTimeBeforeStartTime_ThrowsArgumentException()
    {
        var act = () => new ScheduledEvent("Museum", "Louvre", new TimeOnly(14, 0), new TimeOnly(13, 0));

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("", "Name")]
    [InlineData("Type", "")]
    public void Constructor_EmptyRequiredField_ThrowsArgumentException(string type, string name)
    {
        var act = () => new ScheduledEvent(type, name, new TimeOnly(9, 0));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithPlace_ExposesPlace()
    {
        var place = new Place("Tower", 48.85, 2.29);
        var ev = new ScheduledEvent("Landmark", "Eiffel", new TimeOnly(9, 0), place: place);

        ev.Place.Should().NotBeNull();
        ev.Place!.Name.Should().Be("Tower");
    }

    [Fact]
    public void Constructor_WithEndTimeAfterStartTime_Sets()
    {
        var ev = new ScheduledEvent("Museum", "Louvre", new TimeOnly(9, 0), new TimeOnly(11, 0));

        ev.EndTime.Should().Be(new TimeOnly(11, 0));
    }

    [Fact]
    public void Constructor_WithOptionalDescription_Sets()
    {
        var ev = new ScheduledEvent("Museum", "Louvre", new TimeOnly(9, 0), description: "Great art");

        ev.Description.Should().Be("Great art");
    }
}
