using System.Collections.Generic;
using Entoarox.Framework.Core.Utilities;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DictionaryInjector : IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, DictionaryWrapper> Map = new Dictionary<string, DictionaryWrapper>();


        /*********
        ** Public methods
        *********/
        public bool CanEdit<T>(IAssetInfo info)
        {
            return DictionaryInjector.Map.ContainsKey(info.AssetName) && DictionaryInjector.Map[info.AssetName].DataType == typeof(T);
        }

        public void Edit<T>(IAssetData data)
        {
            DictionaryInjector.Map[data.AssetName].ApplyPatches(data);
        }
    }
}
