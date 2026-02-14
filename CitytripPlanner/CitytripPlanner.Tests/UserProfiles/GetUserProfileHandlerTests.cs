using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.UserProfiles.Domain;
using CitytripPlanner.Features.UserProfiles.GetUserProfile;
using CitytripPlanner.Infrastructure.UserProfiles;

namespace CitytripPlanner.Tests.UserProfiles;

public class GetUserProfileHandlerTests
{
    private class MockCurrentUserService : ICurrentUserService
    {
        public string UserId { get; set; } = "test-user";
        public string DisplayName { get; set; } = "Test User";
    }

    [Fact]
    public async Task Handle_ProfileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "user-123" };
        var handler = new GetUserProfileHandler(repository, currentUserService);
        var query = new GetUserProfileQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ProfileExists_ReturnsUserProfileResponse()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "user-456" };

        var profile = new UserProfile
        {
            UserId = "user-456",
            Name = "Doe",
            Firstname = "John",
            Gender = GenderOptions.Male,
            Country = "United States"
        };
        await repository.SaveAsync(profile);

        var handler = new GetUserProfileHandler(repository, currentUserService);
        var query = new GetUserProfileQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Doe", result.Name);
        Assert.Equal("John", result.Firstname);
        Assert.Equal(GenderOptions.Male, result.Gender);
        Assert.Equal("United States", result.Country);
    }

    [Fact]
    public async Task Handle_MapsEntityFieldsCorrectly()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "user-789" };

        var profile = new UserProfile
        {
            UserId = "user-789",
            Name = "Smith",
            Firstname = "Jane",
            Gender = GenderOptions.Female,
            Country = "Canada"
        };
        await repository.SaveAsync(profile);

        var handler = new GetUserProfileHandler(repository, currentUserService);
        var query = new GetUserProfileQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(profile.Name, result.Name);
        Assert.Equal(profile.Firstname, result.Firstname);
        Assert.Equal(profile.Gender, result.Gender);
        Assert.Equal(profile.Country, result.Country);
    }
}
