using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    class DeferredTypeHandler : IAssetEditor
    {
        internal static Dictionary<Type, List<Delegate>> _EditMap = new Dictionary<Type, List<Delegate>>();
        public bool CanEdit<T>(IAssetInfo asset) => _EditMap.ContainsKey(typeof(T));
        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            foreach (AssetInjector<T> injector in _EditMap[typeof(T)])
                injector(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
