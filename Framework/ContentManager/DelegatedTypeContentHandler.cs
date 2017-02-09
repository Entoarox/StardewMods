using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    class DelegatedTypeContentHandler : ContentHandler
    {
        public delegate void Injector<T>(string assetName, ref T asset);
        public delegate T Loader<T>(string assetName, Func<string, T> loadBase);
        private static Dictionary<Type, Delegate> Loaders = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, List<Delegate>> Injectors = new Dictionary<Type, List<Delegate>>();

        public static void RegisterLoader<T>(Loader<T> handler)
        {
            Loaders.Add(typeof(T), handler);
        }
        public static void RegisterInjector<T>(Injector<T> handler)
        {
            if (!Injectors.ContainsKey(typeof(T)))
                Injectors.Add(typeof(T), new List<Delegate>());
            Injectors[typeof(T)].Add(handler);
        }

        public override bool CanInject<T>(string assetName)
        {
            return Injectors.ContainsKey(typeof(T));
        }
        public override bool CanLoad<T>(string assetName)
        {
            return Loaders.ContainsKey(typeof(T));
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            foreach (Injector<T> injector in Injectors[typeof(T)])
                injector(GetPlatformSafePath(assetName), ref asset);
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            return ((Loader<T>)Loaders[typeof(T)])(GetPlatformSafePath(assetName), loadBase);
        }
    }
}
