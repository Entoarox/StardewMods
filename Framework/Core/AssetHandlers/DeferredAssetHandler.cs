using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DeferredAssetHandler : IAssetLoader, IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, DeferredAssetInfo> LoadMap = new Dictionary<string, DeferredAssetInfo>();
        internal static Dictionary<string, List<DeferredAssetInfo>> EditMap = new Dictionary<string, List<DeferredAssetInfo>>();


        /*********
        ** Public methods
        *********/
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return this.GetLoader<T>(asset.AssetName) != null;
        }

        public T Load<T>(IAssetInfo asset)
        {
            AssetLoader<T> loader = this.GetLoader<T>(asset.AssetName);
            return loader(asset.AssetName);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return this.GetInjectors<T>(asset.AssetName).Any();
        }

        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            foreach (AssetInjector<T> injector in this.GetInjectors<T>(assetData.AssetName))
                injector(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the loaders for a type and asset name.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="assetName">The asset name.</param>
        private AssetLoader<T> GetLoader<T>(string assetName)
        {
            return DeferredAssetHandler.LoadMap.TryGetValue(assetName, out DeferredAssetInfo entry) && entry.Type == typeof(T)
                ? (AssetLoader<T>)entry.Handler
                : null;
        }

        /// <summary>Get the injectors for a type and asset name.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="assetName">The asset name.</param>
        private IEnumerable<AssetInjector<T>> GetInjectors<T>(string assetName)
        {
            if (!DeferredAssetHandler.EditMap.TryGetValue(assetName, out List<DeferredAssetInfo> entries))
                yield break;

            foreach (DeferredAssetInfo entry in entries)
            {
                if (entry.Type == typeof(T))
                    yield return (AssetInjector<T>)entry.Handler;
            }
        }
    }
}
