using Bunit;
using CitytripPlanner.Features.Citytrips.Domain;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using CitytripPlanner.Features.Citytrips.UpdateTrip;
using CitytripPlanner.Infrastructure.Citytrips;
using CitytripPlanner.Web.Components.Pages;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Bunit.TestDoubles;

namespace CitytripPlanner.Tests.Web.Pages;

public class EditCitytripPageTests : BunitContext
{
    private readonly InMemoryCitytripRepository _repository;

    public EditCitytripPageTests()
    {
        _repository = new InMemoryCitytripRepository();

        Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetCitytripDetailHandler>();
            cfg.RegisterServicesFromAssemblyContaining<UpdateTripHandler>();
        });
        Services.AddSingleton<ICitytripRepository>(_repository);
        Services.AddSingleton<ICurrentUserService>(new FakeCurrentUserService("test-user"));
    }

    private async Task<Citytrip> CreateOwnedTrip(string? imageUrl = "https://example.com/img.jpg")
        => await _repository.AddAsync(new Citytrip(
            0, "My Test Trip", "Berlin", imageUrl ?? "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 3),
            "test-user", "A description", 5));

    private async Task<Citytrip> CreateTripWithEvents()
        => await _repository.AddAsync(new Citytrip(
            0, "Trip With Events", "Berlin", "",
            new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 3),
            "test-user",
            DayPlans: new List<DayPlan>
            {
                new DayPlan(1, new DateOnly(2026, 8, 1), "", new List<Attraction>(),
                    new List<ScheduledEvent> { new ScheduledEvent("museum", "Checkpoint Charlie", new TimeOnly(10, 0)) }),
                new DayPlan(2, new DateOnly(2026, 8, 2), "", new List<Attraction>()),
                new DayPlan(3, new DateOnly(2026, 8, 3), "", new List<Attraction>())
            }));

    [Fact]
    public async Task EditPage_LoadsTrip_PreFillsStep1Fields()
    {
        var trip = await CreateOwnedTrip();

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        cut.Find("input[id='title']").GetAttribute("value").Should().Be("My Test Trip");
        cut.Find("input[id='destination']").GetAttribute("value").Should().Be("Berlin");
        cut.Find("input[id='imageUrl']").GetAttribute("value").Should().Be("https://example.com/img.jpg");
    }

    [Fact]
    public async Task EditPage_NonExistentTrip_ShowsNotFound()
    {
        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, 9999));

        cut.Markup.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task EditPage_OtherUsersTrip_ShowsUnauthorized()
    {
        var otherUserTrip = await _repository.AddAsync(new Citytrip(
            0, "Other User Trip", "Paris", "",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 3),
            "other-user"));

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, otherUserTrip.Id));

        cut.Markup.Should().ContainEquivalentOf("not authorised");
    }

    [Fact]
    public async Task EditPage_Confirm_DispatchesUpdateTripCommand()
    {
        var trip = await CreateOwnedTrip();

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Step 1 → Step 2 (no date change, no events → no JS confirm)
        cut.Find("button[id='btn-next']").Click();

        // Step 2 → Step 3
        cut.Find("button[id='btn-next']").Click();

        // Confirm on Step 3
        cut.Find("button[id='btn-confirm']").Click();

        // Verify the trip was updated in the repository
        var updated = await _repository.GetByIdAsync(trip.Id);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("My Test Trip");
    }

    // T018 — US2: Step 2 pre-fills events from the existing trip

    [Fact]
    public async Task EditPage_LoadsTrip_PreFillsStep2Events()
    {
        var trip = await CreateTripWithEvents();

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Navigate to Step 2
        cut.Find("button[id='btn-next']").Click();

        // Event name from Day 1 should be pre-filled
        cut.Find("input[id='eventName']").GetAttribute("value").Should().Be("Checkpoint Charlie");
    }

    // T023 — US3: Date range change warning and day-number preservation

    [Fact]
    public async Task EditPage_DateChange_WithEvents_ShowsConfirmation()
    {
        var trip = await CreateTripWithEvents();
        JSInterop.Setup<bool>("confirm", _ => true);

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Change end date (extends the trip by 2 days)
        cut.Find("input[id='endDate']").Change("2026-08-05");
        cut.Find("button[id='btn-next']").Click();

        // JS confirm must have been called because the trip had events
        JSInterop.VerifyInvoke("confirm", calledTimes: 1);
    }

    [Fact]
    public async Task EditPage_DateChange_CancelRestoresOriginalDates()
    {
        var trip = await CreateTripWithEvents();
        JSInterop.Setup<bool>("confirm", _ => true).SetResult(false);  // returns false = cancel

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Change end date
        cut.Find("input[id='endDate']").Change("2026-08-05");
        cut.Find("button[id='btn-next']").Click();

        // After cancel, we should still be on Step 1 (title input is visible, not btn-back)
        cut.Find("input[id='title']").Should().NotBeNull();
        cut.FindAll("button[id='btn-back']").Should().BeEmpty();
    }

    [Fact]
    public async Task EditPage_DateChange_Confirm_PreservesEventsByDayNumber()
    {
        var trip = await CreateTripWithEvents();  // 3-day trip, Day 1 has "Checkpoint Charlie"
        JSInterop.Setup<bool>("confirm", _ => true).SetResult(true);

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Extend from 3 days to 4 days
        cut.Find("input[id='endDate']").Change("2026-08-04");
        cut.Find("button[id='btn-next']").Click();

        // Now on Step 2: should have 4 day sections
        cut.FindAll("section[data-day]").Count.Should().Be(4);

        // Day 1 event is preserved
        cut.Find("input[id='eventName']").GetAttribute("value").Should().Be("Checkpoint Charlie");
    }

    [Fact]
    public async Task EditPage_DateChange_ReduceLength_DiscardsTailDayEvents()
    {
        var trip = await CreateTripWithEvents();  // 3-day trip
        JSInterop.Setup<bool>("confirm", _ => true).SetResult(true);

        var cut = Render<EditCitytrip>(p => p.Add(c => c.Id, trip.Id));

        // Reduce from 3 days to 2 days
        cut.Find("input[id='endDate']").Change("2026-08-02");
        cut.Find("button[id='btn-next']").Click();

        // Now on Step 2: should have only 2 day sections
        cut.FindAll("section[data-day]").Count.Should().Be(2);
    }
}

internal class FakeCurrentUserService : ICurrentUserService
{
    public FakeCurrentUserService(string userId) => UserId = userId;
    public string UserId { get; }
    public string DisplayName => "Test User";
}
