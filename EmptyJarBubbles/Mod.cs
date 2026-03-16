using TemLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace EmptyJarBubbles;

internal class Mod: StardewModdingAPI.Mod {
    private static Configuration _config;
    private static readonly EmoteManager _emoteManager = new();
    private static List<Object> _machines;
    private static Dictionary<string, MachineData> _machineData;
    private static List<string> _moddedMachineQualifiedIds;

    public override void Entry(IModHelper helper) {
        _config = Helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.Input.ButtonsChanged += InputButtonsChanged;
        helper.Events.GameLoop.DayStarted += DayStarted;
        helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        helper.Events.World.ObjectListChanged += ObjectListChanged;
        helper.Events.Player.Warped += Warped;
        helper.Events.Display.MenuChanged += MenuChanged;
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private static void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!_config.Enabled) return;
        _emoteManager.Animate(_config, 16, 19);
    }

    private static void InputButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        _emoteManager.HandleToggleInput(_config);
    }

    private void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        _emoteManager.InitOnSaveLoaded(_config);
        Helper.Events.Display.RenderedWorld += RenderBubbles;
        _machineData = DataLoader.Machines(Game1.content);
        _moddedMachineQualifiedIds = GetModdedMachinesFromMachineData();
        ApplyZoomLevel99();
    }

    private static List<string> GetModdedMachinesFromMachineData() {
        return _machineData
            .Where(kvp =>
                kvp.Value.OutputRules is not null &&
                kvp.Value.OutputRules
                    .Any(outputRule => outputRule.Triggers
                        .Any(triggerRule => triggerRule.Trigger == MachineOutputTrigger.ItemPlacedInMachine)))
            .Select(kvp => kvp.Key)
            .Where(x => !VanillaMachineQualifiedIds.Contains(x))
            .ToList();
    }

    private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        Helper.Events.Display.RenderedWorld -= RenderBubbles;
    }

    private static void Warped(object sender, WarpedEventArgs e) {
        BuildMachineList();
    }

    private static void DayStarted(object sender, DayStartedEventArgs e) {
        BuildMachineList();
    }

    private static void MenuChanged(object sender, MenuChangedEventArgs e) {
        BuildMachineList();
        ApplyZoomLevel99();
    }

    private static void ObjectListChanged(object sender, ObjectListChangedEventArgs e) {
        if (!_config.Enabled) return;

        var removedMachines = e.Removed
            .Where(kvp => IsValidMachine(kvp.Value))
            .Select(kvp => kvp.Value)
            .ToHashSet();

        var newMachines = e.Added
            .Where(kvp => IsValidMachine(kvp.Value))
            .Select(kvp => kvp.Value);

        _machines.RemoveAll(x => removedMachines.Contains(x));
        _machines.AddRange(newMachines);
    }

    private static void BuildMachineList()
    {
        if (!_config.Enabled) return;
        if (Game1.currentLocation is null) return;
        if (_moddedMachineQualifiedIds is null) return;

        _machines = Game1.currentLocation.Objects.Values
            .Where(IsValidMachine)
            .ToList();
    }

    private static bool IsValidMachine(Object o) {
        return IsObjectValidMachine(o, _config.JarsEnabled, VanillaMachineQualifiedIds.Jar) ||
               IsObjectValidMachine(o, _config.KegsEnabled, VanillaMachineQualifiedIds.Keg) ||
               (IsObjectValidMachine(o, _config.CasksEnabled, VanillaMachineQualifiedIds.Cask) && OnlyShowCaskBubblesWhenInCellar()) ||
               IsObjectValidMachine(o, _config.MayonnaiseMachinesEnabled, VanillaMachineQualifiedIds.MayonnaiseMachine) ||
               IsObjectValidMachine(o, _config.CheesePressesEnabled, VanillaMachineQualifiedIds.CheesePress) ||
               IsObjectValidMachine(o, _config.LoomsEnabled, VanillaMachineQualifiedIds.Loom) ||
               IsObjectValidMachine(o, _config.OilMakersEnabled, VanillaMachineQualifiedIds.OilMaker) ||
               IsObjectValidMachine(o, _config.DehydratorsEnabled, VanillaMachineQualifiedIds.Dehydrator) ||
               IsObjectValidMachine(o, _config.FishSmokersEnabled, VanillaMachineQualifiedIds.FishSmoker) ||
               IsObjectValidMachine(o, _config.BaitMakersEnabled, VanillaMachineQualifiedIds.BaitMaker) ||
               IsObjectValidMachine(o, _config.BoneMillsEnabled, VanillaMachineQualifiedIds.BoneMill) ||
               IsObjectValidMachine(o, _config.CharcoalKilnsEnabled, VanillaMachineQualifiedIds.CharcoalKiln) ||
               IsObjectValidMachine(o, _config.CrystalariumsEnabled, VanillaMachineQualifiedIds.Crystalarium) ||
               IsObjectValidMachine(o, _config.FurnacesEnabled, VanillaMachineQualifiedIds.Furnace) ||
               IsObjectValidMachine(o, _config.FurnacesEnabled, VanillaMachineQualifiedIds.HeavyFurnace) ||
               IsObjectValidMachine(o, _config.RecyclingMachinesEnabled, VanillaMachineQualifiedIds.RecyclingMachine) ||
               IsObjectValidMachine(o, _config.SeedMakersEnabled, VanillaMachineQualifiedIds.SeedMaker) ||
               IsObjectValidMachine(o, _config.SlimeEggPressesEnabled, VanillaMachineQualifiedIds.SlimeEggPress) ||
               IsObjectValidMachine(o, _config.CrabPotsEnabled, VanillaMachineQualifiedIds.CrabPot) ||
               IsObjectValidMachine(o, _config.DeconstructorsEnabled, VanillaMachineQualifiedIds.Deconstructor) ||
               IsObjectValidMachine(o, _config.GeodeCrushersEnabled, VanillaMachineQualifiedIds.GeodeCrusher) ||
               IsObjectValidMachine(o, _config.WoodChippersEnabled, VanillaMachineQualifiedIds.WoodChipper) ||
               (_config.ModdedMachinesEnabled && _moddedMachineQualifiedIds.Contains(o.QualifiedItemId));
    }

    private static bool OnlyShowCaskBubblesWhenInCellar()
    {
        if (_config.CasksOutsideOfCellarEnabled) return true;
        return Game1.player.currentLocation is Cellar;
    }

    private static bool IsObjectValidMachine(Object o, bool enabled, string qualifiedId) {
        if (!enabled) return false;
        return o.QualifiedItemId == qualifiedId;
    }

    private static void RenderBubbles(object sender, RenderedWorldEventArgs e) {
        if (!_config.Enabled) return;
        if (_config.ToggleEmoteKey.IsBound && !_emoteManager.EmoteEnabled) return;

        foreach (var machine in _machines.Where(IsMachineRenderReady))
            DrawBubbles(machine, e.SpriteBatch);
    }

    private static bool IsMachineRenderReady(Object o) {
        return (o is not CrabPot && o.MinutesUntilReady <= 0 && !o.readyForHarvest.Value) ||
               (o is CrabPot pot && pot.bait.Value is null && pot.heldObject.Value is null);
    }

    private static void DrawBubbles(Object o, SpriteBatch spriteBatch) {
        var tilePosition = o.TileLocation * 64;
        var emotePosition = Game1.GlobalToLocal(tilePosition);
        emotePosition += new Vector2(_config.MovePercent * 32 + _config.OffsetX, -_config.OffsetY);
        if (o is CrabPot pot) {
            emotePosition += pot.directionOffset.Value;
            emotePosition.Y += pot.yBob + 20;
        }

        BubbleDrawHelper.DrawEmoteBubble(spriteBatch, emotePosition, _config, _emoteManager.CurrentFrame, (tilePosition.Y + 37) / 10000f);
    }

    private static void ApplyZoomLevel99() {
        if (!_config.Enabled) return;
        if (!_config.ZoomLevel99Enabled) return;

        Game1.options.desiredBaseZoomLevel = 0.99f;
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
            min: 0,
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

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ZoomLevel99Enabled,
            tooltip: I18n.ZoomLevel99EnabledTooltip,
            getValue: () => _config.ZoomLevel99Enabled,
            setValue: value => _config.ZoomLevel99Enabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ModdedMachinesEnabled,
            tooltip: I18n.ModdedMachinesEnabledTooltip,
            getValue: () => _config.ModdedMachinesEnabled,
            setValue: value => _config.ModdedMachinesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.JarsEnabled,
            getValue: () => _config.JarsEnabled,
            setValue: value => _config.JarsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.KegsEnabled,
            getValue: () => _config.KegsEnabled,
            setValue: value => _config.KegsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CasksEnabled,
            getValue: () => _config.CasksEnabled,
            setValue: value => _config.CasksEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CasksOutsideOfCellarEnabled,
            getValue: () => _config.CasksOutsideOfCellarEnabled,
            setValue: value => _config.CasksOutsideOfCellarEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.MayonnaiseMachinesEnabled,
            getValue: () => _config.MayonnaiseMachinesEnabled,
            setValue: value => _config.MayonnaiseMachinesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CheesePressesEnabled,
            getValue: () => _config.CheesePressesEnabled,
            setValue: value => _config.CheesePressesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.LoomsEnabled,
            getValue: () => _config.LoomsEnabled,
            setValue: value => _config.LoomsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OilMakersEnabled,
            getValue: () => _config.OilMakersEnabled,
            setValue: value => _config.OilMakersEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DehydratorsEnabled,
            getValue: () => _config.DehydratorsEnabled,
            setValue: value => _config.DehydratorsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.FishSmokersEnabled,
            getValue: () => _config.FishSmokersEnabled,
            setValue: value => _config.FishSmokersEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.BaitMakersEnabled,
            getValue: () => _config.BaitMakersEnabled,
            setValue: value => _config.BaitMakersEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.BoneMillsEnabled,
            getValue: () => _config.BoneMillsEnabled,
            setValue: value => _config.BoneMillsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CharcoalKilnsEnabled,
            getValue: () => _config.CharcoalKilnsEnabled,
            setValue: value => _config.CharcoalKilnsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CrystalariumsEnabled,
            getValue: () => _config.CrystalariumsEnabled,
            setValue: value => _config.CrystalariumsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.FurnacesEnabled,
            getValue: () => _config.FurnacesEnabled,
            setValue: value => _config.FurnacesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RecyclingMachinesEnabled,
            getValue: () => _config.RecyclingMachinesEnabled,
            setValue: value => _config.RecyclingMachinesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.SeedMakersEnabled,
            getValue: () => _config.SeedMakersEnabled,
            setValue: value => _config.SeedMakersEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.SlimeEggPressesEnabled,
            getValue: () => _config.SlimeEggPressesEnabled,
            setValue: value => _config.SlimeEggPressesEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CrabPotsEnabled,
            getValue: () => _config.CrabPotsEnabled,
            setValue: value => _config.CrabPotsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DeconstructorsEnabled,
            getValue: () => _config.DeconstructorsEnabled,
            setValue: value => _config.DeconstructorsEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.GeodeCrushersEnabled,
            getValue: () => _config.GeodeCrushersEnabled,
            setValue: value => _config.GeodeCrushersEnabled = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.WoodChippersEnabled,
            getValue: () => _config.WoodChippersEnabled,
            setValue: value => _config.WoodChippersEnabled = value
        );
    }
}
