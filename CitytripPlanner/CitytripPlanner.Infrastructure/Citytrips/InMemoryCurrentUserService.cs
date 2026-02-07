using CitytripPlanner.Features.Citytrips.Domain;

namespace CitytripPlanner.Infrastructure.Citytrips;

public class InMemoryCurrentUserService : ICurrentUserService
{
    public string UserId => "demo-user";
    public string DisplayName => "Demo User";
}
