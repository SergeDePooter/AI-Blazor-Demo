# Technical Research: User Profile Management

**Feature**: 004-user-profile-page
**Date**: 2026-02-14
**Purpose**: Resolve technical clarifications identified in Phase 0

---

## Research Task 1: Gender Options Implementation

### Context
The feature requires a gender field with three predefined options: "Male", "Female", "Prefer not to say" (from spec FR-006).

### Decision
**Use a string constant class with predefined values**

### Rationale
1. **Type Safety**: Constants provide compile-time checking while allowing string storage
2. **Simplicity**: No need for enum-to-string conversion logic
3. **Flexibility**: Easy to add options later without database migrations (in-memory storage)
4. **Validation**: Can be validated using a list of valid values in the validator
5. **Display**: No mapping needed for UI display (strings are already display-ready)

### Implementation Pattern
```csharp
// CitytripPlanner.Features/UserProfiles/Domain/GenderOptions.cs
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

### Alternatives Considered
- **Enum**: Rejected because it requires ToString() conversion and less friendly for UI binding
- **Config file**: Rejected as over-engineering for 3 fixed values (violates YAGNI)
- **Database table**: Rejected (no database in current scope, violates simplicity)

### Best Practices Reference
- C# convention: Use constants for fixed string values that appear in code
- .NET pattern: Use static readonly collections for validation lists

---

## Research Task 2: Country List Source

### Context
The feature requires a country selection field (FR-007) from "a list of valid countries".

### Decision
**Use a static C# class with ISO 3166-1 country names**

### Rationale
1. **Simplicity (YAGNI)**: No external dependencies, no file I/O, no network calls
2. **Sufficient for MVP**: ISO standard provides all needed countries
3. **Testable**: Easy to mock or verify in unit tests
4. **In-Memory Alignment**: Consistent with existing in-memory storage pattern
5. **No Database**: Matches current architecture (no database in use)

### Implementation Pattern
```csharp
// CitytripPlanner.Features/UserProfiles/Domain/Countries.cs
public static class Countries
{
    public static readonly IReadOnlyList<string> All = new[]
    {
        "Afghanistan",
        "Albania",
        // ... (full ISO 3166-1 list, ~195 countries)
        "Zimbabwe"
    };
}
```

**Note**: Full list will include all 195 ISO 3166-1 recognized countries, sorted alphabetically.

### Alternatives Considered
- **NuGet library (CountryData, ISO3166)**: Rejected to avoid new dependencies (constitution constraint)
- **JSON resource file**: Rejected as unnecessary complexity for static data
- **External API**: Rejected (offline capability, network dependency, violates simplicity)
- **Enum with country codes**: Rejected (poor UX, requires mapping to display names)

### Best Practices Reference
- .NET pattern: Use static collections for reference data
- ISO 3166-1: International standard for country names
- Blazor pattern: Bind dropdown to IEnumerable<string> for simple selection

---

## Research Task 3: UserProfile Entity Design

### Context
Need to model user profile data separate from authentication/identity.

### Decision
**Create a new `UserProfile` entity separate from authentication**

### Rationale
1. **Separation of Concerns**: Authentication (ICurrentUserService) vs. Profile Data (UserProfile)
2. **Single Responsibility**: ICurrentUserService provides "who am I?", UserProfile provides "what's my info?"
3. **Existing Pattern**: ICurrentUserService is minimal (UserId, DisplayName only)
4. **Extensibility**: Profile can evolve independently of auth
5. **Constitution Compliance**: Follows clean architecture (domain models in Features project)

### Implementation Pattern
```csharp
// CitytripPlanner.Features/UserProfiles/Domain/UserProfile.cs
public class UserProfile
{
    public required string UserId { get; init; }  // Links to ICurrentUserService.UserId
    public required string Name { get; set; }      // Last name
    public required string Firstname { get; set; } // First name
    public string? Gender { get; set; }            // Optional: Male, Female, Prefer not to say
    public string? Country { get; set; }           // Optional: ISO country name

