using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.UserProfiles.Domain;
using MediatR;

namespace CitytripPlanner.Features.UserProfiles.GetUserProfile;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    private readonly IUserProfileRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserProfileHandler(
        IUserProfileRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<UserProfileResponse?> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);

        if (profile == null)
            return null;

        return new UserProfileResponse(
            profile.Name,
            profile.Firstname,
            profile.Gender,
            profile.Country
        );
    }
}
