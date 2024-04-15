namespace EmptyJarBubbles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool JarsEnabled { get; set; } = true;
    public bool KegsEnabled { get; set; } = true;
    public bool CasksEnabled { get; set; } = true;

    public int OffsetY { get; set; } = 80;

    public int EmoteInterval { get; set; } = 250;
    
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}