namespace CitytripPlanner.Features.UserProfiles.Domain;

public class UserProfile
{
    /// <summary>
    /// Unique identifier linking to the authenticated user.
    /// Maps to ICurrentUserService.UserId.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// User's last name / surname.
    /// Required, max 100 characters.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User's first name / given name.
    /// Required, max 100 characters.
    /// </summary>
    public required string Firstname { get; set; }

    /// <summary>
    /// User's gender selection.
    /// Optional, must be one of GenderOptions.All values if provided.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// User's country of residence.
    /// Optional, must be one of Countries.All values if provided.
    /// </summary>
    public string? Country { get; set; }
}
