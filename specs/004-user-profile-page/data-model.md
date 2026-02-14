# Data Model: User Profile Management

**Feature**: 004-user-profile-page
**Date**: 2026-02-14
**Status**: Design

---

## Entities

### UserProfile

**Purpose**: Stores personal information for an authenticated user.

**Location**: `CitytripPlanner.Features/UserProfiles/Domain/UserProfile.cs`

**Definition**:
```csharp
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
```

**Relationships**:
- `UserId` → Links to `ICurrentUserService.UserId` (one-to-one)
- No relationships to other entities

**Invariants**:
- `UserId` is immutable (init-only)
- `UserId` must be unique (repository enforces)
- At most one UserProfile per UserId

---

## Reference Data

### GenderOptions

**Purpose**: Defines valid gender selections.

**Location**: `CitytripPlanner.Features/UserProfiles/Domain/GenderOptions.cs`

**Definition**:
```csharp
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
```

**Usage**: Dropdown binding, validation

---

### Countries

**Purpose**: Provides list of valid countries (ISO 3166-1).

**Location**: `CitytripPlanner.Features/UserProfiles/Domain/Countries.cs`

**Definition**:
```csharp
public static class Countries
{
    public static readonly IReadOnlyList<string> All = new[]
    {
        "Afghanistan",
        "Albania",
        "Algeria",
        "Andorra",
        "Angola",
        // ... (full ISO 3166-1 list, 195 countries total)
        "Zambia",
        "Zimbabwe"
    };
}
```

**Usage**: Dropdown binding, validation

**Note**: Full implementation includes all 195 ISO 3166-1 recognized sovereign states, sorted alphabetically.

---

## DTOs (Data Transfer Objects)

### UserProfileResponse

**Purpose**: Returns profile data from GetUserProfile query.

**Location**: `CitytripPlanner.Features/UserProfiles/GetUserProfile/UserProfileResponse.cs`

**Definition**:
```csharp
public record UserProfileResponse(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
);
```

**Mapping**: From `UserProfile` entity (excluding UserId for security)

---

### UpdateUserProfileCommand

**Purpose**: Command to update user profile data.

**Location**: `CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileCommand.cs`

**Definition**:
```csharp
public record UpdateUserProfileCommand(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
) : IRequest<Unit>;
```

**Notes**:
- UserId is not in command (inferred from ICurrentUserService)
- Implements MediatR `IRequest<Unit>` (void return)
- Validated by `UpdateUserProfileValidator`

---

### GetUserProfileQuery

**Purpose**: Query to retrieve current user's profile.

**Location**: `CitytripPlanner.Features/UserProfiles/GetUserProfile/GetUserProfileQuery.cs`

**Definition**:
```csharp
public record GetUserProfileQuery() : IRequest<UserProfileResponse?>;
```

**Notes**:
- No parameters (UserId inferred from ICurrentUserService)
- Returns `UserProfileResponse?` (nullable, returns null if profile doesn't exist)

---

## Validation Rules

### UpdateUserProfileValidator

**Location**: `CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileValidator.cs`

**Rules**:
```csharp
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
```

**Corresponding Spec Requirements**:
- FR-003, FR-005: Name required, max 100 chars
- FR-004, FR-005: Firstname required, max 100 chars
- FR-006: Gender from predefined list
- FR-007: Country from valid list
- FR-009: Validation before saving

---

## Repository Interface

### IUserProfileRepository

**Purpose**: Abstraction for user profile storage.

**Location**: `CitytripPlanner.Features/UserProfiles/Domain/IUserProfileRepository.cs`

**Definition**:
```csharp
public interface IUserProfileRepository
{
    /// <summary>
    /// Retrieves profile for the specified user.
    /// Returns null if profile doesn't exist.
    /// </summary>
    Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates a user profile.
    /// Creates new profile if UserId doesn't exist, otherwise updates.
    /// </summary>
    Task SaveAsync(UserProfile profile, CancellationToken cancellationToken = default);
}
```

**Implementation**: `CitytripPlanner.Infrastructure/UserProfiles/InMemoryUserProfileRepository.cs`

**In-Memory Storage**:
```csharp
// Internal storage: Dictionary<string, UserProfile>
// Key: UserId
// Value: UserProfile instance
// Thread-safe: Use ConcurrentDictionary or lock for mutations
```

---

## State Transitions

### Profile Creation Flow

```text
1. New user → No profile exists
   GetUserProfile returns null

2. User fills profile form → UpdateUserProfile command
   Validator checks required fields

3. Save → Repository creates new UserProfile
   UserId set from ICurrentUserService.UserId

4. Future loads → GetUserProfile returns existing data
```

### Profile Update Flow

```text
1. User loads profile page → GetUserProfile query
   Returns existing UserProfile data

2. User edits fields → Local state in Blazor component

3. User clicks Save → UpdateUserProfile command
   Validator runs (required, length, valid options)

4. Save → Repository updates existing UserProfile
   Same UserId, updated field values

5. Page refreshes → GetUserProfile shows new data
```

---

## Edge Cases Handling

### Extremely Long Names (200+ characters)
**Spec Edge Case**: "What happens when a user provides an extremely long name?"

**Handling**:
- Validator enforces 100 character limit (FR-003, FR-004)
- Blazor input has `maxlength="100"` attribute (client-side)
- Validation message: "Name must not exceed 100 characters"

### Special Characters / Non-Latin Scripts
**Spec Edge Case**: "How does the system handle special characters or non-Latin scripts?"

**Handling**:
- Accept all Unicode characters (C# strings are UTF-16)
- No character restrictions beyond length
- Test cases include: accents (é, ñ), CJK characters (中文), emoji

### Concurrent Edits (Multiple Tabs)
**Spec Edge Case**: "How does the system handle concurrent edits (user has profile open in two browser tabs)?"

**Handling**:
- Last-write-wins (no optimistic concurrency in MVP)
- Each save overwrites entire profile
- Future enhancement: Add version/timestamp field for conflict detection

### Save Failure (Network Issues)
**Spec Edge Case**: "What happens if the save operation fails due to network issues?"

**Handling**:
- Blazor Server: In-process call, network failure unlikely
- If MediatR handler throws: Catch in component, display error message
- User prompted to retry

### No Changes Save
**Spec Edge Case**: "What happens if a user tries to save without making any changes?"

**Handling**:
- Allow save (idempotent operation)
- No error message (successful no-op)
- Alternative: Disable save button if no changes detected (future enhancement)

---

## Field Defaults

| Field | Default Value | Behavior if Not Set |
|-------|---------------|---------------------|
| UserId | (from ICurrentUserService) | Always set by handler |
| Name | (required by user) | Validation error if empty |
| Firstname | (required by user) | Validation error if empty |
| Gender | null | Optional, dropdown shows "(Select gender)" |
| Country | null | Optional, dropdown shows "(Select country)" |

---

## Summary

**Entities**: 1 (UserProfile)
**Reference Data**: 2 (GenderOptions, Countries)
**DTOs**: 3 (Query, Command, Response)
**Validators**: 1 (UpdateUserProfileValidator)
**Repository**: 1 interface + 1 in-memory implementation

**Complexity**: Low
- Simple CRUD (Create on first save, Read, Update)
- No Delete operation (out of scope)
- No relationships to other entities
- Validation is straightforward (length, required, valid selection)

**Storage**: In-memory dictionary keyed by UserId
**Concurrency**: Last-write-wins (no optimistic locking in MVP)
