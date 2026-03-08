using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace PlaceFloorOnTilledDirt;

internal partial class Mod {
    public class Object_placementAction_Patch {
        public static void Postfix(Object __instance, ref bool __result, GameLocation location, int x, int y,
            Farmer who = null) {
            if (!_config.Enabled) return;

            // ReSharper disable once PossibleLossOfFraction
            Vector2 placementTile = new(x / 64, y / 64);
            var floorPathItemLookup = Flooring.GetFloorPathItemLookup();

            if (!__instance.IsFloorPathItem()
                || !location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature)
                || terrainFeature is not HoeDirt hoeDirt) return;
            
            location.terrainFeatures.Remove(hoeDirt.Tile);
            
            Flooring newFlooring = new(floorPathItemLookup[__instance.ItemId]);
            location.terrainFeatures.Add(placementTile, newFlooring);
            location.playSound(newFlooring.GetData().PlacementSound);

            __result = true;
        }
    }
}