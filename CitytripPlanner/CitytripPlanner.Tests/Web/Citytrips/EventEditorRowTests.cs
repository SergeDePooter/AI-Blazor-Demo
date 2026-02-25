namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

public class EventEditorRowTests : BunitContext
{
    private static ScheduledEventInputModel BasicEvent() => new()
    {
        EventType = "museum",
        Name = "Shibuya Visit",
        StartTime = new TimeOnly(10, 0)
    };

    // T022: EventEditorRow renders all input fields and remove button

    [Fact]
    public void EventEditorRow_RendersEventTypeInput()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("input[id='eventType']").Should().NotBeNull();
    }

    [Fact]
    public void EventEditorRow_RendersEventNameInput()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("input[id='eventName']").Should().NotBeNull();
    }

    [Fact]
    public void EventEditorRow_RendersStartTimeInput()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("input[id='startTime']").Should().NotBeNull();
    }

    [Fact]
    public void EventEditorRow_RendersEndTimeInput()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("input[id='endTime']").Should().NotBeNull();
    }

    [Fact]
    public void EventEditorRow_RendersDescriptionInput()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("input[id='description']").Should().NotBeNull();
    }

    [Fact]
    public void EventEditorRow_RendersRemoveButton()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Find("button[id='btn-remove']").Should().NotBeNull();
    }

    // T023: EventEditorRow fires callbacks and shows end-time error

    [Fact]
    public void EventEditorRow_ShowsEndTimeError_WhenEndTimeBeforeStartTime()
    {
        var ev = new ScheduledEventInputModel
        {
            EventType = "museum",
            Name = "Test",
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(9, 0)
        };

        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("End time must be after start time");
    }

    [Fact]
    public void EventEditorRow_DoesNotShowEndTimeError_WhenNoEndTime()
    {
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent()));

        cut.Markup.Should().NotContain("End time must be after start time");
    }

    [Fact]
    public void EventEditorRow_FiresOnRemove_WhenRemoveClicked()
    {
        var removed = false;
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent())
             .Add(c => c.OnRemove, EventCallback.Factory.Create(this, () => removed = true)));

        cut.Find("button[id='btn-remove']").Click();

        removed.Should().BeTrue();
    }

    [Fact]
    public void EventEditorRow_FiresOnPickLocation_WhenPickMapClicked()
    {
        var picked = false;
        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, BasicEvent())
             .Add(c => c.OnPickLocation, EventCallback.Factory.Create(this, () => picked = true)));

        cut.Find("button[id='btn-pick-map']").Click();

        picked.Should().BeTrue();
    }

    [Fact]
    public void EventEditorRow_ShowsPlaceBadge_WhenEventHasPlace()
    {
        var ev = BasicEvent();
        ev.Place = new PlaceInputModel { Name = "Shibuya Crossing", Latitude = 35.659, Longitude = 139.700 };

        var cut = Render<EventEditorRow>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("Shibuya Crossing");
    }
}
