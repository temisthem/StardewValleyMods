using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace BrighterBuildingPaint; 

internal partial class Mod {
    
    public class BuildingPaintMenu_BuildingColorSlider_Patch {
        public static void Postfix(BuildingPaintMenu.BuildingColorSlider __instance) {
            if (!Config.Enabled) return;

            if (__instance.handle.myID == 107) {
                __instance.max = Config.MaxSaturation;
            }
            
            if (__instance.handle.myID == 108) {
                __instance.max = Config.MaxBrightness;
                __instance.min = Config.MinBrightness;
            }
        }
    }

    public class BuildingPaintMenu_Draw_Patch {
        private const int MarginX = 568;
        private const int MarginY = 208;
        private const int GapY = 40;
        
        public static void Postfix(BuildingPaintMenu.ColorSliderPanel __instance, SpriteBatch b) {
            if (!Config.Enabled) return;
            
            Utility.drawTextWithShadow(b, I18n.Hue() + ": " + __instance.hueSlider.GetValue(), Game1.dialogueFont, new Vector2(__instance.buildingPaintMenu.xPositionOnScreen + IClickableMenu.borderWidth + MarginX, __instance.buildingPaintMenu.yPositionOnScreen + IClickableMenu.borderWidth + MarginY), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(b, I18n.Saturation() + ": " + __instance.saturationSlider.GetValue(), Game1.dialogueFont, new Vector2(__instance.buildingPaintMenu.xPositionOnScreen + IClickableMenu.borderWidth + MarginX, __instance.buildingPaintMenu.yPositionOnScreen + IClickableMenu.borderWidth + MarginY + GapY), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(b, I18n.Lightness() + ": " + __instance.lightnessSlider.GetValue(), Game1.dialogueFont, new Vector2(__instance.buildingPaintMenu.xPositionOnScreen + IClickableMenu.borderWidth + MarginX, __instance.buildingPaintMenu.yPositionOnScreen + IClickableMenu.borderWidth + MarginY + GapY*2), Game1.textColor, 0.8f);
        }
    }
}