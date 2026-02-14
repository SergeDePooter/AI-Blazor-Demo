namespace CitytripPlanner.Features.UserProfiles.Domain;

public static class GenderOptions
{
    public const string Male = "Male";
    public const string Female = "Female";
    public const string PreferNotToSay = "Prefer not to say";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Male,
        Female,
        PreferNotToSay
    };
}
