using TemLib;
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

    private static void DrawFertilizerBubble(HoeDirt hoeDirt, SpriteBatch spriteBatch) {
        if (hoeDirt.HasFertilizer())
            return;
        if (IsTileObstructed(hoeDirt.Tile))
            return;
        if (_config.HideWhenNoCrop && hoeDirt.crop is null)
            return;

        var currentItem = Game1.player.CurrentItem;
        if (IsItemSeed(currentItem))
            return;

        if (_config.DisplayWhenHeld && !IsItemFertilizer(currentItem))
            return;
        if (!_config.DisplayWhenHeld && !_emoteManager.EmoteEnabled)
            return;

        if (_config.HideWhenUnusable) {
            if (currentItem is not null && !hoeDirt.CanApplyFertilizer(currentItem.QualifiedItemId))
                return;
            if (hoeDirt.crop?.indexOfHarvest.Value == "771") //Ignore fiber plants
                return;
        }

        DrawBubble(hoeDirt, spriteBatch);
    }

    private static void DrawSeedBubble(HoeDirt hoeDirt, SpriteBatch spriteBatch) {
        if (hoeDirt.crop is not null)
            return;
        if (IsTileObstructed(hoeDirt.Tile))
            return;

        var currentItem = Game1.player.CurrentItem;
        if (!IsItemSeed(currentItem))
            return;
        if (currentItem is not null && !hoeDirt.canPlantThisSeedHere(currentItem.ItemId))
            return;

        DrawBubble(hoeDirt, spriteBatch);
    }

    private static void DrawBubble(HoeDirt hoeDirt, SpriteBatch spriteBatch) {
        var emotePosition = GetEmotePosition(hoeDirt);
        BubbleDrawHelper.DrawEmoteBubble(spriteBatch, emotePosition, _config, _emoteManager.CurrentFrame, 1f);
    }

    private static Vector2 GetEmotePosition(HoeDirt hoeDirt) {
        var emotePosition = Game1.GlobalToLocal(hoeDirt.Tile * 64);
        var movePercent = _config.MovePercent;
        emotePosition.Y -= 48 - movePercent * 32;
        emotePosition += new Vector2(movePercent * 32 + _config.OffsetX, movePercent * 32 + _config.OffsetY);
        return emotePosition;
    }

    private static bool IsItemFertilizer(Item item) {
        if (item is null) return false;
        if (item.QualifiedItemId == "(O)805") return false; // Tree Fertilizer
        return item.HasContextTag("fertilizer_item") || item.HasContextTag("quality_fertilizer_item");
    }

    private static bool IsItemSeed(Item item) => item is not null && item.HasContextTag("seed_item");

    private static bool IsTileObstructed(Vector2 tile)
        => Game1.currentLocation.objects.TryGetValue(tile, out var obj) && obj is not IndoorPot;
}
