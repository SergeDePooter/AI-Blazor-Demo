namespace CitytripPlanner.Tests.Web.Citytrips;

using Bunit;
using CitytripPlanner.Features.Citytrips.GetCitytripDetail;
using CitytripPlanner.Web.Components.Citytrips;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

public class TripMapSidebarTests : BunitContext
{
    [Fact]
    public void TripMapSidebar_WhenMapsLoads_RendersMapContainer()
    {
        Services.AddSingleton<IJSRuntime>(new FakeJSRuntime(initSuccess: true));

        var cut = Render<TripMapSidebar>(p =>
            p.Add(c => c.Markers, new List<PlaceDetail>()));

        cut.Find("[id^='trip-map']").Should().NotBeNull();
    }

    [Fact]
    public void TripMapSidebar_WhenMapLoadFailed_RendersFallbackMessage()
    {
        Services.AddSingleton<IJSRuntime>(new FakeJSRuntime(initSuccess: false));

        var cut = Render<TripMapSidebar>(p =>
        {
            p.Add(c => c.Markers, new List<PlaceDetail>());
            p.Add(c => c.MapLoadFailed, true);
        });

        cut.Markup.Should().Contain("map-fallback");
    }

    [Fact]
    public void TripMapSidebar_WhenInitFails_ShowsFallback()
    {
        Services.AddSingleton<IJSRuntime>(new FakeJSRuntime(initSuccess: false));

        var cut = Render<TripMapSidebar>(p =>
        {
            p.Add(c => c.Markers, new List<PlaceDetail>());
            p.Add(c => c.MapLoadFailed, false);
        });

        // initMap returned false → _initFailed = true → fallback shown
        cut.Markup.Should().Contain("map-fallback");
    }

    private class FakeJSRuntime : IJSRuntime
    {
        private readonly bool _initSuccess;

        public FakeJSRuntime(bool initSuccess) => _initSuccess = initSuccess;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            if (typeof(TValue) == typeof(bool))
                return ValueTask.FromResult((TValue)(object)_initSuccess);
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (typeof(TValue) == typeof(bool))
                return ValueTask.FromResult((TValue)(object)_initSuccess);
            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
