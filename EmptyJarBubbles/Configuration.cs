namespace EmptyJarBubbles; 

internal class Configuration {
    public bool Enabled { get; set; } = true;
    public bool JarsEnabled { get; set; } = true;
    public bool KegsEnabled { get; set; } = true;
    public bool CasksEnabled { get; set; } = true;
    public bool MayonnaiseMachinesEnabled { get; set; } = false;
    public bool CheesePressesEnabled { get; set; } = false;
    public bool LoomsEnabled { get; set; } = false;
    public bool OilMakersEnabled { get; set; } = false;
    public bool DehydratorsEnabled { get; set; } = false;
    public bool FishSmokersEnabled { get; set; } = false;
    public bool BaitMakersEnabled { get; set; } = false;
    public bool BoneMillsEnabled { get; set; } = false;
    public bool CharcoalKilnsEnabled { get; set; } = false;
    public bool CrystalariumsEnabled { get; set; } = false;
    public bool FurnacesEnabled { get; set; } = false;
    public bool RecyclingMachinesEnabled { get; set; } = false;
    public bool SeedMakersEnabled { get; set; } = false;
    public bool SlimeEggPressesEnabled { get; set; } = false;
    public bool CrabPotsEnabled { get; set; } = false;
    public bool DeconstructorsEnabled { get; set; } = false;
    public int OffsetY { get; set; } = 80;
    public int EmoteInterval { get; set; } = 250;
    public int OpacityPercent { get; set; } = 75;
    public int SizePercent { get; set; } = 75;
}