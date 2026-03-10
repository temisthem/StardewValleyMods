using TemLib;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace CropWateringBubbles;

internal partial class Mod : StardewModdingAPI.Mod {
    private static Configuration _config;
    private static readonly EmoteManager _emoteManager = new();

    public override void Entry(IModHelper helper) {
        _config = helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.Input.ButtonsChanged += InputButtonsChanged;
        ApplyHarmonyPatches();
    }

    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.draw)),
            postfix: new HarmonyMethod(typeof(HoeDirt_draw_Patch), nameof(HoeDirt_draw_Patch.Postfix))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(IndoorPot), nameof(IndoorPot.draw), new[] {
                typeof(SpriteBatch),
                typeof(int),
                typeof(int),
                typeof(float)
            }),
            postfix: new HarmonyMethod(typeof(IndoorPot_draw_Patch), nameof(IndoorPot_draw_Patch.Postfix))
        );
    }

    private static void SaveLoaded(object sender, SaveLoadedEventArgs e) {
        _emoteManager.InitOnSaveLoaded(_config);
    }

    private static void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!_config.Enabled) return;
        _emoteManager.Animate(_config, 28, 31);
    }

    private static void InputButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        _emoteManager.HandleToggleInput(_config);
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

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OnlyWhenWatering,
            getValue: () => _config.OnlyWhenWatering,
            setValue: value => _config.OnlyWhenWatering = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.IncludeGiantable,
            getValue: () => _config.IncludeGiantable,
            setValue: value => _config.IncludeGiantable = value
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
            name: I18n.DefaultToggleOn,
            tooltip: I18n.DefaultToggleOnTooltip,
            getValue: () => _config.DefaultToggleOn,
            setValue: value => _config.DefaultToggleOn = value
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
