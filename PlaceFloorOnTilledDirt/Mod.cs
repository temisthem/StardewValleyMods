using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace PlaceFloorOnTilledDirt;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;
    private static IModHelper _modHelper;

    public override void Entry(IModHelper helper) {
        _config = helper.ReadConfig<Configuration>();
        _modHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }

    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
            postfix: new HarmonyMethod(typeof(Object_placementAction_Patch), nameof(Object_placementAction_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = _modHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => _config = new Configuration(),
            save: () => _modHelper.WriteConfig(_config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => _config.Enabled,
            setValue: value => _config.Enabled = value
        );
    }
}