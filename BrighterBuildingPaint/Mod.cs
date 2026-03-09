using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace BrighterBuildingPaint;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;

    public override void Entry(IModHelper helper) {
        _config = Helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Constructor(typeof(BuildingPaintMenu.BuildingColorSlider), new []{
                typeof(BuildingPaintMenu),
                typeof(int),
                typeof(Rectangle),
                typeof(int), 
                typeof(int),
                typeof(Action<int>)}),
            postfix: new HarmonyMethod(typeof(BuildingPaintMenu_BuildingColorSlider_Patch),
                nameof(BuildingPaintMenu_BuildingColorSlider_Patch.Postfix))
        );
        
        harmony.Patch(
            original: AccessTools.Method(typeof(BuildingPaintMenu.ColorSliderPanel), nameof(BuildingPaintMenu.ColorSliderPanel.Draw), new [] {
                typeof(SpriteBatch)
            }),
            postfix: new HarmonyMethod(typeof(BuildingPaintMenu_Draw_Patch),
                nameof(BuildingPaintMenu_Draw_Patch.Postfix))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(BuildingPaintMenu.ColorSliderPanel), nameof(BuildingPaintMenu.ColorSliderPanel.ReceiveLeftClick), new [] {
                typeof(int),
                typeof(int),
                typeof(bool)
            }),
            postfix: new HarmonyMethod(typeof(BuildingPaintMenu_ReceiveLeftClick_Patch),
                nameof(BuildingPaintMenu_ReceiveLeftClick_Patch.Postfix))
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
            name: I18n.Enabled,
            getValue: () => _config.Enabled,
            setValue: value => _config.Enabled = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MaxBrightness,
            getValue: () => _config.MaxBrightness,
            setValue: value => _config.MaxBrightness = value,
            min: 0,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MinBrightness,
            getValue: () => _config.MinBrightness,
            setValue: value => _config.MinBrightness = value,
            min: -100,
            max: 0
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MaxSaturation,
            getValue: () => _config.MaxSaturation,
            setValue: value => _config.MaxSaturation = value,
            min: 0,
            max: 100
        );
    }
}