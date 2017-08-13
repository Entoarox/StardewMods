using System;
using System.Collections.Generic;
using System.IO;
using Entoarox.Framework.ContentManager;
using StardewModdingAPI;

namespace Entoarox.Framework
{
    internal class SmartContentInterceptor : IAssetLoader
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying content helper.</summary>
        private readonly IContentHelper ContentHelper;

        /// <summary>The root path for the Entoarox Framework mod.</summary>
        private readonly string ModPath;

        /// <summary>The asset names intercepted by this instance.</summary>
        private readonly List<string> HandledFiles = new List<string>();

        /// <summary>Maps asset names to the file paths to load instead.</summary>
        private readonly Dictionary<string, string> Redirects = new Dictionary<string, string>();

        /// <summary>Maps asset names to delegates which can load them.</summary>
        private readonly Dictionary<string, KeyValuePair<Type, Delegate>> LoaderDelegate = new Dictionary<string, KeyValuePair<Type, Delegate>>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentHelper">The underlying content helper.</param>
        /// <param name="modPath">The root path for the Entoarox Framework mod.</param>
        public SmartContentInterceptor(IContentHelper contentHelper, string modPath)
        {
            this.ContentHelper = contentHelper;
            this.ModPath = modPath;
        }

        /// <summary>Add a file path to load instead of the given asset name.</summary>
        /// <param name="assetName">The asset name to intercept.</param>
        /// <param name="fileName">The file path to load instead.</param>
        internal void RegisterRedirect(string assetName, string fileName)
        {
            string x = assetName;

            // validate key
            assetName = this.NormaliseAssetName(assetName);
            Console.WriteLine($"Redirecting {x} ({assetName}) => {fileName}");

            if (!this.MarkHandledOrWarn(assetName))
                return;

            // add
            if (File.Exists(fileName) || File.Exists(fileName + ".xnb"))
                this.Redirects.Add(assetName, fileName);
        }

        /// <summary>Register a delegate which loads an asset.</summary>
        /// <typeparam name="T">The expected asset type.</typeparam>
        /// <param name="assetName">The asset name to intercept.</param>
        /// <param name="handler">The handler which loads the asset.</param>
        internal void RegisterHandler<T>(string assetName, FileLoadMethod<T> handler)
        {
            // validate key
            assetName = this.NormaliseAssetName(assetName);
            if (!this.MarkHandledOrWarn(assetName))
                return;

            // add
            this.LoaderDelegate.Add(assetName, new KeyValuePair<Type, Delegate>(typeof(T), handler));
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return this.GetRedirect<T>(asset) != null || this.GetLoaderDelegate<T>(asset) != null;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            // texture redirect
            string redirectPath = this.GetRedirect<T>(asset);
            if (redirectPath != null)
            {
                string relativePath = ContentHandler.GetRelativePath(redirectPath, this.ModPath); // underlying content manager doesn't allow absolute paths
                return this.ContentHelper.Load<T>(relativePath);
            }

            // loader delegate
            FileLoadMethod<T> loader = this.GetLoaderDelegate<T>(asset);
            if (loader != null)
                return loader.Invoke(asset.AssetName);

            // none found?
            throw new InvalidOperationException("ContentManager: no asset loaders found. This shouldn't happen.");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a normalised asset name.</summary>
        /// <param name="assetName">The asset name.</param>
        private string NormaliseAssetName(string assetName)
        {
            return ContentHandler.GetPlatformSafePath(assetName); // don't use contentHelper.GetActualAssetKey because we don't want to turn it into an Entoarox path
        }

        /// <summary>Mark an asset path as handled by this interceptor.</summary>
        /// <param name="assetName">The asset name to mark.</param>
        /// <returns>Returns true if the asset was not previously marked as handled.</returns>
        private bool MarkHandledOrWarn(string assetName)
        {
            // previously registered
            if (this.HandledFiles.Contains(assetName))
            {
                EntoFramework.Logger.Log($"Interceptor: The `{assetName}` file is already being managed and can't be intercepted again. Some mods may not work correctly.", LogLevel.Warn);
                return false;
            }

            // else new
            this.HandledFiles.Add(assetName);
            return true;
        }

        /// <summary>Get the replacement path for a given asset.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset to find.</param>
        private string GetRedirect<T>(IAssetInfo asset)
        {
            string match = null;
            return this.Redirects.TryGetValue(asset.AssetName, out match)
                ? match
                : null;
        }

        /// <summary>Get a loader delegate for a given asset.</summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset to find.</param>
        private FileLoadMethod<T> GetLoaderDelegate<T>(IAssetInfo asset)
        {
            string assetName = asset.AssetName;

            return this.LoaderDelegate.ContainsKey(assetName) && this.LoaderDelegate[assetName].Key == typeof(T)
                ? (FileLoadMethod<T>)this.LoaderDelegate[assetName].Value
                : null;
        }
    }
}
