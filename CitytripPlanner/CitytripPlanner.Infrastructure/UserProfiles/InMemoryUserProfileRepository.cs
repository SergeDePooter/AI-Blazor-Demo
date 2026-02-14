using System.Collections.Concurrent;
using CitytripPlanner.Features.UserProfiles.Domain;

namespace CitytripPlanner.Infrastructure.UserProfiles;

public class InMemoryUserProfileRepository : IUserProfileRepository
{
    private readonly ConcurrentDictionary<string, UserProfile> _profiles = new();

    public Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        _profiles.TryGetValue(userId, out var profile);
        return Task.FromResult(profile);
    }

    public Task SaveAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        _profiles[profile.UserId] = profile;
        return Task.CompletedTask;
    }
}
