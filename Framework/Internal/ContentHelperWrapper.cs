using System;
using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;

namespace Entoarox.Framework.Internal
{
    /// <summary>Wraps <see cref="IContentHelper"/> to provide access to private fields until SMAPI 2.0 is released.</summary>
    internal class ContentHelperWrapper
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying content helper.</summary>
        private readonly IContentHelper ContentHelper;

        /// <summary>The method which invalidates an asset key from the cache.</summary>
        private readonly MethodInfo InvalidateCacheMethod;


        /*********
        ** Accessors
        *********/
        /// <summary>Interceptors which edit matching content assets after they're loaded.</summary>
        public IList<IAssetLoader> AssetLoaders;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentHelper">The underlying content helper.</param>
        /// <param name="monitor">Writes messages to the log and allows exiting the game.</param>
        public ContentHelperWrapper(IContentHelper contentHelper, IMonitor monitor)
        {
            this.ContentHelper = contentHelper;

            // get real type
            Type type = typeof(IContentHelper).Assembly.GetType("StardewModdingAPI.Framework.ModHelpers.ContentHelper");
            if (type == null)
            {
                monitor.ExitGameImmediately("Could not access SMAPI's internal content helper. Make sure you have the latest version of both Entoarox Framework and SMAPI.");
                return;
            }

            // get asset loaders
            {
                PropertyInfo property = type.GetProperty("AssetLoaders", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (property == null)
                {
                    monitor.ExitGameImmediately("Could not access SMAPI's asset loaders. Make sure you have the latest version of both Entoarox Framework and SMAPI.");
                    return;
                }
                this.AssetLoaders = (IList<IAssetLoader>)property.GetValue(contentHelper);
            }

            // get InvalidateCache method
            {
                MethodInfo method = type.GetMethod("InvalidateCache", new[] { typeof(string) });
                if (method == null)
                {
                    monitor.ExitGameImmediately("Could not access SMAPI's cache invalidation method. Make sure you have the latest version of both Entoarox Framework and SMAPI.");
                    return;
                }
                this.InvalidateCacheMethod = method;
            }
        }

        /// <summary>Remove an asset from the content cache so it's reloaded on the next request. This will reload core game assets if needed, but references to the former asset will still show the previous content.</summary>
        /// <param name="key">The asset key to invalidate in the content folder.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is empty or contains invalid characters.</exception>
        /// <returns>Returns whether the given asset key was cached.</returns>
        public bool InvalidateCache(string key)
        {
            return (bool)this.InvalidateCacheMethod.Invoke(this.ContentHelper, new object[] { key });
        }

        /// <summary>Load content from the game folder or mod folder (if not already cached), and return it. When loading a <c>.png</c> file, this must be called outside the game's draw loop.</summary>
        /// <typeparam name="T">The expected data type. The main supported types are <see cref="T:Microsoft.Xna.Framework.Graphics.Texture2D" /> and dictionaries; other types may be supported by the game's content pipeline.</typeparam>
        /// <param name="key">The asset key to fetch (if the <paramref name="source" /> is <see cref="F:StardewModdingAPI.ContentSource.GameContent" />), or the local path to a content file relative to the mod folder.</param>
        /// <param name="source">Where to search for a matching content asset.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="key" /> is empty or contains invalid characters.</exception>
        /// <exception cref="T:Microsoft.Xna.Framework.Content.ContentLoadException">The content asset couldn't be loaded (e.g. because it doesn't exist).</exception>
        public T Load<T>(string key, ContentSource source = ContentSource.ModFolder) => this.ContentHelper.Load<T>(key, source);
    }
}