using CitytripPlanner.Features.Citytrips.ListCitytrips;

namespace CitytripPlanner.Tests.Citytrips.ListCitytrips;

public class CitytripFilterTests
{
    private static List<CitytripCard> SampleTrips() =>
    [
        new(1, "Weekend in Paris", "Paris", "", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), false, false),
        new(2, "Berlin Adventure", "Berlin", "", new DateOnly(2026, 7, 10), new DateOnly(2026, 7, 15), false, false),
        new(3, "Rome Getaway", "Rome", "", new DateOnly(2026, 8, 5), new DateOnly(2026, 8, 10), false, false),
        new(4, "Paradise Island", "Bali", "", new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 14), false, false),
    ];

    [Fact]
    public void Apply_ReturnsAllTrips_WhenNoCriteriaSet()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria();

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Apply_FiltersByTitle_CaseInsensitive()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(SearchText: "berlin");

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Single(result);
        Assert.Equal("Berlin Adventure", result[0].Title);
    }

    [Fact]
    public void Apply_FiltersByDestination_CaseInsensitive()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(SearchText: "ROME");

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Single(result);
        Assert.Equal("Rome", result[0].Destination);
    }

    [Fact]
    public void Apply_FiltersByTitleOrDestination_PartialMatch()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(SearchText: "par");

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Title == "Weekend in Paris");
        Assert.Contains(result, t => t.Title == "Paradise Island");
    }

    [Fact]
    public void Apply_TrimsWhitespaceSearchText()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(SearchText: "   ");

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Apply_FiltersByFromDate_ExcludesTripsEndingBefore()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(FromDate: new DateOnly(2026, 7, 15));

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(result, t => t.Title == "Weekend in Paris");
    }

    [Fact]
    public void Apply_FiltersByToDate_ExcludesTripsStartingAfter()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(ToDate: new DateOnly(2026, 7, 10));

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Title == "Weekend in Paris");
        Assert.Contains(result, t => t.Title == "Berlin Adventure");
    }

    [Fact]
    public void Apply_CombinesTextAndDateFilters_WithAndLogic()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(
            SearchText: "par",
            FromDate: new DateOnly(2026, 8, 1));

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Single(result);
        Assert.Equal("Paradise Island", result[0].Title);
    }

    [Fact]
    public void Apply_ReturnsEmptyList_WhenNoTripsMatch()
    {
        var trips = SampleTrips();
        var criteria = new CitytripFilterCriteria(SearchText: "Tokyo");

        var result = CitytripFilter.Apply(trips, criteria);

        Assert.Empty(result);
    }
}
