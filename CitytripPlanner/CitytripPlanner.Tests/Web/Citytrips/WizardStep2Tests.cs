namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

public class WizardStep2Tests : BunitContext
{
    private static List<DayPlanInputModel> TwoDayPlans() =>
    [
        new DayPlanInputModel { DayNumber = 1, Date = new DateOnly(2026, 9, 1) },
        new DayPlanInputModel { DayNumber = 2, Date = new DateOnly(2026, 9, 2) }
    ];

    // T024: WizardStep2 renders one section per day with date header and Add Event button

    [Fact]
    public void WizardStep2_RendersOneSectionPerDay()
    {
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans()));

        var sections = cut.FindAll("section[data-day]");
        sections.Count.Should().Be(2);
    }

    [Fact]
    public void WizardStep2_RendersDayNumberInSection()
    {
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans()));

        cut.Markup.Should().Contain("Day 1");
        cut.Markup.Should().Contain("Day 2");
    }

    [Fact]
    public void WizardStep2_RendersDateHeaderInSection()
    {
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans()));

        // DateOnly(2026, 9, 1) → Tuesday, Sept 1 (or similar locale-dependent format)
        cut.Markup.Should().Contain("Sept 1");
    }

    [Fact]
    public void WizardStep2_RendersAddEventButtonPerDay()
    {
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans()));

        var addButtons = cut.FindAll("button.btn-add-event");
        addButtons.Count.Should().Be(2);
    }

    // T025: WizardStep2 adds event row on Add Event click; fires Back/Next callbacks

    [Fact]
    public void WizardStep2_AddsEventRow_WhenAddEventClicked()
    {
        var plans = TwoDayPlans();
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, plans));

        var addButtons = cut.FindAll("button.btn-add-event");
        addButtons[0].Click();

        plans[0].Events.Should().HaveCount(1);
    }

    [Fact]
    public void WizardStep2_FiresOnBack_WhenBackClicked()
    {
        var wentBack = false;
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans())
             .Add(c => c.OnBack, EventCallback.Factory.Create(this, () => wentBack = true)));

        cut.Find("button[id='btn-back']").Click();

        wentBack.Should().BeTrue();
    }

    [Fact]
    public void WizardStep2_FiresOnNext_WhenNextClickedWithNoEvents()
    {
        var wentNext = false;
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, TwoDayPlans())
             .Add(c => c.OnNext, EventCallback.Factory.Create(this, () => wentNext = true)));

        cut.Find("button[id='btn-next']").Click();

        wentNext.Should().BeTrue();
    }

    [Fact]
    public void WizardStep2_DoesNotFireOnNext_WhenEventHasInvalidEndTime()
    {
        var plans = new List<DayPlanInputModel>
        {
            new DayPlanInputModel
            {
                DayNumber = 1,
                Date = new DateOnly(2026, 9, 1),
                Events = new List<ScheduledEventInputModel>
                {
                    new ScheduledEventInputModel
                    {
                        EventType = "museum",
                        Name = "Bad Event",
                        StartTime = new TimeOnly(12, 0),
                        EndTime = new TimeOnly(10, 0)  // end before start
                    }
                }
            }
        };

        var wentNext = false;
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, plans)
             .Add(c => c.OnNext, EventCallback.Factory.Create(this, () => wentNext = true)));

        cut.Find("button[id='btn-next']").Click();

        wentNext.Should().BeFalse();
        cut.Markup.Should().Contain("End time must be after start time");
    }
}
