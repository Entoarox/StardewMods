using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.ContentManager
{
    class TextureContentInjector : ContentInjector
    {
        private static Texture2D Premultiply(string file)
        {
            Texture2D texture = Texture2D.FromStream(StardewValley.Game1.graphics.GraphicsDevice, new System.IO.FileStream(file, System.IO.FileMode.Open));
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            Parallel.For(0, data.Length, i => { data[i] = Color.FromNonPremultiplied(data[i].ToVector4()); });
            texture.SetData(data);
            return texture;
        }
        private static Texture2D PatchTexture(Texture2D @base,Texture2D input,Rectangle destination)
        {
            if (@base == null)
            {
                throw new ArgumentNullException(nameof(@base));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var newData = new Color[input.Width * input.Height];
            input.GetData(newData);

            if (destination.Bottom > @base.Height || destination.Right > @base.Width)
            {
                var originalRect = new Rectangle(0, 0, @base.Width, @base.Height);
                var originalSize = @base.Width * @base.Height;
                var originalData = new Color[originalSize];
                @base.GetData(originalData);
                @base = new Texture2D(
                    StardewValley.Game1.graphics.GraphicsDevice,
                    Math.Max(destination.Right, @base.Width),
                    Math.Max(destination.Bottom, @base.Height));
                @base.SetData(0, originalRect, originalData, 0, originalSize);
            }

            @base.SetData(0, destination, newData, 0, destination.Width * destination.Height);

            return @base;
        }
        private static Dictionary<string, object> Cache = new Dictionary<string, object>();
        private static Dictionary<string, List<KeyValuePair<string, Rectangle>>> Mapping = new Dictionary<string, List<KeyValuePair<string, Rectangle>>>();
        public static void Register(string assetName, string filePath, Rectangle area)
        {
            if (!Mapping.ContainsKey(GetPlatformSafePath(assetName)))
                Mapping.Add(GetPlatformSafePath(assetName), new List<KeyValuePair<string, Rectangle>>());
            Mapping[GetPlatformSafePath(assetName)].Add(new KeyValuePair<string, Rectangle>(GetPlatformSafePath(filePath), area));
        }
        public override bool CanInject<T>(string assetName)
        {
            return typeof(T)==typeof(Texture2D) && Mapping.ContainsKey(GetPlatformSafePath(assetName));
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            if(!Cache.ContainsKey(assetName))
            {
                Texture2D partial = asset as Texture2D;
                foreach(KeyValuePair<string,Rectangle> patch in Mapping[GetPlatformSafePath(assetName)])
                    partial = PatchTexture(partial, Premultiply(patch.Key),patch.Value);
                Cache.Add(assetName, partial);
            }
            asset = (T)Cache[assetName];
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            throw new NotImplementedException();
        }
    }
}
