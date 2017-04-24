using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley;

namespace StardewModdingAPI.Content.Internal
{
    using Plugins;

    public class ExtendibleContentManager : LocalizedContentManager
    {
        private static List<IContentHandler> Handlers;
        private static IContentHandler[] _Inject;
        private static IContentHandler[] _Load;

        internal static LocalizedContentManager ModContent;
        public ExtendibleContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider,rootDirectory)
        {
            if (Handlers != null)
                return;
            Handlers = new List<IContentHandler>();
            // Specific loaders / injectors for certain files & file types
            Handlers.Add(new TextureInjector());
            Handlers.Add(new DictionaryInjector());
            // Second to last, as it is a explicit redirect of xnb files from Content to somewhere else
            Handlers.Add(new XnbLoader());
            // Always last, so that content handlers that do things in a fixed way have priority.
            Handlers.Add(new DelegatedContentHandler());
            // Manually filter the injector and loader sets
            _Inject = Handlers.Where(a => a.IsInjector).ToArray();
            _Load = Handlers.Where(a => a.IsLoader).ToArray();

            ModContent = new LocalizedContentManager(Game1.content.ServiceProvider, System.IO.Path.Combine(Constants.ExecutionPath, "Mods"));
        }
        internal static void AddContentHandler(IContentHandler handler)
        {
            Handlers.Add(handler);
            _Inject = Handlers.Where(a => a.IsInjector).ToArray();
            _Load = Handlers.Where(a => a.IsLoader).ToArray();
        }
        public override T Load<T>(string assetName)
        {
            assetName = assetName.Replace('/', '\\');
            T asset = default(T);
            var Loaders = _Load.Where(a => a.CanLoad<T>(assetName)).ToArray();
            if (Loaders.Length > 1)
                Console.WriteLine($"Multiple loaders found for type {typeof(T)} asset: {assetName}");
            if (Loaders.Length == 0)
                asset = base.Load<T>(assetName);
            else
                asset = Loaders[0].Load(assetName, base.Load<T>);
            var Injectors = _Inject.Where(a => a.CanInject<T>(assetName));
            foreach (var Injector in Injectors)
                Injector.Inject(assetName, ref asset);
            return asset;
        }
    }
}
