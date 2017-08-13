using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    class DelegatedTypeContentHandler : ContentHandler
    {
        public override bool Injector { get; } = true;
        public override bool Loader { get; } = true;
        public delegate void InjectHandler<T>(string assetName, ref T asset);
        public delegate T Loadhandler<T>(string assetName);
        private static Dictionary<Type, Delegate> Loaders = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, List<Delegate>> Injectors = new Dictionary<Type, List<Delegate>>();

        public static void RegisterLoader<T>(Loadhandler<T> handler)
        {
            Loaders.Add(typeof(T), handler);
        }
        public static void RegisterInjector<T>(InjectHandler<T> handler)
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
            foreach (InjectHandler<T> injector in Injectors[typeof(T)])
                injector(assetName, ref asset);
        }
        public override T Load<T>(string assetName)
        {
            return ((Loadhandler<T>)Loaders[typeof(T)])(assetName);
        }
    }
}
