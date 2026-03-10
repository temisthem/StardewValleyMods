using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace CropWateringBubbles;

internal partial class Mod
{
    public class IndoorPot_draw_Patch
    {
        public static void Postfix(IndoorPot __instance, SpriteBatch spriteBatch)
        {
            var hoeDirt = __instance.hoeDirt.Value;
            if (!ShouldDrawBubble()) return;
            if (!IsHoeDirtValid(hoeDirt)) return;
            if (IsHarvestReady(hoeDirt.crop)) return;

            var emotePosition = GetEmotePosition(__instance.TileLocation, -72);
            DrawBubble(spriteBatch, emotePosition, 1f);
        }
    }

    public class HoeDirt_draw_Patch
    {
        public static void Postfix(HoeDirt __instance, SpriteBatch spriteBatch)
        {
            if (!ShouldDrawBubble()) return;
            if (!IsHoeDirtValid(__instance)) return;
            if (IsHarvestReady(__instance.crop) && !CanBecomeGiant(__instance)) return;

            var tilePosition = __instance.Tile;
            var emotePosition = GetEmotePosition(tilePosition, -48);
            DrawBubble(spriteBatch, emotePosition, (tilePosition.Y * 64 + 37) / 10000f);
        }
    }
    
    private static bool ShouldDrawBubble()
    {
        if (!_config.Enabled) return false;
        if (!_emoteEnabled) return false;
        if (_config.OnlyWhenWatering && Game1.player.CurrentTool is not WateringCan) return false;
        return true;
    }
    
    private static bool IsHoeDirtValid(HoeDirt hoeDirt)
    {
        if (IsWatered(hoeDirt)) return false;
        
        var crop = hoeDirt.crop;
        if (crop is null) return false;
        if (IsFiberGrass(crop)) return false;
        if (crop.dead.Value) return false;
        return true;
    }

    private static Vector2 GetEmotePosition(Vector2 tile, float yBase)
    {
        var emotePosition = Game1.GlobalToLocal(tile * 64);
        var movePercent = (100 - _config.SizePercent) / 100f;
        emotePosition.Y += yBase + movePercent * 32;
        emotePosition += new Vector2(movePercent * 32 + _config.OffsetX, movePercent * 32 + _config.OffsetY);
        return emotePosition;
    }

    private static void DrawBubble(SpriteBatch spriteBatch, Vector2 emotePosition, float layerDepth)
    {
        spriteBatch.Draw(Game1.emoteSpriteSheet,
            emotePosition,
            new Rectangle(_currentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width,
                _currentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16,
                16,
                16),
            Color.White * (_config.OpacityPercent / 100f),
            0f,
            Vector2.Zero,
            4f * _config.SizePercent / 100f,
            SpriteEffects.None,
            layerDepth);
    }

    private static bool CanBecomeGiant(HoeDirt hoeDirt)
    {
        if (!_config.IncludeGiantable) return false;

        var indexOfHarvest = hoeDirt.crop.indexOfHarvest.Value;
        if (indexOfHarvest is not "190" and not "254" and not "276" and not "Powdermelon" and not "889")
            return false;

        for (var x = -1; x < 2; x++)
        {
            for (var y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;
                if (!IsAdjacentToSame(hoeDirt, x, y)) return false;
            }
        }

        return true;
    }

    private static bool IsAdjacentToSame(HoeDirt hoeDirt, int offsetX, int offsetY)
    {
        if (!hoeDirt.Location.terrainFeatures.TryGetValue(hoeDirt.Tile + new Vector2(offsetX, offsetY),
                out var terrainFeature))
        {
            return false;
        }

        if (terrainFeature is not HoeDirt adjacent) return false;
        if (adjacent.crop is null) return false;
        if (adjacent.crop.indexOfHarvest?.Value != hoeDirt.crop.indexOfHarvest.Value) return false;
        if (adjacent.crop.currentPhase.Value < adjacent.crop.phaseDays.Count - 1) return false;
        return !hoeDirt.crop.fullyGrown.Value || adjacent.crop.dayOfCurrentPhase.Value <= 0;
    }
    
    private static bool IsHarvestReady(Crop crop) =>
        crop.currentPhase.Value >= crop.phaseDays.Count - 1
        && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);

    private static bool IsWatered(HoeDirt hoeDirt) => hoeDirt.state.Value != 0;
    private static bool IsFiberGrass(Crop crop) => crop.indexOfHarvest.Value == "771";
}