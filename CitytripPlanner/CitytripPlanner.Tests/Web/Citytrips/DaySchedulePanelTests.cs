namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;

public class DaySchedulePanelTests : BunitContext
{
    [Fact]
    public void DaySchedulePanel_RendersDayHeaders()
    {
        var dayPlans = new List<DayPlanDetail>
        {
            new(1, new DateOnly(2026, 6, 1), "Morning", new(), new()),
            new(2, new DateOnly(2026, 6, 2), "Afternoon", new(), new())
        };

        var cut = Render<DaySchedulePanel>(p =>
            p.Add(c => c.DayPlans, dayPlans));

        cut.Markup.Should().Contain("Day 1");
        cut.Markup.Should().Contain("Day 2");
    }

    [Fact]
    public void DaySchedulePanel_RendersDataDayAttributes()
    {
        var dayPlans = new List<DayPlanDetail>
        {
            new(1, new DateOnly(2026, 6, 1), "Morning", new(), new())
        };

        var cut = Render<DaySchedulePanel>(p =>
            p.Add(c => c.DayPlans, dayPlans));

        cut.Markup.Should().Contain("data-day=\"1\"");
    }

    [Fact]
    public void DaySchedulePanel_EmptyState_RendersNoEventsMessage()
    {
        var dayPlans = new List<DayPlanDetail>
        {
            new(1, new DateOnly(2026, 6, 1), "Morning", new(), new()),
            new(2, new DateOnly(2026, 6, 2), "Afternoon", new(), new())
        };

        var cut = Render<DaySchedulePanel>(p =>
            p.Add(c => c.DayPlans, dayPlans));

        cut.Markup.Should().Contain("No events");
    }

    [Fact]
    public void DaySchedulePanel_WithEvents_RendersEventCards()
    {
        var events = new List<ScheduledEventDetail>
        {
            new("Museum", "Louvre", new TimeOnly(10, 0), null, null, null)
        };
        var dayPlans = new List<DayPlanDetail>
        {
            new(1, new DateOnly(2026, 6, 1), "Morning", new(), events)
        };

        var cut = Render<DaySchedulePanel>(p =>
            p.Add(c => c.DayPlans, dayPlans));

        cut.Markup.Should().Contain("Louvre");
    }
}
