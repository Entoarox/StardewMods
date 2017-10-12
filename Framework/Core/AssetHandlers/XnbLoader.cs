using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    class XnbLoader : IAssetLoader
    {
        private static string GetRelativePath(string file)
        {
            Uri fromUri = new Uri(ModEntry.SHelper.DirectoryPath + Path.DirectorySeparatorChar);
            Uri toUri = new Uri(file);
            if (fromUri.Scheme != toUri.Scheme) { throw new InvalidOperationException("Unable to make path relative to the DLL: " + file); }
            return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
        }
        internal static Dictionary<string, (IContentHelper, string)> _Map = new Dictionary<string, (IContentHelper, string)>();
        public bool CanLoad<T>(IAssetInfo asset) => _Map.ContainsKey(asset.AssetName);
        public T Load<T>(IAssetInfo asset) => _Map[asset.AssetName].Item1.Load<T>(GetRelativePath(_Map[asset.AssetName].Item2));
    }
}
