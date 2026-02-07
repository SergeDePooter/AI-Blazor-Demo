# CQRS Contracts: Browse Citytrips

**Feature Branch**: `001-browse-citytrips`
**Date**: 2026-02-06

## Queries

### ListCitytripsQuery

Returns all available citytrips with the current session's
interaction state (liked/enlisted).

**Request**: `ListCitytripsQuery : IRequest<List<CitytripCard>>`
- No parameters (returns all trips)

**Response**: `List<CitytripCard>`
- See data-model.md for CitytripCard fields

**Handler location**:
`CitytripPlanner.Features/Citytrips/ListCitytrips/ListCitytripsHandler.cs`

**Behavior**:
1. Retrieve all citytrips from the data source
2. Merge with session interaction state
3. Return list of CitytripCard DTOs

---

## Commands

### ToggleLikeCommand

Toggles the like state for a citytrip in the current session.

**Request**: `ToggleLikeCommand : IRequest<bool>`
- `CitytripId` (int, required) — ID of the trip to toggle

**Response**: `bool` — new IsLiked state after toggle

**Handler location**:
`CitytripPlanner.Features/Citytrips/ToggleLike/ToggleLikeHandler.cs`

**Behavior**:
1. Look up or create UserTripInteraction for CitytripId
2. Toggle IsLiked
3. Return new IsLiked value

**Validation**:
- CitytripId MUST reference an existing citytrip

---

### EnlistCommand

Enlists the current user for a citytrip.

**Request**: `EnlistCommand : IRequest<bool>`
- `CitytripId` (int, required) — ID of the trip to enlist for

**Response**: `bool` — new IsEnlisted state (true if newly
enlisted, unchanged if already enlisted)

**Handler location**:
`CitytripPlanner.Features/Citytrips/Enlist/EnlistHandler.cs`

**Behavior**:
1. Look up or create UserTripInteraction for CitytripId
2. Set IsEnlisted = true
3. Return IsEnlisted value

**Validation**:
- CitytripId MUST reference an existing citytrip

**Side effects**:
- Triggers toast notification on the UI (handled by the
  Blazor component, not the handler)

---

## Interfaces (Features → Infrastructure boundary)

### ICitytripRepository

Defined in Features, implemented in Infrastructure.

```
ICitytripRepository
├── GetAllAsync() → Task<List<Citytrip>>
└── GetByIdAsync(int id) → Task<Citytrip?>
```

**Location (interface)**:
`CitytripPlanner.Features/Citytrips/Domain/ICitytripRepository.cs`

**Location (implementation)**:
`CitytripPlanner.Infrastructure/Citytrips/InMemoryCitytripRepository.cs`

### IUserInteractionStore

Defined in Features, implemented as scoped service.

```
IUserInteractionStore
├── GetInteraction(int citytripId) → UserTripInteraction
├── GetAllInteractions() → Dictionary<int, UserTripInteraction>
└── SaveInteraction(int citytripId, UserTripInteraction) → void
```

**Location (interface)**:
`CitytripPlanner.Features/Citytrips/Domain/IUserInteractionStore.cs`

**Location (implementation)**:
`CitytripPlanner.Infrastructure/Citytrips/InMemoryUserInteractionStore.cs`

---

## Vertical Slice Folder Structure

```
CitytripPlanner.Features/
└── Citytrips/
    ├── Domain/
    │   ├── Citytrip.cs
    │   ├── UserTripInteraction.cs
    │   ├── ICitytripRepository.cs
    │   └── IUserInteractionStore.cs
    ├── ListCitytrips/
    │   ├── ListCitytripsQuery.cs
    │   ├── ListCitytripsHandler.cs
    │   └── CitytripCard.cs
    ├── ToggleLike/
    │   ├── ToggleLikeCommand.cs
    │   └── ToggleLikeHandler.cs
    └── Enlist/
        ├── EnlistCommand.cs
        └── EnlistHandler.cs

CitytripPlanner.Infrastructure/
└── Citytrips/
    ├── InMemoryCitytripRepository.cs
    └── InMemoryUserInteractionStore.cs
```
