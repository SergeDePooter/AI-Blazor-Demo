using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Infrastructure.Citytrips;

namespace CitytripPlanner.Tests.Citytrips.Infrastructure;

public class InMemoryCitytripRepositoryTests
{
    private readonly InMemoryCitytripRepository _repository = new();

    [Fact]
    public async Task GetAllAsync_ReturnsSeedData()
    {
        var trips = await _repository.GetAllAsync();

        Assert.NotEmpty(trips);
        Assert.True(trips.Count >= 6);
        Assert.Contains(trips, t => t.CityName == "Paris");
        Assert.Contains(trips, t => t.CityName == "Barcelona");
        Assert.Contains(trips, t => t.CityName == "Rome");
        Assert.Contains(trips, t => t.CityName == "Amsterdam");
        Assert.Contains(trips, t => t.CityName == "Prague");
        Assert.Contains(trips, t => t.CityName == "Lisbon");
    }

    [Fact]
    public async Task GetAllAsync_AllTripsHavePositiveDuration()
    {
        var trips = await _repository.GetAllAsync();

        Assert.All(trips, t => Assert.True(t.DurationInDays > 0));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTrip()
    {
        var trips = await _repository.GetAllAsync();
        var first = trips[0];

        var result = await _repository.GetByIdAsync(first.Id);

        Assert.NotNull(result);
        Assert.Equal(first.CityName, result.CityName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonExistent()
    {
        var result = await _repository.GetByIdAsync(-1);

        Assert.Null(result);
    }
}
