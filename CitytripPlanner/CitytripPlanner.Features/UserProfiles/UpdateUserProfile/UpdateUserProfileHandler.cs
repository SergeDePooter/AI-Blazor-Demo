using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.UserProfiles.Domain;
using MediatR;

namespace CitytripPlanner.Features.UserProfiles.UpdateUserProfile;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    private readonly IUserProfileRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileHandler(
        IUserProfileRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var profile = new UserProfile
        {
            UserId = userId,
            Name = request.Name,
            Firstname = request.Firstname,
            Gender = request.Gender,
            Country = request.Country
        };

        await _repository.SaveAsync(profile, cancellationToken);

        return Unit.Value;
    }
}
