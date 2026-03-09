using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace BetterTruffles;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;
    private static IModHelper _modHelper;
    private static IBetterPigsApi _betterPigsApi;

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
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
            prefix: new HarmonyMethod(typeof(FarmAnimal_behaviors_Patch), 
                nameof(FarmAnimal_behaviors_Patch.Prefix))
        );
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.draw), new [] {
                typeof(SpriteBatch),
                typeof(int),
                typeof(int),
                typeof(float)
            }),
            postfix: new HarmonyMethod(typeof(Object_draw_Patch), nameof(Object_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = _modHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
        _betterPigsApi = _modHelper.ModRegistry.GetApi<IBetterPigsApi>("MindMeltMax.WinterPigs");
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
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.PigsDigInGrass,
            getValue: () => _config.PigsDigInGrass,
            setValue: value => _config.PigsDigInGrass = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.PigsDigInFlooring,
            getValue: () => _config.PigsDigInFlooring,
            setValue: value => _config.PigsDigInFlooring = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ShowBubbles,
            getValue: () => _config.ShowBubbles,
            setValue: value => _config.ShowBubbles = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RenderOnTop,
            getValue: () => _config.RenderOnTop,
            setValue: value => _config.RenderOnTop = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleYOffset,
            getValue: () => _config.OffsetY,
            setValue: value => _config.OffsetY = value,
            min: -128,
            max: 128
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleXOffset,
            getValue: () => _config.OffsetX,
            setValue: value => _config.OffsetX = value,
            min: -128,
            max: 128
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Opacity,
            getValue: () => _config.OpacityPercent,
            setValue: value => _config.OpacityPercent = value,
            min: 1,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleSize,
            getValue: () => _config.SizePercent,
            setValue: value => _config.SizePercent = value,
            min: 1,
            max: 100
        );
    }
}