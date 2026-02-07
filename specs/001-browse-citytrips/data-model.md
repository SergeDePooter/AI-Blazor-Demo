# Data Model: Browse Citytrips

**Feature Branch**: `001-browse-citytrips`
**Date**: 2026-02-06

## Entities

### Citytrip

Represents a bookable city trip destination.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | Required, unique | Unique trip identifier |
| CityName | string | Required, non-empty | Name of the destination city |
| ImageUrl | string | Required, valid URL | URL to the destination image |
| DurationInDays | int | Required, > 0 | Trip length in days |
| Description | string | Optional | Short description of the trip |

**Location**: `CitytripPlanner.Features/Citytrips/Domain/Citytrip.cs`

**Notes**:
- Immutable record type. No state transitions.
- Seed data provided via in-memory collection.
- No database mapping needed for this feature.

### UserTripInteraction

Tracks per-session like and enlistment state for a user
on a specific citytrip.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| CitytripId | int | Required | References Citytrip.Id |
| IsLiked | bool | Default: false | Whether user liked this trip |
| IsEnlisted | bool | Default: false | Whether user enlisted for this trip |

**Location**: `CitytripPlanner.Features/Citytrips/Domain/UserTripInteraction.cs`

**Notes**:
- Scoped per Blazor Server circuit (session-lifetime).
- Stored in a scoped `Dictionary<int, UserTripInteraction>`.
- No persistence beyond the session.

## Relationships

```
Citytrip (1) ←——→ (0..1) UserTripInteraction
```

- Each Citytrip may have zero or one interaction record
  per session.
- Interaction is created lazily on first like or enlist.

## Query/Command Responses

### CitytripCard (Read DTO)

Combines trip data with session interaction state for
display.

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Trip identifier |
| CityName | string | City name |
| ImageUrl | string | Image URL |
| DurationInDays | int | Duration in days |
| IsLiked | bool | Current session like state |
| IsEnlisted | bool | Current session enlist state |

**Location**: `CitytripPlanner.Features/Citytrips/ListCitytrips/CitytripCard.cs`

## Seed Data

Initial citytrips are hardcoded in a seed data provider
within Infrastructure. Example entries:

- Paris, 5 days
- Barcelona, 4 days
- Rome, 3 days
- Amsterdam, 2 days
- Prague, 4 days
- Lisbon, 5 days
