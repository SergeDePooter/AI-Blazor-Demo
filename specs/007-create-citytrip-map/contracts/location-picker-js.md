# Contract: Location Picker JavaScript Module

**Feature**: 007-create-citytrip-map
**File**: `CitytripPlanner.Web/wwwroot/js/location-picker.js`
**Pattern**: IIFE module with `DotNetObjectReference` callback (same as `trip-map.js` `observeDaySections`)

---

## Module Interface

```javascript
window.locationPicker = (function () {
    function initPicker(elementId, dotNetRef, lat, lng) { ... }
    function destroyPicker(elementId) { ... }
    return { initPicker, destroyPicker };
})();
```

---

## `initPicker(elementId, dotNetRef, lat, lng)`

**Purpose**: Initialise a Google Maps map in the given element that responds to click events to place a single draggable pin.

**Parameters**:
| Name | Type | Description |
|---|---|---|
| elementId | string | DOM id of the map container div |
| dotNetRef | DotNetObjectReference | .NET component reference for callbacks |
| lat | number or null | Pre-existing latitude (show existing pin if provided) |
| lng | number or null | Pre-existing longitude (show existing pin if provided) |

**Behaviour**:
1. Find `document.getElementById(elementId)`. If not found or `window.google` is unavailable, return `false`.
2. Initialise a `google.maps.Map` centred on `{lat, lng}` if provided, otherwise default to `{lat: 48.8566, lng: 2.3522}` (Paris) at zoom 5.
3. If `lat` and `lng` are provided, place a draggable `google.maps.Marker` at the coordinates.
4. Add a `map.addListener('click', ...)` handler:
   - Place (or move) a draggable marker at the clicked `LatLng`.
   - Attempt reverse geocoding via `google.maps.Geocoder` — if successful, call `dotNetRef.invokeMethodAsync('OnLocationPicked', lat, lng, geocodedName)`.
   - If geocoding fails, call `dotNetRef.invokeMethodAsync('OnLocationPicked', lat, lng, '')` with empty name.
5. If marker is dragged (`marker.addListener('dragend', ...)`), same callback with updated position.
6. Returns `true` on success.

**Returns**: `boolean` — `true` on success, `false` if map could not initialise.

---

## `destroyPicker(elementId)`

**Purpose**: Clean up map instance and listeners to prevent memory leaks when the modal is closed.

**Behaviour**:
1. Remove the marker from the map.
2. Set internal `map` and `marker` variables to `null`.

---

## .NET Callback

The Blazor component hosting the picker MUST expose a `[JSInvokable]` method:

```csharp
[JSInvokable]
public void OnLocationPicked(double latitude, double longitude, string geocodedName)
```

Called by JS after each pin placement or drag. The component updates its `_pickedLat`, `_pickedLng`, and `_pickedName` state variables and calls `StateHasChanged()`.

---

## Test Scenarios (for `LocationPickerModalTests.cs` bUnit tests)

| Scenario | Input | Expected |
|---|---|---|
| Map unavailable | FakeJSRuntime returns false | Fallback message shown; "Save" still enabled |
| Map available, no prior pin | initPicker returns true | Map container rendered with no pre-filled coords |
| Existing pin provided | lat/lng params | `initPicker` called with lat/lng; fields pre-filled |
| OnLocationPicked called | lat=48.85, lng=2.29, name="Eiffel Tower" | Place name field updates; coords stored |
| OnLocationPicked with empty name | name="" | Place name field is empty (user must type) |
| Confirm clicked | valid lat/lng/name | `OnConfirm` callback fires with PlaceInput |
| Cancel clicked | — | `OnCancel` fires; no place input returned |
