using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

namespace Entoarox.Framework
{
    public interface IContentHelper// : IContentHelper
    {
        /// <summary>Load content from the game folder or mod folder (if not already cached), and return it.</summary>
        /// <typeparam name="T">The expected data type. The main supported types are <see cref="Texture2D"/> and dictionaries; other types may be supported by the game's content pipeline.</typeparam>
        /// <param name="key">The asset key to fetch (if the <paramref name="source"/> is <see cref="ContentSource.GameContent"/>), or the local path to a content file relative to the mod folder.</param>
        /// <param name="source">Where to search for a matching content asset.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is empty or contains invalid characters.</exception>
        /// <exception cref="ContentLoadException">The content asset couldn't be loaded (e.g. because it doesn't exist).</exception>
        T Load<T>(string key, ContentSource source = ContentSource.ModFolder);
        /// <summary>Get the underlying key in the game's content cache for an asset. This can be used to load custom map tilesheets, but should be avoided when you can use the content API instead. This does not validate whether the asset exists.</summary>
        /// <param name="key">The asset key to fetch (if the <paramref name="source"/> is <see cref="ContentSource.GameContent"/>), or the local path to a content file relative to the mod folder.</param>
        /// <param name="source">Where to search for a matching content asset.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is empty or contains invalid characters.</exception>
        string GetActualAssetKey(string key, ContentSource source = ContentSource.ModFolder);
        void RegisterContentHandler(IContentHandler handler);
        void RegisterTexturePatch(string assetName, string patchAssetName, Rectangle? destination = null, Rectangle? source = null);
        void RegisterTexturePatch(string assetName, Texture2D patchAsset, Rectangle? destination = null, Rectangle? source = null);
        void RegisterDictionaryPatch<TKey, TValue>(string assetName, string patchAssetName);
        void RegisterDictionaryPatch<TKey, TValue>(string assetName, Dictionary<TKey, TValue> patchAsset);
        void RegisterXnbReplacement(string assetName, string replacementAssetName);
        void RegisterLoader<T>(string assetName, AssetLoader<T> assetLoader);
        void RegisterLoader<T>(AssetLoader<T> assetLoader);
        void RegisterInjector<T>(string assetName, AssetInjector<T> assetInjector);
        void RegisterInjector<T>(AssetInjector<T> assetInjector);
    }
}
