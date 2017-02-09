using System;
using System.Collections.Generic;

namespace Entoarox.Framework.ContentManager
{
    class DictionaryContentInjector : ContentHandler
    {
        private static Dictionary<string, List<string>> Mapping=new Dictionary<string, List<string>>();
        private static Dictionary<string, object> Cache = new Dictionary<string, object>();
        private static void InjectPairs<TKey,TValue>(Dictionary<TKey,TValue> dictionary, string assetName)
        {
            foreach (string file in Mapping[assetName])
                try
                {
                    foreach (KeyValuePair<TKey, TValue> pair in ModManager.Load<Dictionary<TKey, TValue>>(file))
                        if (dictionary.ContainsKey(pair.Key))
                            dictionary[pair.Key] = pair.Value;
                        else
                            dictionary.Add(pair.Key, pair.Value);
                }
                catch(Exception err)
                {
                    EntoFramework.Logger.Log(StardewModdingAPI.LogLevel.Error, "ContentManager: DictionaryContentInjector failed to inject data into `" + assetName + "` from file:" + file, err);
                }
        }
        public static void Register(string assetName, string file)
        {
            if (!Mapping.ContainsKey(assetName))
                Mapping.Add(assetName, new List<string>());
            Mapping[assetName].Add(GetModsRelativePath(GetPlatformSafePath(file)));
        }
        public override bool CanInject<T>(string assetName)
        {
            return typeof(T).GetInterface("IDictionary`2")!=null;
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            if (!Cache.ContainsKey(assetName))
            {
                T copy = asset;
                var method = typeof(DictionaryContentInjector).GetMethod("InjectPairs").MakeGenericMethod(typeof(T).GetGenericArguments());
                method.Invoke(null, new object[] { copy, assetName });
                Cache.Add(assetName, copy);
            }
            asset = (T)Cache[assetName];
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            throw new NotImplementedException();
        }
    }
}
