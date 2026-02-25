namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Features.Citytrips.CreateTrip;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

public class WizardStep3Tests : BunitContext
{
    private static WizardStep1Model BasicModel() => new()
    {
        Title = "Tokyo Adventure",
        Destination = "Tokyo",
        StartDate = new DateOnly(2026, 9, 1),
        EndDate = new DateOnly(2026, 9, 3),
        Description = "A fun trip",
        MaxParticipants = 4,
        ImageUrl = "https://example.com/tokyo.jpg"
    };

    [Fact]
    public void WizardStep3_RendersTripBasics()
    {
        var cut = Render<WizardStep3>(p =>
            p.Add(c => c.Model, BasicModel())
             .Add(c => c.DayPlans, new List<DayPlanInputModel>()));

        cut.Markup.Should().Contain("Tokyo Adventure");
        cut.Markup.Should().Contain("Tokyo");
        cut.Markup.Should().Contain("A fun trip");
        cut.Markup.Should().Contain("4");
    }

    [Fact]
    public void WizardStep3_RendersDayPlansSummary_WhenDayPlansPresent()
    {
        var dayPlans = new List<DayPlanInputModel>
        {
            new DayPlanInputModel
            {
                DayNumber = 1,
                Date = new DateOnly(2026, 9, 1),
                Events = new List<ScheduledEventInputModel>
                {
                    new ScheduledEventInputModel { EventType = "museum", Name = "Shibuya Visit", StartTime = new TimeOnly(10, 0) }
                }
            }
        };

        var cut = Render<WizardStep3>(p =>
            p.Add(c => c.Model, BasicModel())
             .Add(c => c.DayPlans, dayPlans));

        cut.Markup.Should().Contain("Shibuya Visit");
        cut.Markup.Should().Contain("museum");
    }

    [Fact]
    public void WizardStep3_FiresOnConfirm_WhenConfirmClicked()
    {
        var confirmed = false;
        var cut = Render<WizardStep3>(p =>
            p.Add(c => c.Model, BasicModel())
             .Add(c => c.DayPlans, new List<DayPlanInputModel>())
             .Add(c => c.OnConfirm, EventCallback.Factory.Create(this, () => confirmed = true)));

        cut.Find("button[id='btn-confirm']").Click();

        confirmed.Should().BeTrue();
    }

    [Fact]
    public void WizardStep3_FiresOnBack_WhenBackClicked()
    {
        var wentBack = false;
        var cut = Render<WizardStep3>(p =>
            p.Add(c => c.Model, BasicModel())
             .Add(c => c.DayPlans, new List<DayPlanInputModel>())
             .Add(c => c.OnBack, EventCallback.Factory.Create(this, () => wentBack = true)));

        cut.Find("button[id='btn-back']").Click();

        wentBack.Should().BeTrue();
    }
}
