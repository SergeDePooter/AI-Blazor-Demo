namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

public class LocationPickerModalTests : BunitContext
{
    // T033: LocationPickerModal renders map container or fallback based on JS result / MapLoadFailed

    [Fact]
    public void LocationPickerModal_ShowsFallback_WhenMapLoadFailed()
    {
        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.MapLoadFailed, true));

        cut.Find("div.map-fallback").Should().NotBeNull();
    }

    [Fact]
    public void LocationPickerModal_ShowsMapContainer_WhenJsInitSucceeds()
    {
        JSInterop.Setup<bool>("locationPicker.initPicker", _ => true).SetResult(true);
        JSInterop.SetupVoid("locationPicker.destroyPicker", _ => true);

        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.MapLoadFailed, false));

        // Map container is rendered when neither MapLoadFailed nor _initFailed is true
        cut.FindAll("div.picker-map-container").Should().NotBeEmpty();
    }

    [Fact]
    public void LocationPickerModal_ShowsFallback_WhenJsInitReturnsFalse()
    {
        JSInterop.Setup<bool>("locationPicker.initPicker", _ => true).SetResult(false);
        JSInterop.SetupVoid("locationPicker.destroyPicker", _ => true);

        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.MapLoadFailed, false));

        // After JS returns false, _initFailed = true → fallback shown
        cut.WaitForAssertion(() =>
            cut.Find("div.map-fallback").Should().NotBeNull());
    }

    // T034: LocationPickerModal fires OnConfirm(PlaceInput) and OnCancel callbacks

    [Fact]
    public void LocationPickerModal_FiresOnCancel_WhenCancelClicked()
    {
        var canceled = false;
        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.MapLoadFailed, true)
             .Add(c => c.OnCancel, EventCallback.Factory.Create(this, () => canceled = true)));

        cut.Find("button[id='btn-cancel']").Click();

        canceled.Should().BeTrue();
    }

    [Fact]
    public void LocationPickerModal_FiresOnConfirm_WithTypedPlaceName_WhenConfirmClicked()
    {
        PlaceInputModel? received = null;
        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.MapLoadFailed, true)
             .Add(c => c.OnConfirm, EventCallback.Factory.Create<PlaceInputModel>(this, m => received = m)));

        cut.Find("input[id='placeName']").Change("Shibuya Crossing");
        cut.Find("button[id='btn-confirm-location']").Click();

        received.Should().NotBeNull();
        received!.Name.Should().Be("Shibuya Crossing");
    }

    // T035: LocationPickerModal pre-populates from InitialLat/InitialLng/InitialName parameters

    [Fact]
    public void LocationPickerModal_PrePopulatesPlaceName_WhenInitialNameProvided()
    {
        JSInterop.Setup<bool>("locationPicker.initPicker", _ => true).SetResult(true);
        JSInterop.SetupVoid("locationPicker.destroyPicker", _ => true);

        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.InitialLat, 35.659)
             .Add(c => c.InitialLng, 139.700)
             .Add(c => c.InitialName, "Tokyo Tower"));

        var input = cut.Find("input[id='placeName']");
        input.GetAttribute("value").Should().Be("Tokyo Tower");
    }

    [Fact]
    public void LocationPickerModal_FiresOnConfirm_WithInitialCoords_WhenConfirmClicked()
    {
        JSInterop.Setup<bool>("locationPicker.initPicker", _ => true).SetResult(true);
        JSInterop.SetupVoid("locationPicker.destroyPicker", _ => true);

        PlaceInputModel? received = null;
        var cut = Render<LocationPickerModal>(p =>
            p.Add(c => c.InitialLat, 35.659)
             .Add(c => c.InitialLng, 139.700)
             .Add(c => c.InitialName, "Tokyo Tower")
             .Add(c => c.OnConfirm, EventCallback.Factory.Create<PlaceInputModel>(this, m => received = m)));

        cut.Find("button[id='btn-confirm-location']").Click();

        received.Should().NotBeNull();
        received!.Name.Should().Be("Tokyo Tower");
        received.Latitude.Should().Be(35.659);
        received.Longitude.Should().Be(139.700);
    }
}
