using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content.Internal
{
    using Plugins;
    using Utilities;

    class ContentRegistry : IContentRegistry
    {
        private IMod Mod;
        private string ModPath;
        public ContentRegistry(IMod mod)
        {
            Mod = mod;
            ModPath = Mod.Helper.DirectoryPath.Replace(ExtendibleContentManager.ModContent.RootDirectory,"");
        }
        public string Normalize(string path)
        {
            return path.Replace('/', '\\');
        }
        public T Load<T>(string assetName)
        {
            return ExtendibleContentManager.ModContent.Load<T>(Path.Combine(ModPath, assetName));
        }
        public void RegisterContentHandler(IContentHandler handler)
        {
            ExtendibleContentManager.AddContentHandler(handler);
        }
        public void RegisterTexturePatch(string asset, Texture2D patch, Rectangle destination, Rectangle? source)
        {
            asset = Normalize(asset);
            if (!TextureInjector.AssetMap.ContainsKey(asset))
                TextureInjector.AssetMap.Add(asset, new List<TextureData>());
            TextureInjector.AssetMap[asset].Add(new TextureData(patch, destination, source));
            if (TextureInjector.AssetCache.ContainsKey(asset))
                TextureInjector.AssetCache.Remove(asset);
        }
        public void RegisterTexturePatch(string asset, string patch, Rectangle destination, Rectangle? source)
        {
            RegisterTexturePatch(asset, Load<Texture2D>(patch), destination, source);
        }
        public void RegisterDictionaryPatch<Tkey,TValue>(string asset, Dictionary<Tkey,TValue> patch)
        {
            asset = Normalize(asset);
            if (!DictionaryInjector.AssetMap.ContainsKey(asset))
                DictionaryInjector.AssetMap.Add(asset, new List<object>());
            DictionaryInjector.AssetMap[asset].Add(patch);
            if (DictionaryInjector.AssetCache.ContainsKey(asset))
                DictionaryInjector.AssetCache.Remove(asset);
        }
        public void RegisterDictionaryPatch<TKey,TValue>(string asset, string patch)
        {
            RegisterDictionaryPatch(asset, Load<Dictionary<TKey, TValue>>(patch));
        }
        public void RegisterXnbReplacement(string asset, string replacement)
        {
            XnbLoader.AssetMap.Add(Normalize(asset), Path.Combine(ModPath,replacement));
        }
        public void RegisterLoader<T>(string asset, AssetLoader<T> loader)
        {
            DelegatedContentHandler.AssetLoadMap.Add(Normalize(asset), loader);
        }
        public void RegisterLoader<T>(AssetLoader<T> loader)
        {
            DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
        }
        public void RegisterInjector<T>(string asset, AssetInjector<T> injector)
        {
            asset = Normalize(asset);
            if (!DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
        }
        public void RegisterInjector<T>(AssetInjector<T> injector)
        {
            if (!DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
        }
    }
}