    // Validation is handled by UpdateUserProfileValidator
}
```

### Data Relationship
- `UserProfile.UserId` maps to `ICurrentUserService.UserId` (string identifier)
- One-to-one: Each UserId has zero or one UserProfile
- Repository keyed by UserId for lookups

### Alternatives Considered
- **Extend ICurrentUserService**: Rejected (violates single responsibility, would require infrastructure changes)
- **Add to existing User entity**: Rejected (no User entity exists, auth is abstracted via interface)
- **Embed in authentication**: Rejected (tight coupling, profile data is separate concern)

### Best Practices Reference
- Domain-Driven Design: User Profile is an Aggregate Root (entity with identity = UserId)
- Clean Architecture: Domain entities are independent of infrastructure

---

## Research Task 4: Shared Service Location

### Context
`ICurrentUserService` is currently in `CitytripPlanner.Features/Citytrips/Domain/` but is needed by UserProfiles feature.

### Decision
**Keep ICurrentUserService in Citytrips domain; UserProfiles references it directly**

### Rationale
1. **Constitution Compliance**: Avoid creating a 4th project (violates 3-project rule)
2. **Pragmatic**: Features can reference other features' interfaces when needed
3. **No Circular Dependencies**: UserProfiles → Citytrips (interface only), no cycle
4. **YAGNI**: Don't create shared folder until 3+ features need the same interface
5. **Future Path**: If more features need ICurrentUserService, refactor then (YAGNI)

### Implementation
- UserProfiles handlers inject `ICurrentUserService` via constructor
- Reference: `using CitytripPlanner.Features.Citytrips.Domain;`
- No code duplication (interface lives in one place)

### Alternatives Considered
- **Create Shared/Common folder in Features**: Acceptable, but wait until 3rd feature needs it (YAGNI)
- **Create 4th "Common" project**: Rejected (violates constitution 3-project limit)
- **Duplicate interface**: Rejected (DRY violation, maintenance burden)
- **Move to Infrastructure**: Rejected (interfaces belong in Features/domain layer)

### Refactoring Trigger
When a 3rd feature domain needs ICurrentUserService:
1. Create `CitytripPlanner.Features/Shared/` folder
2. Move `ICurrentUserService` there
3. Update usings in Citytrips and UserProfiles

### Best Practices Reference
- Clean Architecture: Cross-feature dependencies are acceptable for stable interfaces
- Constitution: Prefer simplicity now, refactor when needed (not before)
- Pragmatic rule: Refactor on the 3rd usage, not the 2nd (avoid premature abstraction)

---

## Summary of Decisions

| Clarification | Decision | Impact |
|---------------|----------|--------|
| Gender options | Static constants class | Simple, type-safe, easy validation |
| Country list | Static C# list with ISO 3166-1 names | No dependencies, sufficient for MVP |
| UserProfile entity | New entity separate from auth | Clean separation, follows SRP |
| ICurrentUserService location | Keep in Citytrips, reference directly | Avoid 4th project, refactor later if needed |

---

## Architecture Validation

**Constitution Re-Check**:
- ✅ Principle I (CQRS Vertical Slices): UserProfiles is new feature domain
- ✅ Principle II (TDD): Research complete, ready for test-first implementation
- ✅ Principle III (3 Projects): No new projects added
- ✅ Principle IV (YAGNI): All decisions favor simplicity over future speculation

**GATE RESULT**: ✅ PASS - Ready for Phase 1 (Design & Contracts)

---

## Next Steps

1. **Phase 1**: Create data-model.md (UserProfile entity details, validation rules)
2. **Phase 1**: Document MediatR contracts (command/query signatures)
3. **Phase 1**: Create quickstart.md (developer onboarding)
4. **Phase 1**: Update agent context with new patterns
5. **Phase 2**: Generate tasks.md via `/speckit.tasks` command
