# API Contracts: Manage Citytrips

**Feature**: 002-manage-citytrips
**Date**: 2026-02-07

This feature uses MediatR commands/queries dispatched from Blazor Server components (no HTTP API endpoints). The contracts below define the MediatR request/response shapes.

## Queries

### GetMyTripsQuery

Retrieves all citytrips created by the current user.

**Request**:
```
GetMyTripsQuery : IRequest<List<MyTripItem>>
  - CreatorId: string (required)
```

**Response** (`MyTripItem`):
```
MyTripItem (record)
  - Id: int
  - Title: string
  - Destination: string
  - ImageUrl: string?
  - StartDate: DateOnly
  - EndDate: DateOnly
  - Description: string?
  - MaxParticipants: int?
  - EnlistedCount: int
```

### GetEnlistedTripsQuery

Retrieves all citytrips the current user has enlisted in (but did not create).

**Request**:
```
GetEnlistedTripsQuery : IRequest<List<EnlistedTripItem>>
  - UserId: string (required)
```

**Response** (`EnlistedTripItem`):
```
EnlistedTripItem (record)
  - Id: int
  - Title: string
  - Destination: string
  - ImageUrl: string?
  - StartDate: DateOnly
  - EndDate: DateOnly
  - Description: string?
  - CreatorName: string
```

## Commands

### CreateTripCommand

Creates a new citytrip for the current user.

**Request**:
```
CreateTripCommand : IRequest<int>
  - Title: string (required, max 100)
  - Destination: string (required, max 100)
  - StartDate: DateOnly (required, >= today on create)
  - EndDate: DateOnly (required, >= StartDate)
  - Description: string? (optional)
  - MaxParticipants: int? (optional, >= 1 if provided)
  - CreatorId: string (required)
```

**Response**: `int` (the ID of the newly created trip)

**Validation errors**:
- Title empty → "Title is required"
- Destination empty → "Destination is required"
- EndDate < StartDate → "End date must be on or after start date"
- MaxParticipants < 1 → "Max participants must be at least 1"

### UpdateTripCommand

Updates an existing citytrip owned by the current user.

**Request**:
```
UpdateTripCommand : IRequest<bool>
  - TripId: int (required)
  - Title: string (required, max 100)
  - Destination: string (required, max 100)
  - StartDate: DateOnly (required)
  - EndDate: DateOnly (required, >= StartDate)
  - Description: string? (optional)
  - MaxParticipants: int? (optional, >= 1 if provided)
  - UserId: string (required, must match CreatorId)
```

**Response**: `bool` (true if updated successfully)

**Validation errors**:
- Same as CreateTripCommand
- Trip not found → ArgumentException
- UserId != CreatorId → UnauthorizedAccessException

### DeleteTripCommand

Deletes a citytrip owned by the current user.

**Request**:
```
DeleteTripCommand : IRequest<bool>
  - TripId: int (required)
  - UserId: string (required, must match CreatorId)
```

**Response**: `bool` (true if deleted successfully)

**Error cases**:
- Trip not found → ArgumentException
- UserId != CreatorId → UnauthorizedAccessException
