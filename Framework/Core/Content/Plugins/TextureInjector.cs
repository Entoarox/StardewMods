using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Core.Content.Plugins
{
    class TextureInjector : IContentHandler
    {
        internal static Dictionary<string, List<(Texture2D Texture, Rectangle Region, Rectangle Source)>> AssetMap = new Dictionary<string, List<(Texture2D, Rectangle, Rectangle)>>();
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
                foreach((Texture2D patch, Rectangle region, Rectangle source) in AssetMap[assetName])
                {
                    var newData = new Color[source.Width * source.Height];
                    patch.GetData(0, source, newData, 0, source.Width * source.Height);

                    if (region.Bottom > texture.Height || region.Right > texture.Width)
                    {
                        var originalRect = new Rectangle(0, 0, texture.Width, texture.Height);
                        var originalSize = texture.Width * texture.Height;
                        var originalData = new Color[originalSize];
                        texture.GetData(originalData);
                        texture = new Texture2D(
                            StardewValley.Game1.graphics.GraphicsDevice,
                            Math.Max(region.Right, texture.Width),
                            Math.Max(region.Bottom, texture.Height));
                        texture.SetData(0, originalRect, originalData, 0, originalSize);
                    }
                    texture.SetData(0, region, newData, 0, region.Width * region.Height);
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
