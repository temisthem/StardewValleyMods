namespace TemLib;

public class BubbleConfig {
    public bool Enabled { get; set; } = true;
    public int OffsetX { get; set; } = 0;
    public int OffsetY { get; set; } = 0;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;

    public float Opacity => OpacityPercent / 100f;
    public float Scale => 4f * SizePercent / 100f;
    public float MovePercent => (100 - SizePercent) / 100f;
}
