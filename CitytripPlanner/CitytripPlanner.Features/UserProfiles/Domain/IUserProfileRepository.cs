namespace CitytripPlanner.Features.UserProfiles.Domain;

public interface IUserProfileRepository
{
    /// <summary>
    /// Retrieves profile for the specified user.
    /// Returns null if profile doesn't exist.
    /// </summary>
    Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates a user profile.
    /// Creates new profile if UserId doesn't exist, otherwise updates.
    /// </summary>
    Task SaveAsync(UserProfile profile, CancellationToken cancellationToken = default);
}
