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
    using Core.Utilities;
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
            if (EntoaroxFrameworkMod.SerializerInjected)
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod failed to augment the serializer, serializer has already been created.", LogLevel.Error);
            else if (!EntoaroxFrameworkMod.SerializerTypes.Contains(typeof(T)))
                EntoaroxFrameworkMod.SerializerTypes.Add(typeof(T));
            else
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod failed to augment the serializer, the `"+typeof(T).FullName+"` type has already been injected before.", LogLevel.Warn);
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
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, string patchAssetName, Rectangle? destination = null, Rectangle? source = null)
        {
            RegisterTexturePatch(helper, assetName, helper.Load<Texture2D>(patchAssetName), destination, source);
        }
        /// <summary>
        /// Lets you replace a region of pixels in one texture with the contents of another texture
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify</param>
        /// <param name="patchAssetName">The texture used for the modification</param>
        /// <param name="region">The area you wish to replace</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used</param>
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, Texture2D patchAsset, Rectangle? destination = null, Rectangle? source = null)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (!TextureInjector._Map.ContainsKey(assetName))
                TextureInjector._Map.Add(assetName, new List<(Texture2D, Rectangle?, Rectangle?)>());
            TextureInjector._Map[assetName].Add((patchAsset, source, destination));
            helper.InvalidateCache(assetName);
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
            try
            {
                RegisterDictionaryPatch(helper, assetName, helper.Load<Dictionary<TKey, TValue>>(patchAssetName));
            }
            catch
            {
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod's attempt to inject data into the `" + assetName + "` asset failed, as the TKey and TValue given do not match those of the data to inject", LogLevel.Error);
            }
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
            try
            {
                var check = Game1.content.Load<Dictionary<TKey, TValue>>(assetName);
                assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
                if (!DictionaryInjector._Map.ContainsKey(assetName))
                    DictionaryInjector._Map.Add(assetName, new DictionaryWrapper<TKey, TValue>());
                (DictionaryInjector._Map[assetName] as DictionaryWrapper<TKey, TValue>).Register(helper, patchAsset);
                helper.InvalidateCache(assetName);
            }
            catch
            {
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod's attempt to inject data into the `" + assetName + "` asset failed, as the TKey and TValue of the injected asset do not match the original.", LogLevel.Error);
            }
        }
        /// <summary>
        /// Lets you define a xnb file to completely replace with another
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to replace</param>
        /// <param name="replacementAssetName">The asset (Relative to your mod directory and without extension) to use instead</param>
        public static void RegisterXnbReplacement(this IContentHelper helper, string assetName, string replacementAssetName)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            replacementAssetName = helper.GetActualAssetKey(replacementAssetName, ContentSource.GameContent);
            if (XnbLoader._Map.ContainsKey(assetName))
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod's attempt to register a replacement asset for the `" + assetName + "` asset failed, as another mod has already done so.", LogLevel.Error);
            else
            {
                XnbLoader._Map.Add(assetName, (helper, replacementAssetName));
                helper.InvalidateCache(assetName);
            }
        }

        /// <summary>
        /// Lets you define a xnb file to completely replace with another
        /// </summary>
        /// <param name="helper">The <see cref="IContentHelper"/> this extension method is attached to</param>
        /// <param name="assetMapping">Dictionary with the asset (Relative to Content and without extension) to replace as key, and the asset (Relative to your mod directory and without extension) to use instead as value</param>
        public static void RegisterXnbReplacements(this IContentHelper helper, Dictionary<string,string> assetMapping)
        {
            List<string> matchedAssets = new List<string>();
            foreach (var pair in assetMapping)
            {
                string asset = helper.GetActualAssetKey(pair.Key, ContentSource.GameContent);
                string replacement = helper.GetActualAssetKey(pair.Value, ContentSource.GameContent);
                if (XnbLoader._Map.ContainsKey(asset))
                {
                    EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod's attempt to register a replacement asset for the `" + pair.Key + "` asset failed, as another mod has already done so.", LogLevel.Error);
                }
                else
                {
                    XnbLoader._Map.Add(asset, (helper, replacement));
                    matchedAssets.Add(asset);
                }
            }
            helper.InvalidateCache((assetInfo) => matchedAssets.Contains(assetInfo.AssetName));
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
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (DeferredAssetHandler._LoadMap.ContainsKey((typeof(T), assetName)))
                EntoaroxFrameworkMod.Logger.Log("[IContentHelper] The `" + Globals.GetModName(helper) + "` mod's attempt to register a replacement asset for the `" + assetName + "` asset of type `" + typeof(T).FullName + "` failed, as another mod has already done so.", LogLevel.Error);
            else
            {
                DeferredAssetHandler._LoadMap.Add((typeof(T), assetName), assetLoader);
                helper.InvalidateCache(assetName);
            }
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
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (!DeferredAssetHandler._EditMap.ContainsKey((typeof(T), assetName)))
                DeferredAssetHandler._EditMap.Add((typeof(T), assetName), new List<Delegate>());
            DeferredAssetHandler._EditMap[(typeof(T), assetName)].Add(assetInjector);
            helper.InvalidateCache(assetName);
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
            DeferredTypeHandler._EditMap[typeof(T)].Add(assetInjector);
            helper.InvalidateCache<T>();
        }
        public static string GetPlatformRelativeContent(this IContentHelper helper)
        {
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs")) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
        }
    }
}
