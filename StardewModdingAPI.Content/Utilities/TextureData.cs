using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content.Utilities
{
    struct TextureData
    {
        public Texture2D Texture;
        public Rectangle Region;
        public Rectangle Source;
        public TextureData(Texture2D texture, Rectangle region, Rectangle? source)
        {
            Texture = texture;
            Region = region;
            Source = source ?? new Rectangle(0, 0, texture.Width, texture.Height);
        }
    }
}
