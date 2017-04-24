using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content.Plugins
{
    using Utilities;

    class TextureInjector : IContentHandler
    {
        internal static Dictionary<string, List<TextureData>> AssetMap = new Dictionary<string, List<TextureData>>();
        internal static Dictionary<string, object> AssetCache = new Dictionary<string, object>();
        
        public bool IsInjector { get; } = true;
        public bool CanInject<T>(string assetName)
        {
            return typeof(T) == typeof(Texture2D) && AssetMap.ContainsKey(assetName);
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            if (!AssetCache.ContainsKey(assetName))
            {
                var texture=asset as Texture2D;
                foreach(TextureData data in AssetMap[assetName])
                {
                    var newData = new Color[data.Source.Width * data.Source.Height];
                    data.Texture.GetData(0, data.Source, newData, 0, data.Source.Width * data.Source.Height);

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
            asset=(T)AssetCache[assetName];
        }
        
        public bool IsLoader { get; } = false;
        public bool CanLoad<T>(string assetName)
        {
            throw new NotImplementedException();
        }
        public T Load<T>(string assetName, Func<string,T> loadBase)
        {
            throw new NotImplementedException();
        }
    }
}
