using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterTruffles;

internal partial class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static IModHelper ModHelper;

    public override void Entry(IModHelper helper) {
        Config = helper.ReadConfig<Configuration>();
        ModHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
            prefix: new HarmonyMethod(typeof(Mod.FarmAnimal_behaviors_Patch),
                nameof(Mod.FarmAnimal_behaviors_Patch.Prefix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => ModHelper.WriteConfig(Config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
    }
}