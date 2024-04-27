using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace WheresMyHorse; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public KeybindList DoEmoteKey { get; set; } = new(new Keybind(SButton.C));

    public bool RenderOnTop { get; set; } = true;
    public bool DisableOnMount { get; set; } = true;
    public bool OnlyMyHorse { get; set; } = false;
    public int OffsetY { get; set; } = 0;
    public int OffsetX { get; set; } = 0;
    public int EmoteInterval { get; set; } = 250;
    public int OpacityPercent { get; set; } = 100;
    public int SizePercent { get; set; } = 100;
}