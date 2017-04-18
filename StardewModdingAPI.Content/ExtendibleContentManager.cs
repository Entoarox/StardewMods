using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley;

namespace StardewModdingAPI.Content
{
    public class ExtendibleContentManager : LocalizedContentManager
    {
        private List<IContentHandler> Handlers = new List<IContentHandler>();
        private IContentHandler[] _Inject;
        private IContentHandler[] _Load;
        public ExtendibleContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider,rootDirectory)
        {
            AddContentHandler(new Plugins.XnbLoader());
        }
        public void AddContentHandler(IContentHandler handler)
        {
            Handlers.Add(handler);
            _Inject = Handlers.Where(a => a.IsInjector).ToArray();
            _Load = Handlers.Where(a => a.IsLoader).ToArray();
        }
        public override T Load<T>(string assetName)
        {
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
