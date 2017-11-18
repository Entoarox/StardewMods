using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    class TextureInjector : IAssetEditor
    {
        internal static Dictionary<string, List<(Texture2D, Rectangle?, Rectangle?)>> _Map = new Dictionary<string, List<(Texture2D, Rectangle?, Rectangle?)>>();
        public bool CanEdit<T>(IAssetInfo info) => typeof(T) == typeof(Texture2D) && _Map.ContainsKey(info.AssetName);
        public void Edit<T>(IAssetData data)
        {
            var img = data.AsImage();
            foreach ((Texture2D texture, Rectangle? source, Rectangle? destination) in _Map[data.AssetName])
                img.PatchImage(texture, source, destination, PatchMode.Replace);
        }
    }
}
