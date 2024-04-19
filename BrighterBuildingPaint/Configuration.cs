namespace BrighterBuildingPaint; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public int MaxBrightness { get; set; } = 50;
    public int MinBrightness { get; set; } = -50;
    public int MaxSaturation { get; set; } = 100;
}