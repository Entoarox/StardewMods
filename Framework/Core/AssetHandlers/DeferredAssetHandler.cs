using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DeferredAssetHandler : IAssetLoader, IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<(Type, string), Delegate> LoadMap = new Dictionary<(Type, string), Delegate>();
        internal static Dictionary<(Type, string), List<Delegate>> EditMap = new Dictionary<(Type, string), List<Delegate>>();


        /*********
        ** Public methods
        *********/
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return DeferredAssetHandler.LoadMap.ContainsKey((typeof(T), asset.AssetName));
        }

        public T Load<T>(IAssetInfo asset)
        {
            return ((AssetLoader<T>)DeferredAssetHandler.LoadMap[(typeof(T), asset.AssetName)])(asset.AssetName);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return DeferredAssetHandler.EditMap.ContainsKey((typeof(T), asset.AssetName));
        }

        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            foreach (AssetInjector<T> injector in DeferredAssetHandler.EditMap[(typeof(T), assetData.AssetName)])
                injector(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
