using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace Entoarox.Framework.ContentManager
{
    internal class SmartContentInterceptor : IAssetEditor, IAssetLoader
    {
        /*********
        ** Properties
        *********/
        private List<ContentHandler> ContentHandlers { get; } = new List<ContentHandler>
        {
            new MapContentInjector(),
            new TextureContentInjector(),
            new DictionaryContentInjector(),
            new XnbContentLoader(),
            new DelegatedAssetContentHandler(),
            new DelegatedTypeContentHandler()
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return this.GetLoaders<T>(asset).Any();
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return this.GetEditors<T>(asset).Any();
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            // get loaders
            ContentHandler[] loaders = this.GetLoaders<T>(asset).ToArray();
            if (!loaders.Any())
                throw new InvalidOperationException("ContentManager: no asset loaders found. This shouldn't happen.");
            if (loaders.Length > 1)
                EntoFramework.Logger.Log($"ContentManager: multiple loaders for `{asset.AssetName}` found, using first.", LogLevel.Warn);

            // load asset
            return loaders[0].Load<T>(asset.AssetName);
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            var data = asset.Data;
            var injectors = this.GetEditors<T>(asset).ToArray();
            foreach (ContentHandler injector in injectors)
                injector.Inject(asset.AssetName, ref data);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get all editors which can handle the given asset.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset metadata.</param>
        private IEnumerable<ContentHandler> GetEditors<T>(IAssetInfo asset)
        {
            return this.ContentHandlers.Where(a => a.Injector && a.CanInject<T>(asset.AssetName));
        }

        /// <summary>Get all loaders which can handle the given asset.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset metadata.</param>
        private IEnumerable<ContentHandler> GetLoaders<T>(IAssetInfo asset)
        {
            return this.ContentHandlers.Where(a => a.Loader && a.CanInject<T>(asset.AssetName));
        }
    }
}
