using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace FertilizerBubbles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool DisplayWhenHeld { get; set; } = true;
    public KeybindList ToggleEmoteKey { get; set; } = new(new Keybind(SButton.None));
    public bool HideWhenUnusable { get; set; } = true;

    public int OffsetY { get; set; } = 0;
    public int OffsetX { get; set; } = 0;
    public int EmoteInterval { get; set; } = 250;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}