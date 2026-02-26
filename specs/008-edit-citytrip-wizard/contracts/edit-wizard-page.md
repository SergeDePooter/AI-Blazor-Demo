# Contract: EditCitytrip.razor Page

**Route**: `@page "/citytrips/edit/{Id:int}"`
**File**: `CitytripPlanner.Web/Components/Pages/EditCitytrip.razor`

---

## Page Lifecycle

```text
OnInitializedAsync:
  1. Send GetCitytripDetailQuery(Id) via Mediator
  2. If result is null → _notFound = true; return
  3. If result.CreatorId != CurrentUser.UserId → _unauthorized = true; return
  4. Map result → _step1Model (WizardStep1Model)
  5. Map result.DayPlans → _dayPlans (List<DayPlanInputModel>)
     If result.DayPlans is null/empty → generate empty day slots from date range
```

---

## Markup Structure

Identical step-indicator + wizard-body structure to `CreateCitytrip.razor`:

```razor
@page "/citytrips/edit/{Id:int}"
@rendermode InteractiveServer

<PageTitle>Edit Citytrip</PageTitle>

@if (_notFound)      { <p>Trip not found.</p> return; }
@if (_unauthorized)  { <p>You are not authorised to edit this trip.</p> return; }
@if (_step1Model is null) { <p>Loading…</p> return; }

<div class="create-wizard-page">
    <!-- step indicator: 1. Basics | 2. Itinerary | 3. Review -->

    @if (_step == 1)
    {
        <WizardStep1 InitialModel="_step1Model" OnNext="HandleStep1Next" />
    }
    else if (_step == 2)
    {
        <WizardStep2 DayPlans="_dayPlans" OnBack="HandleStep2Back" OnNext="HandleStep2Next" />
    }
    else if (_step == 3)
    {
        <WizardStep3
            Model="_step1Model!"
            DayPlans="_dayPlans"
            OnBack="HandleStep3Back"
            OnConfirm="HandleConfirm"
            ConfirmLabel="Save Changes" />
    }
</div>
```

---

## Step-Navigation Handlers

### HandleStep1Next (async Task)

```text
Parameters: WizardStep1Model model

1. datesChanged = (_step1Model.StartDate != model.StartDate ||
                   _step1Model.EndDate   != model.EndDate)
2. hadEvents = _dayPlans.Any(dp => dp.Events.Count > 0)

3. If datesChanged AND hadEvents:
     confirmed = await JS.InvokeAsync<bool>("confirm",
         "Changing the dates will adjust the schedule. Continue?")
     If !confirmed: return (restore previous step1Model; do NOT update _step1Model)

4. If datesChanged (confirmed) OR _dayPlans.Count == 0:
     _dayPlans = PreserveByDayNumber(
         oldPlans: _dayPlans,
         newStart: model.StartDate,
         newEnd: model.EndDate)

5. _step1Model = model
6. _step = 2
```

### PreserveByDayNumber (static helper)

```text
Input: oldPlans (List<DayPlanInputModel>), newStart, newEnd
Output: List<DayPlanInputModel>

days = (newEnd - newStart).Days + 1
return Enumerable.Range(0, days).Select(i =>
  new DayPlanInputModel {
    DayNumber = i + 1,
    Date = newStart.AddDays(i),
    Events = (i + 1 <= oldPlans.Count)
      ? oldPlans[i].Events   // copy reference — same list
      : new List<ScheduledEventInputModel>()
  })
```

### HandleConfirm (async Task)

```text
1. Guard: if _isSaving || _step1Model is null → return
2. _isSaving = true; _errors.Clear()
3. Map _dayPlans → List<DayPlanInput> (same mapping as CreateCitytrip.razor)
4. Send UpdateTripCommand(
     TripId: Id,
     Title: _step1Model.Title,
     Destination: _step1Model.Destination,
     StartDate: _step1Model.StartDate,
     EndDate: _step1Model.EndDate,
     UserId: CurrentUser.UserId,
     Description: _step1Model.Description,
     MaxParticipants: _step1Model.MaxParticipants,
     ImageUrl: _step1Model.ImageUrl,
     DayPlans: dayPlanInputs)
5. Navigation.NavigateTo("/my-citytrips")
6. On exception: _errors.Add(ex.Message)
7. _isSaving = false
```

---

## MyCitytrips.razor Change

`OpenEditModal` is replaced by navigation:

```csharp
// BEFORE:
private void OpenEditModal(MyTripItem trip)
{
    _editingTrip = trip;
    _showFormModal = true;
}

// AFTER:
private void OpenEditModal(MyTripItem trip)
    => Navigation.NavigateTo($"/citytrips/edit/{trip.Id}");
```

The `TripFormModal` component reference in `MyCitytrips.razor` markup is removed.
The `_showFormModal` and `_editingTrip` fields are removed.

---

## WizardStep3 ConfirmLabel Parameter (optional enhancement)

To display "Save Changes" instead of the default "Confirm" button label in edit mode,
`WizardStep3.razor` gains an optional `[Parameter] public string ConfirmLabel`:

```csharp
[Parameter] public string ConfirmLabel { get; set; } = "Confirm";
```

This is backward-compatible — `CreateCitytrip.razor` does not set it.

---

## Test Scenarios (`EditCitytripPageTests.cs`)

| # | Scenario | Expected |
|---|----------|----------|
| 1 | Page renders with `Id` of existing own trip | WizardStep1 fields contain trip's title, destination, dates, image URL |
| 2 | Page renders with `Id` of existing own trip that has day plans | Step 1 pre-fills; after Next, Step 2 shows event rows for each day |
| 3 | Trip not found (Id = 999) | Shows "Trip not found." message |
| 4 | Trip belongs to another user | Shows unauthorised message |
| 5 | User changes dates and confirms warning | `_dayPlans` rebuilt; events on Day 1 preserved on new Day 1 |
| 6 | User changes dates and cancels warning | `_dayPlans` unchanged; `_step1Model` dates not updated |
| 7 | Reduce trip length (3 days → 2) | Day 3 events discarded; Day 1 and Day 2 events preserved |
| 8 | Confirm on Review step dispatches `UpdateTripCommand` (not `CreateTripCommand`) | Mediator receives `UpdateTripCommand` with correct TripId and all fields |
| 9 | Back navigation from Step 2 to Step 1 | Step 1 form retains previously entered values |
