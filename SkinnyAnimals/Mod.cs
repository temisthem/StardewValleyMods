using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkinnyAnimals;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;

    public override void Entry(IModHelper helper) {
        _config = Helper.ReadConfig<Configuration>();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.farmerPushing)),
            postfix: new HarmonyMethod(typeof(FarmAnimal_farmerPushing_Patch),
                nameof(FarmAnimal_farmerPushing_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => _config = new Configuration(),
            save: () => Helper.WriteConfig(_config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Enabled", 
            getValue: () => _config.Enabled,
            setValue: value => _config.Enabled = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Push Speed Multiplier", 
            getValue: () => _config.PushSpeedMultiplier,
            setValue: value => _config.PushSpeedMultiplier = value,
            tooltip: () => "Multiplies the speed of you pushing past animals, only applies when ignore collision is off",
            min: 1,
            max: 10
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Ignore Collision", 
            getValue: () => _config.IgnoreCollision,
            setValue: value => _config.IgnoreCollision = value
        );
    }
}