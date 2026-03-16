using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TemLib;

public static class BubbleDrawHelper {
    public static void DrawEmoteBubble(SpriteBatch spriteBatch, Vector2 position, BubbleConfig config, int frame, float layerDepth) {
        spriteBatch.Draw(
            Game1.emoteSpriteSheet,
            position,
            GetEmoteSourceRect(frame),
            Color.White * config.Opacity,
            0f,
            Vector2.Zero,
            config.Scale,
            SpriteEffects.None,
            layerDepth);
    }
    
    private static Rectangle GetEmoteSourceRect(int frame) {
        return new Rectangle(
            frame * 16 % Game1.emoteSpriteSheet.Width,
            frame * 16 / Game1.emoteSpriteSheet.Width * 16,
            16, 16);
    }
}
