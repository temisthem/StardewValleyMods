using TemLib;

namespace CropWateringBubbles;

internal class Configuration : EmoteBubbleConfig {
    public bool OnlyWhenWatering { get; set; } = false;
    public bool IncludeGiantable { get; set; } = true;

    public Configuration() {
        SizePercent = 100;
        OffsetY = 0;
    }
}
