using System;
using System.Collections;
using System.Collections.Generic;

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
            return typeof(IDictionary).IsAssignableFrom(typeof(T)) && AssetMap.ContainsKey(assetName);
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            if (!AssetCache.ContainsKey(assetName))
            {
                var dictionary = asset as IDictionary;
                foreach(IDictionary patch in AssetMap[assetName])
                {
                    if (patch.GetType() != typeof(T))
                        continue;
                    foreach (var key in patch)
                        if (dictionary.Contains(key))
                            dictionary[key] = patch[key];
                        else
                            dictionary.Add(key, patch[key]);
                }
                AssetCache[assetName] = dictionary;
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
