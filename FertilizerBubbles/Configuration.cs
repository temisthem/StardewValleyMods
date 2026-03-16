using TemLib;

namespace FertilizerBubbles;

internal class Configuration : EmoteBubbleConfig {
    public bool DisplayBubbleForFertilizers { get; set; } = true;
    public bool DisplayBubbleForSeeds { get; set; } = false;
    public bool DisplayWhenHeld { get; set; } = true;
    public bool HideWhenNoCrop { get; set; } = false;
}
