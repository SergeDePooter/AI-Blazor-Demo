using MediatR;

namespace CitytripPlanner.Features.UserProfiles.GetUserProfile;

public record GetUserProfileQuery() : IRequest<UserProfileResponse?>;
