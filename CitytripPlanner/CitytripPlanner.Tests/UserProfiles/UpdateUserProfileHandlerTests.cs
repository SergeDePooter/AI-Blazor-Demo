using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.UserProfiles.Domain;
using CitytripPlanner.Features.UserProfiles.UpdateUserProfile;
using CitytripPlanner.Infrastructure.UserProfiles;

namespace CitytripPlanner.Tests.UserProfiles;

public class UpdateUserProfileHandlerTests
{
    private class MockCurrentUserService : ICurrentUserService
    {
        public string UserId { get; set; } = "test-user";
        public string DisplayName { get; set; } = "Test User";
    }

    [Fact]
    public async Task Handle_CreatesNewProfile_WhenNoneExists()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "user-123" };
        var handler = new UpdateUserProfileHandler(repository, currentUserService);

        var command = new UpdateUserProfileCommand(
            Name: "Doe",
            Firstname: "John",
            Gender: null,
            Country: null
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await repository.GetByUserIdAsync("user-123");
        Assert.NotNull(saved);
        Assert.Equal("user-123", saved.UserId);
        Assert.Equal("Doe", saved.Name);
        Assert.Equal("John", saved.Firstname);
        Assert.Null(saved.Gender);
        Assert.Null(saved.Country);
    }

    [Fact]
    public async Task Handle_UpdatesExistingProfile()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "user-456" };

        var existing = new UserProfile
        {
            UserId = "user-456",
            Name = "OldName",
            Firstname = "OldFirstname",
            Gender = GenderOptions.Male,
            Country = "Old Country"
        };
        await repository.SaveAsync(existing);

        var handler = new UpdateUserProfileHandler(repository, currentUserService);

        var command = new UpdateUserProfileCommand(
            Name: "NewName",
            Firstname: "NewFirstname",
            Gender: GenderOptions.Female,
            Country: "New Country"
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await repository.GetByUserIdAsync("user-456");
        Assert.NotNull(updated);
        Assert.Equal("NewName", updated.Name);
        Assert.Equal("NewFirstname", updated.Firstname);
        Assert.Equal(GenderOptions.Female, updated.Gender);
        Assert.Equal("New Country", updated.Country);
    }

    [Fact]
    public async Task Handle_UsesUserIdFromCurrentUserService()
    {
        // Arrange
        var repository = new InMemoryUserProfileRepository();
        var currentUserService = new MockCurrentUserService { UserId = "specific-user-id" };
        var handler = new UpdateUserProfileHandler(repository, currentUserService);

        var command = new UpdateUserProfileCommand(
            Name: "Test",
            Firstname: "User",
            Gender: null,
            Country: null
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await repository.GetByUserIdAsync("specific-user-id");
        Assert.NotNull(saved);
        Assert.Equal("specific-user-id", saved.UserId);
    }
}
