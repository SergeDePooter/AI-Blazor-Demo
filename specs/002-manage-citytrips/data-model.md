# Data Model: Manage Citytrips

**Feature**: 002-manage-citytrips
**Date**: 2026-02-07

## Entities

### Citytrip (MODIFIED)

Evolves the existing `Citytrip` record to support management fields.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | int | Yes | Auto-generated, unique identifier |
| Title | string | Yes | Display name of the trip (was `CityName`) |
| Destination | string | Yes | City/location of the trip |
| ImageUrl | string | No | Cover image URL (existing field, kept for browse page) |
| Description | string? | No | Optional trip description (already exists) |
| StartDate | DateOnly | Yes | Trip start date (replaces `DurationInDays`) |
| EndDate | DateOnly | Yes | Trip end date (replaces `DurationInDays`) |
| MaxParticipants | int? | No | Optional participant limit |
| CreatorId | string | Yes | ID of the user who created the trip |

**Validation rules**:
- Title: non-empty, max 100 characters
- Destination: non-empty, max 100 characters
- StartDate: must not be in the past (on create only)
- EndDate: must be >= StartDate
- MaxParticipants: if provided, must be >= 1
- CreatorId: non-empty

**Migration from existing model**:
- `CityName` → split into `Title` (for display) and `Destination` (for location). For seed data, both can be set to the city name.
- `DurationInDays` → replaced by `StartDate` / `EndDate`. Seed data gets reasonable default dates.
- `CreatorId` → new field, seed data assigned to a default user.

### UserTripInteraction (UNCHANGED)

| Field | Type | Notes |
|-------|------|-------|
| CitytripId | int | Reference to Citytrip |
| IsLiked | bool | Whether current user liked the trip |
| IsEnlisted | bool | Whether current user enlisted in the trip |

### CurrentUser (NEW - Service concept, not persisted entity)

| Field | Type | Notes |
|-------|------|-------|
| UserId | string | Unique identifier for the current user |
| DisplayName | string | Name shown in UI |

Not a persisted entity — provided by `ICurrentUserService`. Hardcoded for demo.

## Relationships

```
User (1) ──creates──> (*) Citytrip
User (1) ──enlists──> (*) Citytrip (via UserTripInteraction.IsEnlisted)
User (1) ──likes────> (*) Citytrip (via UserTripInteraction.IsLiked)
```

- A Citytrip has exactly one creator (CreatorId)
- A User can enlist in many Citytrips (not their own)
- A User can like many Citytrips
- Deleting a Citytrip removes it from all users' enlisted lists (cascade via in-memory cleanup)

## Repository Interface Changes

```
ICitytripRepository:
  EXISTING:
    - GetAll(): List<Citytrip>
    - GetById(int id): Citytrip?

  NEW:
    - Add(Citytrip trip): Citytrip          # Returns created trip with generated Id
    - Update(Citytrip trip): Citytrip       # Returns updated trip
    - Delete(int id): bool                  # Returns true if found and deleted
    - GetByCreator(string creatorId): List<Citytrip>
    - GetEnlistedByUser(string userId): List<Citytrip>  # Requires cross-ref with UserInteractionStore
```

Note: `GetEnlistedByUser` needs access to both the trip repository and the interaction store. This will be handled in the query handler by composing both services, keeping the repository focused on trip data only. The `GetEnlistedByUser` method will instead be implemented in the handler by querying interactions then fetching matching trips.

Revised repository additions:
```
  NEW:
    - Add(Citytrip trip): Citytrip
    - Update(Citytrip trip): Citytrip
    - Delete(int id): bool
    - GetByCreator(string creatorId): List<Citytrip>
```
