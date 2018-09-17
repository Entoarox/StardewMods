using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class XnbLoader : IAssetLoader
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, Tuple<IContentHelper, string>> Map = new Dictionary<string, Tuple<IContentHelper, string>>();


        /*********
        ** Public methods
        *********/
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return XnbLoader.Map.ContainsKey(asset.AssetName);
        }

        public T Load<T>(IAssetInfo asset)
        {
            return XnbLoader.Map[asset.AssetName].Item1.Load<T>(XnbLoader.Map[asset.AssetName].Item2);
        }
    }
}
