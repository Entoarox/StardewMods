using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Content.Utilities;

namespace StardewModdingAPI.Content.Plugins
{
    class TextureLoader : IContentHandler
    {
        internal static Dictionary<string, List<TextureData>> AssetMap = new Dictionary<string, List<TextureData>>();
        internal static Dictionary<string, Texture2D> AssetCache = new Dictionary<string, Texture2D>();

        // Loader
        public bool IsLoader { get; } = true;
        public bool CanLoad<T>(string assetName)
        {
            return typeof(T) == typeof(Texture2D) && AssetMap.ContainsKey(assetName);
        }
        public T Load<T>(string assetName, Func<string, T> loadBase)
        {
            if (!AssetCache.ContainsKey(assetName))
            {
                var texture=loadBase(assetName) as Texture2D;
                foreach(TextureData data in AssetMap[assetName])
                {
                    var patch = ExtendibleContentManager.ModContent.Load<Texture2D>(data.Texture);
                    var newData = new Color[patch.Width * patch.Height];
                    patch.GetData(0, patch.Bounds, newData, 0, patch.Width * patch.Height);

                    if (data.Region.Bottom > texture.Height || data.Region.Right > texture.Width)
                    {
                        var originalRect = new Rectangle(0, 0, texture.Width, texture.Height);
                        var originalSize = texture.Width * texture.Height;
                        var originalData = new Color[originalSize];
                        texture.GetData(originalData);
                        texture = new Texture2D(
                            StardewValley.Game1.graphics.GraphicsDevice,
                            Math.Max(data.Region.Right, texture.Width),
                            Math.Max(data.Region.Bottom, texture.Height));
                        texture.SetData(0, originalRect, originalData, 0, originalSize);
                    }
                    texture.SetData(0, data.Region, newData, 0, data.Region.Width * data.Region.Height);
                }
                AssetCache[assetName] = texture;
            }
            return (T)(object)AssetCache[assetName];
        }

        // Injector
        public bool IsInjector { get; } = false;
        public bool CanInject<T>(string assetName)
        {
            throw new NotImplementedException();
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            throw new NotImplementedException();
        }
    }
}
