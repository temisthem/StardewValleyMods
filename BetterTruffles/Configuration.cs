using TemLib;

namespace BetterTruffles;

internal class Configuration : BubbleConfig {
    public bool PigsDigInGrass { get; set; } = true;
    public bool PigsDigInFlooring { get; set; } = false;
    public bool ShowBubbles { get; set; } = true;
    public bool RenderOnTop { get; set; } = false;
}
