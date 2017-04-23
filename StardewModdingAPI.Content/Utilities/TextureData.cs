using Microsoft.Xna.Framework;

namespace StardewModdingAPI.Content.Utilities
{
    struct TextureData
    {
        public string Texture;
        public Rectangle Region;
        public Rectangle? Source;
        public TextureData(string texture, Rectangle region, Rectangle? source)
        {
            Texture = texture;
            Region = region;
            Source = source;
        }
    }
}
