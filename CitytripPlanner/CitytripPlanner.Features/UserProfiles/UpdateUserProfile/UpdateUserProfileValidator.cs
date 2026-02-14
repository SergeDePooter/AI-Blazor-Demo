using CitytripPlanner.Features.UserProfiles.Domain;
using FluentValidation;

namespace CitytripPlanner.Features.UserProfiles.UpdateUserProfile;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileValidator()
    {
        // FR-005: Name and Firstname are required
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname is required")
            .MaximumLength(100).WithMessage("Firstname must not exceed 100 characters");

        // FR-006: Gender must be valid option if provided
        RuleFor(x => x.Gender)
            .Must(BeValidGender).When(x => !string.IsNullOrEmpty(x.Gender))
            .WithMessage($"Gender must be one of: {string.Join(", ", GenderOptions.All)}");

        // FR-007: Country must be valid if provided
        RuleFor(x => x.Country)
            .Must(BeValidCountry).When(x => !string.IsNullOrEmpty(x.Country))
            .WithMessage("Country must be a valid country name");
    }

    private bool BeValidGender(string? gender) =>
        gender == null || GenderOptions.All.Contains(gender);

    private bool BeValidCountry(string? country) =>
        country == null || Countries.All.Contains(country);
}
