using Microsoft.Xna.Framework;

namespace StardewModdingAPI.Content.Utilities
{
    struct TextureData
    {
        public string Texture;
        public Rectangle Region;
        public TextureData(string texture, Rectangle region)
        {
            Texture = texture;
            Region = region;
        }
    }
}
