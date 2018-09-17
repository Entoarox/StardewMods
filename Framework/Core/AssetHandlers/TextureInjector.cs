using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class TextureInjector : IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, List<(Texture2D, Rectangle?, Rectangle?)>> Map = new Dictionary<string, List<(Texture2D, Rectangle?, Rectangle?)>>();


        /*********
        ** Public methods
        *********/
        public bool CanEdit<T>(IAssetInfo info)
        {
            return typeof(T) == typeof(Texture2D) && TextureInjector.Map.ContainsKey(info.AssetName);
        }

        public void Edit<T>(IAssetData data)
        {
            IAssetDataForImage img = data.AsImage();
            foreach ((Texture2D texture, Rectangle? source, Rectangle? destination) in TextureInjector.Map[data.AssetName])
                img.PatchImage(texture, source, destination, PatchMode.Replace);
        }
    }
}
