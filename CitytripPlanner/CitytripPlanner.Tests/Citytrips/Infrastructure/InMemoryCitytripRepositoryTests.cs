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
        Assert.Contains(trips, t => t.Title == "Paris");
        Assert.Contains(trips, t => t.Title == "Barcelona");
        Assert.Contains(trips, t => t.Title == "Rome");
        Assert.Contains(trips, t => t.Title == "Amsterdam");
        Assert.Contains(trips, t => t.Title == "Prague");
        Assert.Contains(trips, t => t.Title == "Lisbon");
    }

    [Fact]
    public async Task GetAllAsync_AllTripsHaveValidDates()
    {
        var trips = await _repository.GetAllAsync();

        Assert.All(trips, t => Assert.True(t.EndDate >= t.StartDate));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTrip()
    {
        var trips = await _repository.GetAllAsync();
        var first = trips[0];

        var result = await _repository.GetByIdAsync(first.Id);

        Assert.NotNull(result);
        Assert.Equal(first.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonExistent()
    {
        var result = await _repository.GetByIdAsync(-1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AssignsNewIdAndReturnsTrip()
    {
        var trip = new Citytrip(0, "Test Trip", "Test City", "",
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5), "test-user");

        var created = await _repository.AddAsync(trip);

        Assert.True(created.Id > 0);
        Assert.Equal("Test Trip", created.Title);
        Assert.Equal("test-user", created.CreatorId);
    }

    [Fact]
    public async Task AddAsync_TripIsRetrievableAfterAdd()
    {
        var trip = new Citytrip(0, "New Trip", "New City", "",
            new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3), "user-1");

        var created = await _repository.AddAsync(trip);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("New Trip", retrieved.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTrip()
    {
        var trips = await _repository.GetAllAsync();
        var original = trips[0];
        var updated = original with { Title = "Updated Title" };

        var result = await _repository.UpdateAsync(updated);

        Assert.Equal("Updated Title", result.Title);
        var retrieved = await _repository.GetByIdAsync(original.Id);
        Assert.Equal("Updated Title", retrieved!.Title);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsForNonExistentTrip()
    {
        var trip = new Citytrip(999, "Ghost", "Nowhere", "",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2), "nobody");

        await Assert.ThrowsAsync<ArgumentException>(
            () => _repository.UpdateAsync(trip));
    }

    [Fact]
    public async Task DeleteAsync_RemovesTrip()
    {
        var trips = await _repository.GetAllAsync();
        var first = trips[0];

        var result = await _repository.DeleteAsync(first.Id);

        Assert.True(result);
        var retrieved = await _repository.GetByIdAsync(first.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseForNonExistent()
    {
        var result = await _repository.DeleteAsync(-1);

        Assert.False(result);
    }

    [Fact]
    public async Task GetByCreatorAsync_ReturnsOnlyMatchingTrips()
    {
        var trip = new Citytrip(0, "My Trip", "My City", "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 5), "custom-user");
        await _repository.AddAsync(trip);

        var result = await _repository.GetByCreatorAsync("custom-user");

        Assert.Single(result);
        Assert.Equal("My Trip", result[0].Title);
    }

    [Fact]
    public async Task GetByCreatorAsync_ReturnsEmptyForUnknownCreator()
    {
        var result = await _repository.GetByCreatorAsync("unknown-user");

        Assert.Empty(result);
    }
}
