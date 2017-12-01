using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    class XnbLoader : IAssetLoader
    {
        internal static Dictionary<string, (IContentHelper, string)> _Map = new Dictionary<string, (IContentHelper, string)>();
        public bool CanLoad<T>(IAssetInfo asset) => _Map.ContainsKey(asset.AssetName);
        public T Load<T>(IAssetInfo asset) => _Map[asset.AssetName].Item1.Load<T>(_Map[asset.AssetName].Item1.GetActualAssetKey(_Map[asset.AssetName].Item2,ContentSource.GameContent));
    }
}
