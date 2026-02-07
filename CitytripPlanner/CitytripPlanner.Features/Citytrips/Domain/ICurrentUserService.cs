namespace CitytripPlanner.Features.Citytrips.Domain;

public interface ICurrentUserService
{
    string UserId { get; }
    string DisplayName { get; }
}
