# MediatR Contracts: User Profile Management

**Feature**: 004-user-profile-page
**Date**: 2026-02-14
**Type**: CQRS Commands and Queries

---

## Overview

This feature uses MediatR for CQRS implementation. All handlers are registered automatically via `services.AddMediatR(typeof(GetUserProfileHandler).Assembly)` in the Features project.

---

## Queries

### GetUserProfileQuery

**Purpose**: Retrieve the authenticated user's profile information.

**Namespace**: `CitytripPlanner.Features.UserProfiles.GetUserProfile`

**Contract**:
```csharp
public record GetUserProfileQuery() : IRequest<UserProfileResponse?>;
```

**Request Parameters**: None (UserId inferred from `ICurrentUserService`)

**Response**:
```csharp
public record UserProfileResponse(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
);
```

**Response Scenarios**:
- **Profile exists**: Returns `UserProfileResponse` with populated fields
- **Profile doesn't exist**: Returns `null`
- **User not authenticated**: Handler throws or returns null (depends on `ICurrentUserService` implementation)

**Handler Signature**:
```csharp
public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    public async Task<UserProfileResponse?> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken);
}
```

**Dependencies**:
- `IUserProfileRepository` - data access
- `ICurrentUserService` - get authenticated UserId

**Example Usage** (from Blazor component):
```csharp
@inject IMediator Mediator

var profile = await Mediator.Send(new GetUserProfileQuery());
if (profile != null)
{
    // Display profile data
}
else
{
    // Show empty form
}
```

---

## Commands

### UpdateUserProfileCommand

**Purpose**: Update the authenticated user's profile information.

**Namespace**: `CitytripPlanner.Features.UserProfiles.UpdateUserProfile`

**Contract**:
```csharp
public record UpdateUserProfileCommand(
    string Name,
    string Firstname,
    string? Gender,
    string? Country
) : IRequest<Unit>;
```

**Request Parameters**:
- `Name` (required): User's last name, max 100 chars
- `Firstname` (required): User's first name, max 100 chars
- `Gender` (optional): One of `GenderOptions.All` values or null
- `Country` (optional): One of `Countries.All` values or null

**UserId**: Not in command (inferred from `ICurrentUserService` in handler)

**Response**: `Unit` (void equivalent from MediatR)

**Validation**: `UpdateUserProfileValidator` runs before handler

**Validation Errors**:
```csharp
// FluentValidation.ValidationException thrown if:
// - Name is empty or > 100 chars
// - Firstname is empty or > 100 chars
// - Gender is not in GenderOptions.All (if provided)
// - Country is not in Countries.All (if provided)
```

**Handler Signature**:
```csharp
public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken);
}
```

**Dependencies**:
- `IUserProfileRepository` - data access
- `ICurrentUserService` - get authenticated UserId

**Handler Logic**:
1. Get UserId from `ICurrentUserService`
2. Create or update `UserProfile` entity with command data
3. Call `repository.SaveAsync(profile)`
4. Return `Unit.Value`

**Example Usage** (from Blazor component):
```csharp
@inject IMediator Mediator

var command = new UpdateUserProfileCommand(
    Name: "Doe",
    Firstname: "John",
    Gender: GenderOptions.Male,
    Country: "United States"
);

try
{
    await Mediator.Send(command);
    // Show success message
}
catch (FluentValidation.ValidationException ex)
{
    // Show validation errors
}
```

---

## Component Contracts

### Profile.razor

**Purpose**: Blazor page for viewing and editing user profile.

**Route**: `/profile`

**Parameters**: None (uses `IMediator` to send queries/commands)

**Component State**:
```csharp
private string name = string.Empty;
private string firstname = string.Empty;
private string? gender = null;
private string? country = null;
private bool isEditing = false;
private bool isSaving = false;
private string? errorMessage = null;
private string? successMessage = null;
```

**Component Interface**:
```csharp
// Injected Services
[Inject] private IMediator Mediator { get; set; } = null!;

// Lifecycle Methods
protected override async Task OnInitializedAsync()
{
    // Load profile via GetUserProfileQuery
}

// User Actions
private void EnableEdit() { }
private void CancelEdit() { }
private async Task SaveProfileAsync() { }
```

**User Flow**:
1. **Page Load** → `OnInitializedAsync()` sends `GetUserProfileQuery`
2. **View Mode** → Display profile data (read-only)
3. **Click Edit** → `EnableEdit()` sets `isEditing = true`, fields become editable
4. **Click Save** → `SaveProfileAsync()` sends `UpdateUserProfileCommand`
5. **Success** → Show success message, return to view mode
6. **Validation Error** → Show error messages, stay in edit mode
7. **Click Cancel** → `CancelEdit()` restores original values, return to view mode

