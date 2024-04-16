using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace EmptyJarBubbles;

internal class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static int CurrentEmoteFrame;
    internal static int CurrentEmoteInterval;

    public override void Entry(IModHelper helper) {
        Config = Helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!Config.Enabled) return;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        CurrentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (CurrentEmoteFrame is < 16 or > 19) CurrentEmoteFrame = 16;
        if (CurrentEmoteInterval > Config.EmoteInterval) {
            if (CurrentEmoteFrame < 19) CurrentEmoteFrame++;
            else CurrentEmoteFrame = 16;
            CurrentEmoteInterval = 0;
        }
    }

    private void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        Helper.Events.Display.RenderedWorld += RenderBubbles;
    }

    private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        Helper.Events.Display.RenderedWorld -= RenderBubbles;
    }

    private void RenderBubbles(object sender, RenderedWorldEventArgs e) {
        if (!Config.Enabled) return;
        if (Game1.currentLocation is null) return;

        // MinutesUntilReady <= 0 because casks that have an item removed will be < 0
        var objects = Game1.currentLocation.Objects.Values
            .Where(o => IsObjectJar(o) || IsObjectKeg(o) || IsObjectCask(o) ||
                        IsObjectMayonnaiseMachine(o) || IsObjectCheesePress(o) || IsObjectLoom(o) ||
                        IsObjectOilMaker(o) || IsObjectDehydrator(o) || IsObjectFishSmoker(o))
            .Where(o => o.MinutesUntilReady <= 0 && !o.readyForHarvest.Value)
            .ToList();

        foreach (var o in objects) DrawBubbles(o, e.SpriteBatch);
    }

    private void DrawBubbles(Object o, SpriteBatch spriteBatch) {
        Vector2 tilePosition = o.TileLocation * 64;
        Vector2 emotePosition = Game1.GlobalToLocal(tilePosition);
        emotePosition += new Vector2((100 - Config.SizePercent) / 100f * 32, -Config.OffsetY);
        
        spriteBatch.Draw(Game1.emoteSpriteSheet,
            emotePosition, 
            new Rectangle(CurrentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
            Color.White * (Config.OpacityPercent / 100f), 
            0f,
            Vector2.Zero, 
            4f * Config.SizePercent / 100f, 
            SpriteEffects.None, 
            (tilePosition.Y + 37) / 10000f);
    }
    
    private bool IsObjectJar(Object o) {
        if (!Config.JarsEnabled) return false;
        return o.QualifiedItemId == "(BC)15";
    }

    private bool IsObjectKeg(Object o) {
        if (!Config.KegsEnabled) return false;
        return o.QualifiedItemId == "(BC)12";
    }
    
    private bool IsObjectCask(Object o) {
        if (!Config.CasksEnabled) return false;
        return o.QualifiedItemId == "(BC)163";
    }
    
    private bool IsObjectMayonnaiseMachine(Object o) {
        if (!Config.MayonnaiseMachineEnabled) return false;
        return o.QualifiedItemId == "(BC)24";
    }
    
    private bool IsObjectCheesePress(Object o) {
        if (!Config.CheesePressEnabled) return false;
        return o.QualifiedItemId == "(BC)16";
    }
    
    private bool IsObjectLoom(Object o) {
        if (!Config.LoomEnabled) return false;
        return o.QualifiedItemId == "(BC)17";
    }
    
    private bool IsObjectOilMaker(Object o) {
        if (!Config.OilMakerEnabled) return false;
        return o.QualifiedItemId == "(BC)19";
    }
    
    private bool IsObjectDehydrator(Object o) {
        if (!Config.DehydratorEnabled) return false;
        return o.QualifiedItemId == "(BC)Dehydrator";
    }
    
    private bool IsObjectFishSmoker(Object o) {
        if (!Config.FishSmokerEnabled) return false;
        return o.QualifiedItemId == "(BC)FishSmoker";
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => Helper.WriteConfig(Config)
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.JarsEnabled, 
            getValue: () => Config.JarsEnabled,
            setValue: value => Config.JarsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.KegsEnabled, 
            getValue: () => Config.KegsEnabled,
            setValue: value => Config.KegsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CasksEnabled, 
            getValue: () => Config.CasksEnabled,
            setValue: value => Config.CasksEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.MayonnaiseMachinesEnabled, 
            getValue: () => Config.MayonnaiseMachineEnabled,
            setValue: value => Config.MayonnaiseMachineEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CheesePressesEnabled, 
            getValue: () => Config.CheesePressEnabled,
            setValue: value => Config.CheesePressEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.LoomsEnabled, 
            getValue: () => Config.LoomEnabled,
            setValue: value => Config.LoomEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OilMakersEnabled, 
            getValue: () => Config.OilMakerEnabled,
            setValue: value => Config.OilMakerEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DehydratorsEnabled, 
            getValue: () => Config.DehydratorEnabled,
            setValue: value => Config.DehydratorEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.FishSmokersEnabled, 
            getValue: () => Config.FishSmokerEnabled,
            setValue: value => Config.FishSmokerEnabled = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleYOffset,
            getValue: () => Config.OffsetY,
            setValue: value => Config.OffsetY = value,
            min: 0,
            max: 128
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.EmoteInterval,
            getValue: () => Config.EmoteInterval,
            setValue: value => Config.EmoteInterval = value,
            min: 0,
            max: 1000
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Opacity,
            getValue: () => Config.OpacityPercent,
            setValue: value => Config.OpacityPercent = value,
            min: 1,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleSize,
            getValue: () => Config.SizePercent,
            setValue: value => Config.SizePercent = value,
            min: 1,
            max: 100
        );
    }
}