namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

public class WizardStep1Tests : BunitContext
{
    [Fact]
    public void WizardStep1_RendersAllBasicFields()
    {
        var cut = Render<WizardStep1>();

        cut.Find("input[id='title']").Should().NotBeNull();
        cut.Find("input[id='destination']").Should().NotBeNull();
        cut.Find("input[id='startDate']").Should().NotBeNull();
        cut.Find("input[id='endDate']").Should().NotBeNull();
        cut.Find("textarea[id='description']").Should().NotBeNull();
        cut.Find("input[id='maxParticipants']").Should().NotBeNull();
        cut.Find("input[id='imageUrl']").Should().NotBeNull();
    }

    [Fact]
    public void WizardStep1_ShowsValidationErrors_WhenRequiredFieldsMissing()
    {
        var onNextCalled = false;
        var cut = Render<WizardStep1>(p =>
            p.Add(c => c.OnNext, EventCallback.Factory.Create<WizardStep1Model>(this, _ => onNextCalled = true)));

        cut.Find("button[id='btn-next']").Click();

        onNextCalled.Should().BeFalse();
        cut.Markup.Should().Contain("required");
    }

    [Fact]
    public void WizardStep1_InvokesOnNext_WhenRequiredFieldsValid()
    {
        WizardStep1Model? received = null;
        var cut = Render<WizardStep1>(p =>
            p.Add(c => c.OnNext, EventCallback.Factory.Create<WizardStep1Model>(this, m => received = m)));

        cut.Find("input[id='title']").Change("Tokyo Adventure");
        cut.Find("input[id='destination']").Change("Tokyo");
        cut.Find("input[id='startDate']").Change("2026-09-01");
        cut.Find("input[id='endDate']").Change("2026-09-03");

        cut.Find("button[id='btn-next']").Click();

        received.Should().NotBeNull();
        received!.Title.Should().Be("Tokyo Adventure");
        received.Destination.Should().Be("Tokyo");
    }
}
