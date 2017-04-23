using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace StardewModdingAPI.Content
{
    class ContentRegistry
    {
        private IMod Mod;
        public delegate T AssetLoader<T>(string assetName, Func<string, T> loadBase);
        public delegate void AssetInjector<T>(string assetName, ref T asset);
        public ContentRegistry(IMod mod)
        {
            Mod = mod;
        }
        public void AddContentHandler(IContentHandler handler)
        {
            ExtendibleContentManager.AddContentHandler(handler);
        }
        public void RegisterXnbLoader(string asset, string replacement)
        {
            Plugins.XnbLoader.AssetMap.Add(asset, replacement);
        }
        public void RegisterAssetLoader<T>(string asset, AssetLoader<T> loader)
        {
            Plugins.DelegatedContentHandler.AssetLoadMap.Add(asset, loader);
        }
        public void RegisterTypeLoader<T>(AssetLoader<T> loader)
        {
            Plugins.DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
        }
        public void RegisterAssetInjector<T>(string asset, AssetInjector<T> injector)
        {
            if (!Plugins.DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                Plugins.DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            Plugins.DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
        }
        public void RegisterAssetInjector<T>(AssetInjector<T> injector)
        {
            if (!Plugins.DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                Plugins.DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            Plugins.DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
        }
    }
}
