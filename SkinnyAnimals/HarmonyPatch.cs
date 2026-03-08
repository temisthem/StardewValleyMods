using StardewValley;

namespace SkinnyAnimals; 

internal partial class Mod {
    
    public class FarmAnimal_farmerPushing_Patch {
        private static bool IsEnabled() {
            if (!_config.Enabled) return false;
            if (_config.PushSpeedMultiplier == 1 && !_config.IgnoreCollision) return false;
            
            return true;
        }
        public static void Postfix(FarmAnimal __instance) {
            if (!IsEnabled()) return;
            if (__instance.pushAccumulator > 60) return;

            if (_config.IgnoreCollision) __instance.pushAccumulator = 61;
            else __instance.pushAccumulator += _config.PushSpeedMultiplier-1;
        }
    }
}