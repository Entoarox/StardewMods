using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SundropCity
{
    static class PaletteBatch
    {
        // Extension methods that make it so PaletteTexture can be used in place of Texture2D when using SpriteBatch.Draw
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Dictionary<string, Color> palette)
        {
            texture.Draw(batch, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, palette, 0, Vector2.Zero, new Vector2(destinationRectangle.Width / (sourceRectangle?.Width ?? texture.Source.Width), destinationRectangle.Height / (sourceRectangle?.Height ?? texture.Source.Height)), SpriteEffects.None, 0);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Rectangle destinationRectangle, Dictionary<string, Color> palette)
        {
            texture.Draw(batch, new Vector2(destinationRectangle.X, destinationRectangle.Y), null, palette, 0, Vector2.Zero, new Vector2(destinationRectangle.Width / texture.Source.Width, destinationRectangle.Height / texture.Source.Height), SpriteEffects.None, 0);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Vector2 position, Rectangle? sourceRectangle, Dictionary<string, Color> palette)
        {
            texture.Draw(batch, position, sourceRectangle, palette, 0, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Vector2 position, Dictionary<string, Color> palette)
        {
            texture.Draw(batch, position, null, palette, 0, Vector2.Zero, Vector2.Zero, SpriteEffects.None, 0);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Dictionary<string, Color> palette, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            texture.Draw(batch, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, palette, rotation, origin, new Vector2(destinationRectangle.Width / (sourceRectangle?.Width ?? texture.Source.Width), destinationRectangle.Height / (sourceRectangle?.Height ?? texture.Source.Height)), effects, layerDepth);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Vector2 position, Rectangle? sourceRectangle, Dictionary<string, Color> palette, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            texture.Draw(batch, position, sourceRectangle, palette, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }
        public static void Draw(this SpriteBatch batch, PaletteTexture texture, Vector2 position, Rectangle? sourceRectangle, Dictionary<string, Color> palette, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            texture.Draw(batch, position, sourceRectangle, palette, rotation, origin, scale, effects, layerDepth);
        }
    }
}
