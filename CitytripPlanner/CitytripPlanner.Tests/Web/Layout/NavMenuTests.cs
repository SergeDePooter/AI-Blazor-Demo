namespace CitytripPlanner.Tests.Web.Layout;

using Bunit;
using CitytripPlanner.Web.Components.Layout;
using FluentAssertions;

/// <summary>
/// TDD tests for NavMenu hamburger toggle behaviour.
/// T019–T022: These tests FAIL before implementing the hamburger toggle in NavMenu.razor.
/// </summary>
public class NavMenuTests : BunitContext
{
    // T019 — hamburger button must be present in rendered markup
    [Fact]
    public void NavMenu_RendersMobileToggleButton()
    {
        var cut = Render<NavMenu>();

        cut.Find(".nav-toggle").Should().NotBeNull();
    }

    // T020 — hamburger button must have aria-expanded="false" by default
    [Fact]
    public void NavMenu_ToggleButtonHasAriaExpandedFalseByDefault()
    {
        var cut = Render<NavMenu>();

        var toggle = cut.Find(".nav-toggle");
        toggle.GetAttribute("aria-expanded").Should().Be("False");
    }

    // T021 — clicking toggle once sets aria-expanded="true" and applies open CSS class
    [Fact]
    public void NavMenu_ClickingToggleOpensMenu()
    {
        var cut = Render<NavMenu>();

        cut.Find(".nav-toggle").Click();

        cut.Find(".nav-toggle").GetAttribute("aria-expanded").Should().Be("True");
        cut.Find(".nav-menu--open").Should().NotBeNull();
    }

    // T022 — clicking toggle twice closes menu (aria-expanded back to "false")
    [Fact]
    public void NavMenu_ClickingToggleAgainClosesMenu()
    {
        var cut = Render<NavMenu>();

        cut.Find(".nav-toggle").Click();
        cut.Find(".nav-toggle").Click();

        cut.Find(".nav-toggle").GetAttribute("aria-expanded").Should().Be("False");
        cut.FindAll(".nav-menu--open").Should().BeEmpty();
    }
}
