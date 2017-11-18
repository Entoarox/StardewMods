using System.Collections.Generic;

using StardewModdingAPI;


namespace Entoarox.Framework.Core.AssetHandlers
{
    using Utilities;
    class DictionaryInjector : IAssetEditor
    {
        internal static Dictionary<string, DictionaryWrapper> _Map = new Dictionary<string, DictionaryWrapper>();
        public bool CanEdit<T>(IAssetInfo info)
        {
            return _Map.ContainsKey(info.AssetName) && _Map[info.AssetName].DataType == typeof(T);
        }

        public void Edit<T>(IAssetData data)
        {
            _Map[data.AssetName].ApplyPatches(data);
        }
    }
}
