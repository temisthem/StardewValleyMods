using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TemLib;

public class EmoteBubbleConfig : BubbleConfig {
    public KeybindList ToggleEmoteKey { get; set; } = new(new Keybind(SButton.None));
    public bool DefaultToggleOn { get; set; } = false;
    public int EmoteInterval { get; set; } = 250;
    public bool NoAnimation { get; set; } = false;
}
