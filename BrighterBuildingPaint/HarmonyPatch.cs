using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace BrighterBuildingPaint;

internal partial class Mod {

    private struct AdjustButton {
        public Rectangle Bounds;
        public BuildingPaintMenu.BuildingColorSlider Slider;
        public int Delta;
    }

    private static readonly List<AdjustButton> _adjustButtons = new();

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
        private const int BtnOffsetX = 260;
        private const int BtnSize = 36;
        private const int BtnGap = 4;
        private const int HoverGrow = 1;

        public static void Postfix(BuildingPaintMenu.ColorSliderPanel __instance, SpriteBatch b) {
            if (!Config.Enabled) return;

            _adjustButtons.Clear();

            var baseX = __instance.buildingPaintMenu.xPositionOnScreen + IClickableMenu.borderWidth + MarginX;
            var baseY = __instance.buildingPaintMenu.yPositionOnScreen + IClickableMenu.borderWidth + MarginY;
            var btnBaseX = baseX + BtnOffsetX;

            var sliders = new[] {
                (slider: __instance.hueSlider, label: I18n.Hue()),
                (slider: __instance.saturationSlider, label: I18n.Saturation()),
                (slider: __instance.lightnessSlider, label: I18n.Lightness())
            };

            for (var i = 0; i < sliders.Length; i++) {
                var y = baseY + GapY * i;
                var (slider, label) = sliders[i];

                Utility.drawTextWithShadow(b, label + ": " + slider.GetValue(), Game1.dialogueFont, new Vector2(baseX, y), Game1.textColor, 0.8f);

                DrawButton(b, btnBaseX, y, "-");
                _adjustButtons.Add(new AdjustButton { Bounds = new Rectangle(btnBaseX, y, BtnSize, BtnSize), Slider = slider, Delta = -1 });

                var plusX = btnBaseX + BtnSize + BtnGap;
                DrawButton(b, plusX, y, "+", 4);
                _adjustButtons.Add(new AdjustButton { Bounds = new Rectangle(plusX, y, BtnSize, BtnSize), Slider = slider, Delta = 1 });
            }
        }

        private static void DrawButton(SpriteBatch b, int x, int y, string text, int offsetY = 0) {
            var hovered = new Rectangle(x, y, BtnSize, BtnSize).Contains(Game1.getMouseX(), Game1.getMouseY());
            var grow = hovered ? HoverGrow : 0;
            var drawX = x - grow;
            var drawY = y - grow;
            var drawSize = BtnSize + grow * 2;

            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), drawX, drawY, drawSize, drawSize, Color.White, 0.5f, false);

            var textSize = Game1.smallFont.MeasureString(text);
            Utility.drawTextWithShadow(b, text, Game1.smallFont,
                new Vector2(drawX + drawSize / 2f - textSize.X / 2f, drawY + drawSize / 2f - textSize.Y / 2f + offsetY),
                Game1.textColor);
        }
    }

    public class BuildingPaintMenu_ReceiveLeftClick_Patch {
        public static void Postfix(BuildingPaintMenu.ColorSliderPanel __instance, int x, int y)
        {
            if (!Config.Enabled) return;

            foreach (var button in _adjustButtons.Where(button => button.Bounds.Contains(x, y)))
            {
                button.Slider.SetValue(button.Slider.GetValue() + button.Delta);
                Game1.playSound("smallSelect");
                break;
            }
        }
    }
}
