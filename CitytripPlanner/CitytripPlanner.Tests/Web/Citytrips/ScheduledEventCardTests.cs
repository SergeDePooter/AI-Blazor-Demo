namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;

public class ScheduledEventCardTests : BunitContext
{
    [Fact]
    public void ScheduledEventCard_RendersEventTypeAndName()
    {
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            null, null, null);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("Museum");
        cut.Markup.Should().Contain("Louvre");
    }

    [Fact]
    public void ScheduledEventCard_WithPlace_RendersLocationName()
    {
        var place = new PlaceDetail("Louvre Museum", 48.8606, 2.3376, null);
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            null, null, place);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("Louvre Museum");
    }

    [Fact]
    public void ScheduledEventCard_WithoutPlace_RendersNoLocationIcon()
    {
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            null, null, null);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().NotContain("event-location");
    }

    [Fact]
    public void ScheduledEventCard_RendersStartTime()
    {
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            null, null, null);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("14:00");
    }

    [Fact]
    public void ScheduledEventCard_WithEndTime_RendersEndTime()
    {
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            new TimeOnly(16, 0), null, null);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("16:00");
    }

    [Fact]
    public void ScheduledEventCard_WithoutEndTime_EndTimeNotRendered()
    {
        var ev = new ScheduledEventDetail("Museum", "Louvre", new TimeOnly(14, 0),
            null, null, null);

        var cut = Render<ScheduledEventCard>(p =>
            p.Add(c => c.Event, ev));

        cut.Markup.Should().Contain("14:00");
        cut.Markup.Should().NotContain("event-time-end");
    }
}
