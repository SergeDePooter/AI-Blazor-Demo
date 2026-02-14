using MediatR;

namespace CitytripPlanner.Features.UserProfiles.UpdateUserProfile;

public record UpdateUserProfileCommand(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
) : IRequest<Unit>;