**Dropdown Bindings**:
```csharp
<!-- Gender Dropdown -->
<select @bind="gender">
    <option value="">@("(Select gender)")</option>
    @foreach (var option in GenderOptions.All)
    {
        <option value="@option">@option</option>
    }
</select>

<!-- Country Dropdown -->
<select @bind="country">
    <option value="">@("(Select country)")</option>
    @foreach (var c in Countries.All)
    {
        <option value="@c">@c</option>
    }
</select>
```

---

## Error Handling

### Validation Errors (UpdateUserProfileCommand)

**Source**: `UpdateUserProfileValidator` via FluentValidation

**Exception Type**: `FluentValidation.ValidationException`

**Component Handling**:
```csharp
try
{
    await Mediator.Send(command);
}
catch (FluentValidation.ValidationException ex)
{
    errorMessage = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
}
```

**Error Messages** (from validator):
- "Name is required"
- "Name must not exceed 100 characters"
- "Firstname is required"
- "Firstname must not exceed 100 characters"
- "Gender must be one of: Male, Female, Prefer not to say"
- "Country must be a valid country name"

### Repository Errors

**Scenario**: `IUserProfileRepository.SaveAsync()` throws exception

**Handling**: Propagate to component, display generic error

**Component Code**:
```csharp
try
{
    await Mediator.Send(command);
}
catch (Exception ex)
{
    errorMessage = "Failed to save profile. Please try again.";
    // Log exception (if logging available)
}
```

---

## Authorization

**Current Scope**: No explicit authorization checks (assumes user is authenticated)

**Assumptions**:
- Blazor Server requires authentication to access any page
- `ICurrentUserService.UserId` is always valid (non-null, non-empty)
- Users can only access their own profile (UserId from auth context)

**Future Enhancements** (out of scope for MVP):
- Add `[Authorize]` attribute to Profile.razor
- Add explicit null checks in handlers for `ICurrentUserService.UserId`
- Add admin capability to edit other users' profiles (FR-012 currently prevents this)

---

## Performance Characteristics

### GetUserProfileQuery

**Expected Latency**: < 50ms (in-memory lookup)

**Operations**:
1. Dictionary lookup by UserId: O(1)
2. Map entity to DTO: O(1)

**No Database**: In-memory storage, no I/O overhead

### UpdateUserProfileCommand

**Expected Latency**: < 100ms (in-memory write)

**Operations**:
1. Validation: O(1) - simple field checks
2. Dictionary insert/update: O(1)
3. Entity creation/mapping: O(1)

**Concurrency**: Last-write-wins (no locking in MVP)

**Success Criteria Alignment**:
- SC-001: Page load < 2s ✅ (query is < 50ms)
- SC-002: Save < 3s ✅ (command is < 100ms)

---

## Testing Contracts

### Handler Tests

**GetUserProfileHandler Tests**:
```csharp
[Fact]
public async Task Handle_ProfileExists_ReturnsResponse()
[Fact]
public async Task Handle_ProfileNotFound_ReturnsNull()
[Fact]
public async Task Handle_MapsFieldsCorrectly()
```

**UpdateUserProfileHandler Tests**:
```csharp
[Fact]
public async Task Handle_ValidCommand_SavesProfile()
[Fact]
public async Task Handle_NewProfile_CreatesWithUserId()
[Fact]
public async Task Handle_ExistingProfile_Updates()
```

### Validator Tests

**UpdateUserProfileValidator Tests**:
```csharp
[Fact]
public void Validate_EmptyName_ReturnsError()
[Fact]
public void Validate_NameTooLong_ReturnsError()
[Fact]
public void Validate_InvalidGender_ReturnsError()
[Fact]
public void Validate_ValidCommand_Passes()
```

### Component Tests (bUnit)

**Profile.razor Tests**:
```csharp
[Fact]
public async Task OnInit_LoadsProfile_DisplaysData()
[Fact]
public async Task ClickEdit_EnablesEditing()
[Fact]
public async Task ClickSave_ValidData_SendsCommand()
[Fact]
public async Task ValidationError_DisplaysMessage()
```

---

## Summary

**Queries**: 1 (GetUserProfile)
**Commands**: 1 (UpdateUserProfile)
**DTOs**: 1 (UserProfileResponse)
**Components**: 1 (Profile.razor)

**CQRS Compliance**: ✅ Read and write operations fully separated
**MediatR Usage**: ✅ All operations via `IMediator.Send()`
**Validation**: ✅ FluentValidation on command pipeline
**Error Handling**: ✅ Try-catch in component, user-friendly messages
