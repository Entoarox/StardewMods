using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework
{
    using Core;
    using Core.ContentHelper;
    using Core.AssetHandlers;
    public static class IContentHelperExtensions
    {
        /// <summary>
        /// Allows you to add a new type to the serializer, provided the serializer has not yet been initialized.
        /// </summary>
        /// <typeparam name="T">The Type to add</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        public static void RegisterSerializerType<T>(this IContentHelper helper)
        {
            if (ModEntry.SerializerInjected)
                ModEntry.Logger.Log("[IContentHelper] The `" + Utilities.ModName(helper) + "` mod failed to augment the serializer, serializer has already been created.", LogLevel.Error);
            else if (!ModEntry.SerializerTypes.Contains(typeof(T)))
                ModEntry.SerializerTypes.Add(typeof(T));
            else
                ModEntry.Logger.Log("[IContentHelper] The `" + Utilities.ModName(helper) + "` mod failed to augment the serializer, the `"+typeof(T).FullName+"` type has already been injected before.", LogLevel.Warn);
        }
        /// <summary>
        /// Enables you to add a custom <see cref="IContentHandler"/> that the content manager will process for content
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="handler">Your content handler implementation</param>
        public static void RegisterContentHandler(this IContentHelper helper, IContentHandler handler)
        {
            try
            {
                if (handler.IsInjector)
                    if (handler.IsLoader)
                    {
                        WrappedHandler wrappedHandler = new WrappedHandler(handler);
                        Utilities.AssetEditors.Add(wrappedHandler);
                        Utilities.AssetLoaders.Add(wrappedHandler);
                    }
                    else
                        Utilities.AssetEditors.Add(new WrappedInjector(handler));
                else
                    Utilities.AssetLoaders.Add(new WrappedLoader(handler));
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("[IContentHelper] The `" + Utilities.ModName(helper) + "` mod's attempt to register a ContentHandler threw a exception, the error message follows." + Environment.NewLine + err.ToString(), LogLevel.Error);
            }
        }
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// The texture asset referenced by patchAssetName has to be in xnb format
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAssetName">The texture asset (Relative to your mod directory and without extension) used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, string patchAssetName, Rectangle? destination = null, Rectangle? source = null)
        {
        }
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAssetName">The texture used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, Texture2D patchAsset, Rectangle? destination = null, Rectangle? source = null)
        {

        }
        /// <summary>
        /// Lets you add and replace keys in a content dictionary
        /// The dictionary asset referenced by patchAssetName has to be in xnb format
        /// </summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAsset">The dictionary asset (Relative to your mod directory and without extension) used for the modification</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDictionaryPatch<TKey, TValue>(this IContentHelper helper, string assetName, string patchAssetName)
        {

        }
        /// <summary>
        /// Lets you add and replace keys in a content dictionary
        /// </summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAsset">The dictionary used for the modification</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDictionaryPatch<TKey, TValue>(this IContentHelper helper, string assetName, Dictionary<TKey, TValue> patchAsset)
        {

        }
        /// <summary>
        /// Lets you define a xnb file to completely replace with another
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to replace</param>
        /// <param name="replacementAssetName">The asset (Relative to your mod directory and without extension) to use instead</param>
        public static void RegisterXnbReplacement(this IContentHelper helper, string assetName, string replacementAssetName)
        {
            if (XnbLoader._Map.ContainsKey(assetName))
                ModEntry.Logger.Log("[IContentHelper] The `" + Utilities.ModName(helper) + "` mod's attempt to register a replacement asset for the `" + assetName + "` asset failed, as another mod has already done so.", LogLevel.Error);
            else
                XnbLoader._Map.Add(assetName, (helper, replacementAssetName));
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="assetLoader">The delegate assigned to handle loading for this asset</param>
        public static void RegisterLoader<T>(this IContentHelper helper, string assetName, AssetLoader<T> assetLoader)
        {
            if (DeferredAssetHandler._LoadMap.ContainsKey((typeof(T), assetName)))
                ModEntry.Logger.Log("[IContentHelper] The `" + Utilities.ModName(helper) + "` mod's attempt to register a replacement asset for the `" + assetName + "` asset of type `" + typeof(T).FullName + "` failed, as another mod has already done so.", LogLevel.Error);
            else
                DeferredAssetHandler._LoadMap.Add((typeof(T), assetName), assetLoader);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for one specific asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle</param>
        /// <param name="assetInjector">The delegate assigned to handle injection for this asset</param>
        public static void RegisterInjector<T>(this IContentHelper helper, string assetName, AssetInjector<T> assetInjector)
        {
            if (!DeferredAssetHandler._EditMap.ContainsKey((typeof(T), assetName)))
                DeferredAssetHandler._EditMap.Add((typeof(T), assetName), new List<Delegate>());
            else
                DeferredAssetHandler._EditMap[(typeof(T), assetName)].Add(assetInjector);
        }
        /// <summary>
        /// If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for a specific type of asset
        /// </summary>
        /// <typeparam name="T">The Type the asset is loaded as</typeparam>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetInjector">The delegate assigned to handle loading for this type</param>
        public static void RegisterInjector<T>(this IContentHelper helper, AssetInjector<T> assetInjector)
        {
            if (DeferredTypeHandler._EditMap.ContainsKey(typeof(T)))
                DeferredTypeHandler._EditMap.Add(typeof(T), new List<Delegate>());
            else
                DeferredTypeHandler._EditMap[typeof(T)].Add(assetInjector);
        }
        public static string GetPlatformRelativeContent(this IContentHelper helper)
        {
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
        }
    }
}
