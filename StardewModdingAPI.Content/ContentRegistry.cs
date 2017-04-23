using System;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Content.Utilities;

namespace StardewModdingAPI.Content
{
    class ContentRegistry
    {
        private IMod Mod;
        private IMonitor Monitor;
        public delegate T AssetLoader<T>(string assetName, Func<string, T> loadBase);
        public delegate void AssetInjector<T>(string assetName, ref T asset);
        public ContentRegistry(IMonitor monitor, IMod mod)
        {
            Monitor = monitor;
            Mod = mod;
        }
        /// <summary>
        /// Enables you to add a custom <see cref="IContentHandler"/> that the content manager will process for content
        /// </summary>
        /// <param name="handler">Your content handler implementation</param>
        public void RegisterContentHandler(IContentHandler handler)
        {
            Monitor.Log("Custom content handler added by mod: "+Mod.ModManifest.Name, LogLevel.Trace);
            ExtendibleContentManager.AddContentHandler(handler);
        }
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// Note that both the source and patch texture need to be packaged into xnb files
        /// </summary>
        /// <param name="asset">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patch">The texture asset (Relative to Mods and without extension) used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        public void RegisterTexturePatch(string asset, string patch, Rectangle region)
        {
            Monitor.Log("Texture patch registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!Plugins.TextureLoader.AssetMap.ContainsKey(asset))
                    Plugins.TextureLoader.AssetMap.Add(asset, new List<TextureData>());
                Plugins.TextureLoader.AssetMap[asset].Add(new TextureData(patch, region));
            if (Plugins.TextureLoader.AssetCache.ContainsKey(asset))
                Plugins.TextureLoader.AssetCache.Remove(asset);
        }
        /// <summary>
        /// Lets you define a xnb file to completely replace with another
        /// This will only work if none of the more specific loaders deal with the file first
        /// </summary>
        /// <param name="asset">The asset (Relative to Content and without extension) to replace</param>
        /// <param name="replacement">The asset (Relative to Mods and without extension) to use instead</param>
        public void RegisterXnbReplacement(string asset, string replacement)
        {
            Monitor.Log("Xnb replacement registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            Plugins.XnbLoader.AssetMap.Add(asset, replacement);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="asset">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="loader">The delegate assigned to handle loading for this asset</param>
        public void RegisterLoader<T>(string asset, AssetLoader<T> loader)
        {
            Monitor.Log("Asset loader registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            Plugins.DelegatedContentHandler.AssetLoadMap.Add(asset, loader);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="loader">The delegate assigned to handle loading for this type</param>
        public void RegisterLoader<T>(AssetLoader<T> loader)
        {
            Monitor.Log("Type loader registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            Plugins.DelegatedContentHandler.TypeLoadMap.Add(typeof(T), loader);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="asset">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="injector">The delegate assigned to handle injection for this asset</param>
        public void RegisterInjector<T>(string asset, AssetInjector<T> injector)
        {
            Monitor.Log("Asset injector registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!Plugins.DelegatedContentHandler.AssetInjectMap.ContainsKey(asset))
                Plugins.DelegatedContentHandler.AssetInjectMap.Add(asset, new List<Delegate>());
            Plugins.DelegatedContentHandler.AssetInjectMap[asset].Add(injector);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="injector">The delegate assigned to handle loading for this type</param>
        public void RegisterInjector<T>(AssetInjector<T> injector)
        {
            Monitor.Log("Type injector registered by mod: " + Mod.ModManifest.Name, LogLevel.Trace);
            if (!Plugins.DelegatedContentHandler.TypeInjectMap.ContainsKey(typeof(T)))
                Plugins.DelegatedContentHandler.TypeInjectMap.Add(typeof(T), new List<Delegate>());
            Plugins.DelegatedContentHandler.TypeInjectMap[typeof(T)].Add(injector);
        }
    }
}
