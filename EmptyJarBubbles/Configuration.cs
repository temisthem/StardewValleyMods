namespace EmptyJarBubbles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool JarsEnabled { get; set; } = true;
    public bool KegsEnabled { get; set; } = true;
    public bool CasksEnabled { get; set; } = true;
    public bool MayonnaiseMachineEnabled { get; set; } = false;
    public bool CheesePressEnabled { get; set; } = false;
    public bool LoomEnabled { get; set; } = false;
    public bool OilMakerEnabled { get; set; } = false;
    public bool DehydratorEnabled { get; set; } = false;
    public bool FishSmokerEnabled { get; set; } = false;
    public int OffsetY { get; set; } = 80;
    public int EmoteInterval { get; set; } = 250;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}