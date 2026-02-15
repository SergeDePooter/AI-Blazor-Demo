namespace CitytripPlanner.Tests.Infrastructure;

using CitytripPlanner.Infrastructure.Citytrips;
using FluentAssertions;

public class InMemoryCitytripRepositoryTests
{
    private readonly InMemoryCitytripRepository _repository;

    public InMemoryCitytripRepositoryTests()
    {
        _repository = new InMemoryCitytripRepository();
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_ValidId_ReturnsItinerary()
    {
        // Arrange
        const int parisId = 1;

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(parisId);

        // Assert
        citytrip.Should().NotBeNull();
        citytrip!.Id.Should().Be(parisId);
        citytrip.Title.Should().Be("Paris");
        citytrip.DayPlans.Should().NotBeNull();
        citytrip.DayPlans.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_ValidId_ReturnsDayPlansInOrder()
    {
        // Arrange
        const int parisId = 1;

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(parisId);

        // Assert
        citytrip.Should().NotBeNull();
        citytrip!.DayPlans.Should().NotBeNull();
        var dayNumbers = citytrip.DayPlans!.Select(d => d.DayNumber).ToList();
        dayNumbers.Should().BeInAscendingOrder();
        dayNumbers.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_ValidId_ReturnsAttractionsWithTransportation()
    {
        // Arrange
        const int parisId = 1;

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(parisId);

        // Assert
        citytrip.Should().NotBeNull();
        var day1 = citytrip!.DayPlans![0];
        day1.Attractions.Should().HaveCountGreaterThan(0);

        var eiffelTower = day1.Attractions[0];
        eiffelTower.Name.Should().Be("Eiffel Tower");
        eiffelTower.WebsiteUrl.Should().NotBeNullOrEmpty();
        eiffelTower.TransportationOptions.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_CitytripWithoutItinerary_ReturnsNullDayPlans()
    {
        // Arrange
        const int barcelonaId = 2; // Barcelona has no itinerary in seed data

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(barcelonaId);

        // Assert
        citytrip.Should().NotBeNull();
        citytrip!.Id.Should().Be(barcelonaId);
        citytrip.DayPlans.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        const int nonExistentId = 999;

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(nonExistentId);

        // Assert
        citytrip.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithItineraryAsync_ValidId_ReturnsAttractionWithNullWebsiteUrl()
    {
        // Arrange
        const int parisId = 1;

        // Act
        var citytrip = await _repository.GetByIdWithItineraryAsync(parisId);

        // Assert
        citytrip.Should().NotBeNull();
        var day1 = citytrip!.DayPlans![0];
        var trocadero = day1.Attractions[1]; // Trocadéro has no website
        trocadero.Name.Should().Be("Trocadéro Gardens");
        trocadero.WebsiteUrl.Should().BeNull();
        trocadero.TransportationOptions.Should().HaveCountGreaterThan(0);
    }
}
