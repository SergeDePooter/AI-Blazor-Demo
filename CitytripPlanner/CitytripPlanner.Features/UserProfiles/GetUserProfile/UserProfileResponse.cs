namespace CitytripPlanner.Features.UserProfiles.GetUserProfile;

public record UserProfileResponse(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
);
