using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    public class DelegatedAssetContentHandler : ContentHandler
    {
        public delegate void Injector<T>(string assetName, ref T asset);
        public delegate T Loader<T>(string assetName, Func<string, T> loadBase);
        private static Dictionary<Asset, Delegate> Loaders = new Dictionary<Asset, Delegate>();
        private static Dictionary<Asset, List<Delegate>> Injectors = new Dictionary<Asset, List<Delegate>>();

        public static void RegisterLoader<T>(string assetName, Loader<T> handler)
        {
            Loaders.Add(new Asset(typeof(T), GetPlatformSafePath(assetName)), handler);
        }
        public static void RegisterInjector<T>(string assetName, Injector<T> handler)
        {
            Asset asset = new Asset(typeof(T), GetPlatformSafePath(assetName));
            if (!Injectors.ContainsKey(asset))
                Injectors.Add(asset, new List<Delegate>());
            Injectors[asset].Add(handler);
        }

        public override bool CanInject<T>(string assetName)
        {
            return Injectors.ContainsKey(new Asset(typeof(T), GetPlatformSafePath(assetName)));
        }
        public override bool CanLoad<T>(string assetName)
        {
            return Loaders.ContainsKey(new Asset(typeof(T), GetPlatformSafePath(assetName)));
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            foreach (Injector<T> injector in Injectors[new Asset(typeof(T), GetPlatformSafePath(assetName))])
                injector(GetPlatformSafePath(assetName), ref asset);
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            return ((Loader<T>)Loaders[new Asset(typeof(T), GetPlatformSafePath(assetName))])(GetPlatformSafePath(assetName), loadBase);
        }
    }
}
