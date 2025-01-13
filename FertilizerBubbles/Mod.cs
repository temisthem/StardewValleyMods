using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FertilizerBubbles;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;
    private static IModHelper _modHelper;
    private static int _currentEmoteInterval;
    private static int _currentEmoteFrame;
    private static bool _toggleEmoteEnabled;

    public override void Entry(IModHelper helper) {
        _config = helper.ReadConfig<Configuration>();
        _modHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        Helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.draw)),
            postfix: new HarmonyMethod(typeof(HoeDirt_draw_Patch), nameof(HoeDirt_draw_Patch.Postfix))
        );
        
        harmony.Patch(
            original: AccessTools.Method(typeof(IndoorPot), nameof(IndoorPot.draw), new [] {
                typeof(SpriteBatch),
                typeof(int),
                typeof(int),
                typeof(float)
            }),
            postfix: new HarmonyMethod(typeof(IndoorPot_draw_Patch), nameof(IndoorPot_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = _modHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        if (!_config.Enabled) return;
        if (_config.ToggleEmoteKey.JustPressed()) _toggleEmoteEnabled = !_toggleEmoteEnabled;
    }
    
    private void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!_config.Enabled) return;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        _currentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (_currentEmoteFrame is < 16 or > 19) _currentEmoteFrame = 16;
        if (_currentEmoteInterval > _config.EmoteInterval) {
            if (_currentEmoteFrame < 19) _currentEmoteFrame++;
            else _currentEmoteFrame = 16;
            _currentEmoteInterval = 0;
        }
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
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => _config.debugOffset,
            setValue: value => _config.debugOffset = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisplayBubbleForFertilizers,
            getValue: () => _config.DisplayBubbleForFertilizers,
            setValue: value => _config.DisplayBubbleForFertilizers = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisplayBubbleForSeeds,
            getValue: () => _config.DisplayBubbleForSeeds,
            setValue: value => _config.DisplayBubbleForSeeds = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisplayWhenHeld,
            getValue: () => _config.DisplayWhenHeld,
            setValue: value => _config.DisplayWhenHeld = value
        );
        
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: I18n.ToggleEmoteKey,
            tooltip: I18n.ToggleEmoteKeyTooltip,
            getValue: () => _config.ToggleEmoteKey,
            setValue: value => _config.ToggleEmoteKey = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.HideWhenUnusable,
            getValue: () => _config.HideWhenUnusable,
            setValue: value => _config.HideWhenUnusable = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.HideWhenNoCrop,
            getValue: () => _config.HideWhenNoCrop,
            setValue: value => _config.HideWhenNoCrop = value
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
            name: I18n.EmoteInterval,
            getValue: () => _config.EmoteInterval,
            setValue: value => _config.EmoteInterval = value,
            min: 0,
            max: 1000
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