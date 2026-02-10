# Data Model: Filter Citytrips

## Entities

### CitytripFilterCriteria (NEW)

Represents the active filter state. This is a simple value object used client-side — not persisted.

| Field       | Type      | Required | Description                              |
|-------------|-----------|----------|------------------------------------------|
| SearchText  | string?   | No       | Text to match against title/destination  |
| FromDate    | DateOnly? | No       | Exclude trips ending before this date    |
| ToDate      | DateOnly? | No       | Exclude trips starting after this date   |

**Validation rules**:
- SearchText is trimmed; whitespace-only treated as null (no filter)
- If both FromDate and ToDate are set, FromDate must be <= ToDate (enforced by UI constraints)

### CitytripCard (EXISTING — no changes)

The filter operates on the existing `CitytripCard` DTO from the `ListCitytrips` slice.

| Field       | Type     | Filterable | Filter Type                 |
|-------------|----------|------------|-----------------------------|
| Id          | int      | No         | —                           |
| Title       | string   | Yes        | Case-insensitive contains   |
| Destination | string   | Yes        | Case-insensitive contains   |
| ImageUrl    | string   | No         | —                           |
| StartDate   | DateOnly | Yes        | Range overlap (ToDate)      |
| EndDate     | DateOnly | Yes        | Range overlap (FromDate)    |
| IsLiked     | bool     | No         | —                           |
| IsEnlisted  | bool     | No         | —                           |

## Relationships

```
CitytripFilterCriteria  ──applies to──>  List<CitytripCard>  ──produces──>  List<CitytripCard> (filtered)
```

No database changes. No new tables or columns. The filter criteria exists only as transient state in the Blazor component.

## Filter Logic

The `CitytripFilter.Apply()` method implements these rules:

1. **Text filter**: If SearchText is non-empty, keep trips where Title OR Destination contains the search text (case-insensitive, `OrdinalIgnoreCase`)
2. **From date filter**: If FromDate is set, keep trips where EndDate >= FromDate
3. **To date filter**: If ToDate is set, keep trips where StartDate <= ToDate
4. **Combination**: All active filters are ANDed together
5. **No filters active**: Return the original list unchanged
