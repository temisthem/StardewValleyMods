using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace WheresMyHorse;

internal partial class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;
    private static IModHelper _modHelper;
    private static bool _emoteEnabled;
    private static int _currentEmoteInterval;
    private static int _currentEmoteFrame;

    public override void Entry(IModHelper helper) {
        _config = helper.ReadConfig<Configuration>();
        _modHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        helper.Events.Player.Warped += Warped;
        helper.Events.Input.ButtonsChanged += InputButtonsChanged;
        ApplyHarmonyPatches();
    }

    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new []{typeof(SpriteBatch)}),
            postfix: new HarmonyMethod(typeof(Horse_draw_Patch), nameof(Horse_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = _modHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private static void Warped(object sender, WarpedEventArgs e) {
        if (_config.DisableOnMapChange) _emoteEnabled = false;
    }
    
    private static void SaveLoaded(object sender, SaveLoadedEventArgs e) {
        _emoteEnabled = false;
    }
    
    private static void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!_config.Enabled) return;
        if (_config.DisableOnMount && Game1.player.isAnimatingMount) _emoteEnabled = false;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        _currentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (_currentEmoteFrame is < 40 or > 43) _currentEmoteFrame = 40;
        if (_currentEmoteInterval > _config.EmoteInterval) {
            if (_currentEmoteFrame < 43) _currentEmoteFrame++;
            else _currentEmoteFrame = 40;
            _currentEmoteInterval = 0;
        }
    }
    
    private static void InputButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        if (!_config.Enabled) return;
        if (Game1.player.isRidingHorse()) return;
        if (!_config.DoEmoteKey.JustPressed()) return;
        
        if (IsHorseInLocation()) _emoteEnabled = !_emoteEnabled;
        else Game1.player.doEmote(8);
    }

    private static bool IsHorseInLocation() {
        var horses = Game1.player.currentLocation.characters.OfType<Horse>().ToList();
        return _config.OnlyMyHorse ?
            horses.Any(horse => horse.getOwner() == Game1.player) :
            horses.Any();
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

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: I18n.DoEmoteKey,
            getValue: () => _config.DoEmoteKey,
            setValue: value => _config.DoEmoteKey = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RenderOnTop,
            getValue: () => _config.RenderOnTop,
            setValue: value => _config.RenderOnTop = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisableOnMount,
            getValue: () => _config.DisableOnMount,
            setValue: value => _config.DisableOnMount = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisableOnMapChange,
            getValue: () => _config.DisableOnMapChange,
            setValue: value => _config.DisableOnMapChange = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OnlyMyHorse,
            getValue: () => _config.OnlyMyHorse,
            setValue: value => _config.OnlyMyHorse = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.AlwaysRender,
            getValue: () => _config.AlwaysRender,
            setValue: value => _config.AlwaysRender = value
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
            max: 200
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
    }
}