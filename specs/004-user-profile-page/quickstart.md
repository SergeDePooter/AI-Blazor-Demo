# Quickstart Guide: User Profile Management

**Feature**: 004-user-profile-page
**Audience**: Developers implementing or extending this feature
**Prerequisites**: Familiarity with C#, .NET 10, Blazor, and MediatR

---

## Overview

This feature adds user profile management to CitytripPlanner. Users can view and edit their personal information (name, firstname, gender, country) on a dedicated profile page.

**Architecture**: CQRS vertical slices with in-memory storage
**Key Technologies**: Blazor Server, MediatR, FluentValidation

---

## Quick Navigation

| Need | Go To |
|------|-------|
| Run the feature locally | [Running Locally](#running-locally) |
| Test profile updates | [Testing Guide](#testing-guide) |
| Add a new profile field | [Extending the Feature](#extending-the-feature) |
| Understand the architecture | [Architecture Overview](#architecture-overview) |
| Run unit tests | [Running Tests](#running-tests) |

---

## Running Locally

### Prerequisites

1. **.NET 10 SDK** installed
2. IDE with C# support (Visual Studio 2025, VS Code with C# extension, Rider)
3. Git repository cloned: `C:\Projects\demo-speckit-ai\`

### Steps

1. **Navigate to project directory**:
   ```bash
   cd C:\Projects\demo-speckit-ai\CitytripPlanner
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the web application**:
   ```bash
   cd CitytripPlanner.Web
   dotnet run
   ```

5. **Open browser** to the URL shown in terminal (usually `https://localhost:5001`)

6. **Navigate to profile page**: Click "Profile" in navigation or go to `/profile`

7. **Test the feature**:
   - View profile (initially empty)
   - Click "Edit" to enable editing
   - Fill in name, firstname, select gender and country
   - Click "Save"
   - Verify changes are displayed

---

## Testing Guide

### Manual Testing Checklist

#### User Story 1: View Profile (P1)

- [ ] Navigate to `/profile` page
- [ ] Verify page loads within 2 seconds
- [ ] If profile exists, verify all fields display correctly
- [ ] If no profile exists, verify empty form is shown

#### User Story 2: Edit Basic Information (P2)

- [ ] Click "Edit" button
- [ ] Verify Name and Firstname fields become editable
- [ ] Enter values for Name and Firstname
- [ ] Click "Save"
- [ ] Verify success message appears
- [ ] Verify changes persist after page reload
- [ ] Click "Edit", then "Cancel" → verify changes are discarded

**Validation Tests**:
- [ ] Try to save with empty Name → verify error message
- [ ] Try to save with empty Firstname → verify error message
- [ ] Enter Name > 100 characters → verify error message
- [ ] Enter Firstname > 100 characters → verify error message

#### User Story 3: Edit Additional Information (P3)

- [ ] Click "Edit" button
- [ ] Select a gender from dropdown
- [ ] Select a country from dropdown
- [ ] Click "Save"
- [ ] Verify changes persist
- [ ] Clear gender selection, save → verify optional field works
- [ ] Clear country selection, save → verify optional field works

**Edge Case Tests**:
- [ ] Enter name with 100 characters (boundary test)
- [ ] Enter name with special characters (é, ñ, ü)
- [ ] Enter name with emoji (🙂) or CJK characters (中文)
- [ ] Open profile in two browser tabs, edit in both → last save wins
- [ ] Try to save without changes → should succeed (no error)

---

## Running Tests

### Unit Tests

```bash
cd C:\Projects\demo-speckit-ai\CitytripPlanner
dotnet test
```

**Test Coverage**:
- `GetUserProfileHandlerTests.cs` - Query handler tests
- `UpdateUserProfileHandlerTests.cs` - Command handler tests
- `UpdateUserProfileValidatorTests.cs` - Validation tests
- `ProfilePageTests.cs` - Blazor component tests (bUnit)

### Run Specific Test

```bash
dotnet test --filter "FullyQualifiedName~GetUserProfileHandlerTests"
```

### Run with Code Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## Architecture Overview

### Project Structure

```text
CitytripPlanner.Features/UserProfiles/
├── Domain/
│   ├── UserProfile.cs              # Entity
│   ├── IUserProfileRepository.cs   # Repository interface
│   ├── GenderOptions.cs            # Gender constants
│   └── Countries.cs                # Country list
├── GetUserProfile/
│   ├── GetUserProfileQuery.cs      # Query
│   ├── GetUserProfileHandler.cs    # Query handler
│   └── UserProfileResponse.cs      # DTO
└── UpdateUserProfile/
    ├── UpdateUserProfileCommand.cs # Command
    ├── UpdateUserProfileHandler.cs # Command handler
    └── UpdateUserProfileValidator.cs # FluentValidation

CitytripPlanner.Infrastructure/UserProfiles/
└── InMemoryUserProfileRepository.cs # Repository implementation

CitytripPlanner.Web/Components/Pages/
├── Profile.razor                   # Blazor page
└── Profile.razor.css               # Scoped styles

CitytripPlanner.Tests/UserProfiles/
└── [Test files matching above structure]
```

### Data Flow

**View Profile** (Query):
```
User → Profile.razor → GetUserProfileQuery
  → GetUserProfileHandler → IUserProfileRepository
  → InMemoryUserProfileRepository → UserProfile entity
  → UserProfileResponse DTO → Profile.razor → Display
```

**Update Profile** (Command):
```
User → Profile.razor → UpdateUserProfileCommand
  → UpdateUserProfileValidator (validation)
  → UpdateUserProfileHandler → IUserProfileRepository
  → InMemoryUserProfileRepository → Save UserProfile
  → Success → Profile.razor → Show message
```

---

## Extending the Feature

### Adding a New Profile Field

**Example**: Add a "Phone Number" field

#### Step 1: Update Entity

**File**: `CitytripPlanner.Features/UserProfiles/Domain/UserProfile.cs`

```csharp
public class UserProfile
{
    // ... existing fields ...

    public string? PhoneNumber { get; set; }  // NEW
}
```

#### Step 2: Update DTOs

**File**: `CitytripPlanner.Features/UserProfiles/GetUserProfile/UserProfileResponse.cs`

```csharp
public record UserProfileResponse(
    string Name,
    string Firstname,
    string? Gender,
    string? Country,
    string? PhoneNumber  // NEW
);
```

**File**: `CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileCommand.cs`

```csharp
public record UpdateUserProfileCommand(
    string Name,
    string Firstname,
    string? Gender,
    string? Country,
    string? PhoneNumber  // NEW
) : IRequest<Unit>;
```

#### Step 3: Update Validator (if needed)

**File**: `CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileValidator.cs`

```csharp
public UpdateUserProfileValidator()
{
    // ... existing rules ...

    RuleFor(x => x.PhoneNumber)
        .Matches(@"^\+?[0-9\s\-()]+$")  // Example: phone regex
        .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
        .WithMessage("Phone number is not valid");
}
```

#### Step 4: Update Handlers

**File**: `CitytripPlanner.Features/UserProfiles/GetUserProfile/GetUserProfileHandler.cs`

```csharp
return new UserProfileResponse(
    profile.Name,
    profile.Firstname,
    profile.Gender,
    profile.Country,
    profile.PhoneNumber  // NEW
);
```

**File**: `CitytripPlanner.Features/UserProfiles/UpdateUserProfile/UpdateUserProfileHandler.cs`

```csharp
var profile = new UserProfile
{
    UserId = userId,
    Name = request.Name,
    Firstname = request.Firstname,
    Gender = request.Gender,
    Country = request.Country,
    PhoneNumber = request.PhoneNumber  // NEW
};
```

#### Step 5: Update Blazor Component

**File**: `CitytripPlanner.Web/Components/Pages/Profile.razor`

```csharp
// Add state variable
private string? phoneNumber = null;

// Update OnInitializedAsync
phoneNumber = profile.PhoneNumber ?? string.Empty;

// Update SaveProfileAsync
var command = new UpdateUserProfileCommand(
    // ... existing fields ...
    PhoneNumber: phoneNumber
);

// Add input field in markup
<div class="form-group">
    <label>Phone Number:</label>
    <input type="tel" @bind="phoneNumber" disabled="@(!isEditing)" />
</div>
```

#### Step 6: Update Tests

Add test cases for the new field in:
- `GetUserProfileHandlerTests.cs`
- `UpdateUserProfileHandlerTests.cs`
- `UpdateUserProfileValidatorTests.cs`
- `ProfilePageTests.cs`

#### Step 7: Run Tests

```bash
dotnet test
```

---

## Common Tasks

### Change Gender Options

**File**: `CitytripPlanner.Features/UserProfiles/Domain/GenderOptions.cs`

Add or modify options:
```csharp
public static class GenderOptions
{
    public const string Male = "Male";
    public const string Female = "Female";
    public const string NonBinary = "Non-binary";  // NEW
    public const string PreferNotToSay = "Prefer not to say";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Male,
        Female,
        NonBinary,  // NEW
        PreferNotToSay
    };
}
```

**Impact**: Validator and dropdown automatically use updated list.

### Add Country Subset

**File**: `CitytripPlanner.Features/UserProfiles/Domain/Countries.cs`

Create a filtered list:
```csharp
public static class Countries
{
    public static readonly IReadOnlyList<string> All = new[] { /* full list */ };

    // NEW: European countries only
    public static readonly IReadOnlyList<string> European = new[]
    {
        "Belgium",
        "France",
        "Germany",
        // ...
    };
}
```

Update `Profile.razor` dropdown to use `Countries.European` if needed.

### Switch to Database Storage

**Future Enhancement** (requires database setup):

1. Create EF Core `DbContext` with `UserProfile` entity
2. Implement `EfUserProfileRepository : IUserProfileRepository`
3. Update DI registration in `Program.cs`:
   ```csharp
   services.AddScoped<IUserProfileRepository, EfUserProfileRepository>();
   ```
4. No changes needed to handlers, validators, or UI (abstraction via interface)

---

## Troubleshooting

### Profile Not Saving

**Symptom**: Save button clicked, no error, but data doesn't persist

**Possible Causes**:
1. Validation failed silently → Check browser console for errors
2. Repository not registered in DI → Check `Program.cs` service registration
3. Command not dispatched → Add breakpoint in handler

**Solution**: Check MediatR registration and DI setup

### Validation Errors Not Showing

**Symptom**: Invalid data submitted, but no error messages appear

**Possible Causes**:
1. Validator not registered → Check MediatR setup includes validators
2. Component not catching `ValidationException` → Check try-catch in `SaveProfileAsync`

**Solution**: Ensure FluentValidation is configured in MediatR pipeline

### Country Dropdown Is Empty

**Symptom**: Gender dropdown works, but country dropdown has no options

**Possible Causes**:
1. `Countries.All` not loaded → Check static class initialization
2. Binding issue in Blazor → Check `@foreach (var c in Countries.All)` in razor

**Solution**: Verify `Countries.cs` is in correct namespace and accessible

---

## Key Files Reference

| File | Purpose | Lines |
|------|---------|-------|
| `UserProfile.cs` | Entity definition | ~25 |
| `GenderOptions.cs` | Gender constants | ~15 |
| `Countries.cs` | Country list | ~200 |
| `GetUserProfileHandler.cs` | Query handler | ~30 |
| `UpdateUserProfileHandler.cs` | Command handler | ~40 |
| `UpdateUserProfileValidator.cs` | Validation rules | ~35 |
| `InMemoryUserProfileRepository.cs` | Storage implementation | ~50 |
| `Profile.razor` | UI component | ~150 |

**Total Feature Size**: ~550 lines of code (excluding tests and country list data)

---

## Next Steps

After implementing this feature:

1. **Run all tests**: `dotnet test`
2. **Manual testing**: Follow the testing checklist above
3. **Code review**: Verify constitution compliance (CQRS, TDD, layering)
4. **Documentation**: Update CLAUDE.md with new patterns (via `update-agent-context.ps1`)
5. **Merge**: Create PR to main branch

---

## Support

**Questions?**
- Review the feature specification: [spec.md](./spec.md)
- Check the implementation plan: [plan.md](./plan.md)
- Review data model: [data-model.md](./data-model.md)
- Check contracts: [contracts/mediatr-contracts.md](./contracts/mediatr-contracts.md)

**Found a bug?**
- Create an issue in the repository
- Include steps to reproduce
- Reference this feature: `004-user-profile-page`
