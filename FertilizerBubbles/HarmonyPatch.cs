using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace FertilizerBubbles;

internal partial class Mod {
    public class IndoorPot_draw_Patch {
        public static void Postfix(IndoorPot __instance, SpriteBatch spriteBatch) {
            if (!_config.Enabled) return;
            
            if (_config.DisplayBubbleForFertilizers) {
                if (_config.HideWhenUnusable && __instance.bush.Value is not null) 
                    return;
                DrawFertilizerBubble(__instance.hoeDirt.Get(), spriteBatch);
            }

            if (_config.DisplayBubbleForSeeds) {
                if (__instance.bush.Value is not null) 
                    return;
                DrawSeedBubble(__instance.hoeDirt.Get(), spriteBatch);
            }
        }
    }

    public class HoeDirt_draw_Patch {
        public static void Postfix(HoeDirt __instance, SpriteBatch spriteBatch) {
            if (!_config.Enabled) return;

            if (_config.DisplayBubbleForFertilizers) {
                DrawFertilizerBubble(__instance, spriteBatch);
            }

            if (_config.DisplayBubbleForSeeds) {
                DrawSeedBubble(__instance, spriteBatch);
            }
        }
    }

    private static bool IsItemFertilizer(Item item) {
        if (item is null) return false;
        if (item.QualifiedItemId == "(O)805") return false; // Tree Fertilizer
        return item.HasContextTag("fertilizer_item") || item.HasContextTag("quality_fertilizer_item");
    }

    private static bool IsItemSeed(Item item) {
        if (item is null) return false;
        var obj = new Object(item.ItemId, 1);
        return obj.Type == "Seeds";
    }

    private static void DrawFertilizerBubble(HoeDirt __instance, SpriteBatch spriteBatch) {
        if (__instance.HasFertilizer()) 
            return;
        if (_config.HideWhenNoCrop && __instance.crop is null) 
            return;

        var currentItem = Game1.player.CurrentItem;

        if (_config.DisplayWhenHeld && !IsItemFertilizer(currentItem)) 
            return;
        if (IsTileObstructed(__instance.Tile)) 
            return;
        if (_config.HideWhenUnusable) {
            if (currentItem is not null && !__instance.CanApplyFertilizer(currentItem.QualifiedItemId)) 
                return;
            if (__instance.crop?.indexOfHarvest.Value == "771") //Ignore fiber plants
                return;
        }
        if (!_config.DisplayWhenHeld && !_toggleEmoteEnabled) 
            return;
        if (IsItemSeed(currentItem)) 
            return;
        
        DrawBubble(__instance, spriteBatch);
    }

    private static void DrawSeedBubble(HoeDirt __instance, SpriteBatch spriteBatch) {
        var currentItem = Game1.player.CurrentItem;

        if (__instance.crop is not null) 
            return;
        if (IsTileObstructed(__instance.Tile)) 
            return;
        if (currentItem is not null && !__instance.canPlantThisSeedHere(currentItem.ItemId)) 
            return;
        if (!IsItemSeed(currentItem)) 
            return;

        DrawBubble(__instance, spriteBatch);
    }

    private static bool IsTileObstructed(Vector2 tile)
        => Game1.currentLocation.objects.TryGetValue(tile, out var obj) && obj is not IndoorPot;

    private static void DrawBubble(HoeDirt __instance, SpriteBatch spriteBatch, float yOffset = 0f) {
        var tilePosition = GetEmotePosition(__instance, out var emotePosition);

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
            1f);
    }

    private static Vector2 GetEmotePosition(HoeDirt __instance, out Vector2 emotePosition) {
        Vector2 tilePosition = __instance.Tile;
        emotePosition = Game1.GlobalToLocal(tilePosition * 64);
        float movePercent = (100 - _config.SizePercent) / 100f;
        emotePosition.Y -= 48 - movePercent * 32;
        emotePosition += new Vector2(movePercent * 32 + _config.OffsetX, movePercent * 32 + _config.OffsetY);
        return tilePosition;
    }
}