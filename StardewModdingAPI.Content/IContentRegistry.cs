using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModdingAPI.Content
{
    public interface IContentRegistry
    {
        /// <summary>
        /// Load a asset relative to your mod directory
        /// </summary>
        /// <typeparam name="T">The type of the asset</typeparam>
        /// <param name="assetName">The relative path to the asset, without extension</param>
        /// <returns></returns>
        T Load<T>(string assetName);
        /// <summary>
        /// Enables you to add a custom <see cref="IContentHandler"/> that the content manager will process for content
        /// </summary>
        /// <param name="handler">Your content handler implementation</param>
        void RegisterContentHandler(IContentHandler handler);
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// The texture asset referenced by patchAssetName has to be in xnb format
        /// </summary>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAssetName">The texture asset (Relative to your mod directory and without extension) used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used</param>
        void RegisterTexturePatch(string assetName, string patchAssetName, Rectangle destination, Rectangle? source = null);
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// </summary>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAssetName">The texture used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used</param>
        void RegisterTexturePatch(string assetName, Texture2D patchAsset, Rectangle destination, Rectangle? source = null);
        /// <summary>
        /// Lets you add and replace keys in a content dictionary
        /// The dictionary asset referenced by patchAssetName has to be in xnb format
        /// </summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary</typeparam>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAsset">The dictionary asset (Relative to your mod directory and without extension) used for the modification</param>
        void RegisterDictionaryPatch<TKey, TValue>(string assetName, string patchAssetName);
        /// <summary>
        /// Lets you add and replace keys in a content dictionary
        /// </summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary</typeparam>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAsset">The dictionary used for the modification</param>
        void RegisterDictionaryPatch<TKey, TValue>(string assetName, Dictionary<TKey, TValue> patchAsset);
        /// <summary>
        /// Lets you define a xnb file to completely replace with another
        /// This will only work if none of the more specific loaders deal with the file first
        /// </summary>
        /// <param name="assetName">The asset (Relative to Content and without extension) to replace</param>
        /// <param name="replacementAssetName">The asset (Relative to your mod directory and without extension) to use instead</param>
        void RegisterXnbReplacement(string assetName, string replacementAssetName);
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="assetLoader">The delegate assigned to handle loading for this asset</param>
        void RegisterLoader<T>(string assetName, AssetLoader<T> assetLoader);
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="assetLoader">The delegate assigned to handle loading for this type</param>
        void RegisterLoader<T>(AssetLoader<T> assetLoader);
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="assetInjector">The delegate assigned to handle injection for this asset</param>
        void RegisterInjector<T>(string assetName, AssetInjector<T> assetInjector);
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="assetInjector">The delegate assigned to handle loading for this type</param>
        void RegisterInjector<T>(AssetInjector<T> assetInjector);
    }
}
