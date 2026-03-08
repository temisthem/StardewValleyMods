using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace WheresMyHorse;

internal partial class Mod {
    public class Horse_draw_Patch {
        [HarmonyAfter("Goldenrevolver.HorseOverhaul")]
        public static void Postfix(Horse __instance, SpriteBatch b) {
            if (!_config.Enabled) return;
            if (__instance.rider != null) return;
            if (_config.OnlyMyHorse && __instance.getOwner() != Game1.player) return; 
            if (!_config.AlwaysRender && !_emoteEnabled) return;

            var offsetX = _config.OffsetX + (__instance.GetSpriteWidthForPositioning() == 16 ? 0f : 32f);
            var offsetY = _config.OffsetY - 96f;
            var localPosition = __instance.getLocalPosition(Game1.viewport) + new Vector2(offsetX, offsetY);
            
            float num = __instance.StandingPixel.Y + 1;

            switch (__instance.FacingDirection)
            {
                case 0:
                    localPosition.Y -= 40f;
                    break;
                case 1:
                    localPosition.X += 40f;
                    localPosition.Y -= 30f;
                    break;
                case 2:
                    localPosition.Y += 5f;
                    break;
                case 3:
                    localPosition.X -= 40f;
                    localPosition.Y -= 30f;
                    break;
            }

            b.Draw(Game1.emoteSpriteSheet,
                localPosition,
                new Rectangle(_currentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width,
                    _currentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 
                    16, 
                    16), 
                Color.White  * (_config.OpacityPercent / 100f), 
                0.0f, 
                Vector2.Zero,
                4f * _config.SizePercent / 100f, 
                SpriteEffects.None, 
                _config.RenderOnTop ? 0.99f : num / 10000f);
        }
    }
}