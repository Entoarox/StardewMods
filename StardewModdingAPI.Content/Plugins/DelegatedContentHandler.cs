using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewModdingAPI.Content.Plugins
{
    class DelegatedContentHandler : IContentHandler
    {
        internal static Dictionary<string, Delegate> AssetLoadMap = new Dictionary<string, Delegate>();
        internal static Dictionary<Type, Delegate> TypeLoadMap = new Dictionary<Type, Delegate>();
        internal static Dictionary<string, List<Delegate>> AssetInjectMap = new Dictionary<string, List<Delegate>>();
        internal static Dictionary<Type, List<Delegate>> TypeInjectMap = new Dictionary<Type, List<Delegate>>();

        // Loader
        public bool IsLoader { get; } = true;
        public bool CanLoad<T>(string assetName)
        {
            return AssetLoadMap.ContainsKey(assetName) || TypeLoadMap.ContainsKey(typeof(T));
        }
        public T Load<T>(string assetName, Func<string, T> loadBase)
        {
            if (AssetLoadMap.ContainsKey(assetName))
                return ((Func<string, Func<string, T>, T>)AssetLoadMap[assetName])(assetName, loadBase);
            return ((Func<string, Func<string, T>, T>)TypeLoadMap[typeof(T)])(assetName, loadBase);
        }

        // Injector
        public bool IsInjector { get; } = true;
        public bool CanInject<T>(string assetName)
        {
            return AssetInjectMap.ContainsKey(assetName) || TypeInjectMap.ContainsKey(typeof(T));
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            if (TypeInjectMap.ContainsKey(typeof(T)))
                foreach (ContentRegistry.AssetInjector<T> method in TypeInjectMap[typeof(T)])
                    method(assetName, ref asset);
            foreach (ContentRegistry.AssetInjector<T> method in AssetInjectMap[assetName])
                method(assetName, ref asset);
        }

    }
}
