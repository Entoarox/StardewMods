using System;
using System.Reflection;
using System.Collections.Generic;

using StardewModdingAPI.Content.Utilities;

namespace StardewModdingAPI.Content.Plugins
{
    class DictionaryInjector : IContentHandler
    {
        internal static Dictionary<string, List<object>> AssetMap = new Dictionary<string, List<object>>();
        internal static Dictionary<string, object> AssetCache = new Dictionary<string, object>();

        // Injector
        public bool IsInjector { get; } = true;
        public bool CanInject<T>(string assetName)
        {
            return typeof(T).Is(typeof(Dictionary<,>)) && AssetMap.ContainsKey(assetName);
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            if (!AssetCache.ContainsKey(assetName))
            {
                T copy = asset;
                typeof(DictionaryHelper)
                    .GetMethod(nameof(DictionaryHelper.InjectPairs), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(T).GetGenericArguments())
                    .Invoke(null, new object[] { copy, AssetMap[assetName] });
                AssetCache.Add(assetName, copy);
            }
            asset = (T)AssetCache[assetName];
        }

        // Loader
        public bool IsLoader { get; } = false;
        public bool CanLoad<T>(string assetName)
        {
            throw new NotImplementedException();
        }
        public T Load<T>(string assetName, Func<string,T> loadBase)
        {
            throw new NotImplementedException();
        }
    }
}
