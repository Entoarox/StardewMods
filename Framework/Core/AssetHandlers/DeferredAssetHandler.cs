using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    class DeferredAssetHandler : IAssetLoader, IAssetEditor
    {
        internal static Dictionary<(Type, string), Delegate> _LoadMap = new Dictionary<(Type, string), Delegate>();
        public bool CanLoad<T>(IAssetInfo asset) => _LoadMap.ContainsKey((typeof(T),asset.AssetName));
        public T Load<T>(IAssetInfo asset) => ((AssetLoader<T>)_LoadMap[(typeof(T), asset.AssetName)])(asset.AssetName);
        
        internal static Dictionary<(Type, string), List<Delegate>> _EditMap = new Dictionary<(Type, string), List<Delegate>>();
        public bool CanEdit<T>(IAssetInfo asset) => _EditMap.ContainsKey((typeof(T), asset.AssetName));
        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            foreach(AssetInjector<T> injector in _EditMap[(typeof(T), assetData.AssetName)])
                injector(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
