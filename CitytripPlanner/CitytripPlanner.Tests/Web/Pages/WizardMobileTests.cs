namespace CitytripPlanner.Tests.Web.Pages;

using Bunit;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Structural tests for the wizard step components.
/// Verifies that active steps render in DOM and navigation buttons are present —
/// the structural prerequisite for one-step-per-screen mobile CSS to work.
/// </summary>
public class WizardMobileTests : BunitContext
{
    // T026 — active wizard step is rendered in DOM
    [Fact]
    public void WizardStep1_ActiveStepIsRendered()
    {
        var cut = Render<WizardStep1>();

        cut.Find(".wizard-step-1").Should().NotBeNull();
    }

    // T026 — Next button is present on the active step
    [Fact]
    public void WizardStep1_NextButtonIsPresent()
    {
        var cut = Render<WizardStep1>();

        cut.Find("button#btn-next").Should().NotBeNull();
    }

    [Fact]
    public void WizardStep2_BackAndNextButtonsArePresent()
    {
        var cut = Render<WizardStep2>(p =>
            p.Add(c => c.DayPlans, new List<DayPlanInputModel>())
             .Add(c => c.OnBack, EventCallback.Factory.Create(this, () => { }))
             .Add(c => c.OnNext, EventCallback.Factory.Create(this, () => { })));

        cut.Find("button.btn-secondary").Should().NotBeNull();
        cut.Find("button.btn-primary").Should().NotBeNull();
    }
}
