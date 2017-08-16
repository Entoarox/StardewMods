using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    public class DelegatedAssetContentHandler : ContentHandler
    {
        public override bool Injector { get; } = true;
        public override bool Loader { get; } = true;
        public delegate void InjectHandler<T>(string assetName, ref T asset);
        public delegate T Loadhandler<T>(string assetName);
        private static Dictionary<Asset, Delegate> Loaders = new Dictionary<Asset, Delegate>();
        private static Dictionary<Asset, List<Delegate>> Injectors = new Dictionary<Asset, List<Delegate>>();

        public static void RegisterLoader<T>(string assetName, Loadhandler<T> handler)
        {
            Loaders.Add(new Asset(typeof(T), GetPlatformSafePath(assetName)), handler);
        }
        public static void RegisterInjector<T>(string assetName, InjectHandler<T> handler)
        {
            Asset asset = new Asset(typeof(T), GetPlatformSafePath(assetName));
            if (!Injectors.ContainsKey(asset))
                Injectors.Add(asset, new List<Delegate>());
            Injectors[asset].Add(handler);
        }

        public override bool CanInject<T>(string assetName)
        {
            return Injectors.ContainsKey(new Asset(typeof(T), assetName));
        }
        public override bool CanLoad<T>(string assetName)
        {
            return Loaders.ContainsKey(new Asset(typeof(T), assetName));
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            foreach (InjectHandler<T> injector in Injectors[new Asset(typeof(T), assetName)])
                injector(assetName, ref asset);
        }
        public override T Load<T>(string assetName)
        {
            return ((Loadhandler<T>)Loaders[new Asset(typeof(T), assetName)])(assetName);
        }
    }
}
