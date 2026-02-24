namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;
using FluentAssertions;

public class PlaceTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesPlace()
    {
        var place = new Place("Eiffel Tower", 48.8584, 2.2945);

        place.Name.Should().Be("Eiffel Tower");
        place.Latitude.Should().Be(48.8584);
        place.Longitude.Should().Be(2.2945);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        var act = () => new Place("", 48.0, 2.0);

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    public void Constructor_InvalidLatitude_ThrowsArgumentOutOfRangeException(double lat, double lng)
    {
        var act = () => new Place("X", lat, lng);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("latitude");
    }

    [Theory]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Constructor_InvalidLongitude_ThrowsArgumentOutOfRangeException(double lat, double lng)
    {
        var act = () => new Place("X", lat, lng);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("longitude");
    }

    [Theory]
    [InlineData(-90, 0)]
    [InlineData(90, 0)]
    [InlineData(0, -180)]
    [InlineData(0, 180)]
    public void Constructor_BoundaryCoordinates_Creates(double lat, double lng)
    {
        var act = () => new Place("Boundary", lat, lng);

        act.Should().NotThrow();
    }
}
