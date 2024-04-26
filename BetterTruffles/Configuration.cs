namespace BetterTruffles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool PigsDigInGrass {get; set;} = true;
    public bool ShowBubbles { get; set; } = true;
    public int OffsetY { get; set; } = 0;
    public int OffsetX { get; set; } = 0;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}