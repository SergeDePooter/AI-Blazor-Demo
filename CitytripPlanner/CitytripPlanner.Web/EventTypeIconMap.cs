namespace CitytripPlanner.Web;

public static class EventTypeIconMap
{
    private static readonly Dictionary<string, string> _icons =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["museum"]      = "🏛",
            ["market"]      = "🛒",
            ["stadium"]     = "🏟",
            ["park"]        = "🌳",
            ["restaurant"]  = "🍽",
            ["church"]      = "⛪",
            ["cathedral"]   = "⛪",
            ["beach"]       = "🏖",
            ["gallery"]     = "🖼",
            ["landmark"]    = "🗺",
            ["castle"]      = "🏰",
            ["palace"]      = "🏰",
            ["zoo"]         = "🦁",
            ["aquarium"]    = "🐠",
            ["theater"]     = "🎭",
            ["theatre"]     = "🎭",
            ["concert"]     = "🎵",
            ["hotel"]       = "🏨",
            ["shopping"]    = "🛍",
            ["bar"]         = "🍺",
            ["cafe"]        = "☕",
            ["coffee"]      = "☕",
            ["viewpoint"]   = "🌅",
            ["garden"]      = "🌸",
            ["bridge"]      = "🌉",
            ["port"]        = "⛵",
            ["train"]       = "🚂",
        };

    public static string GetIcon(string eventType)
        => _icons.TryGetValue(eventType, out var icon) ? icon : "📍";
}
