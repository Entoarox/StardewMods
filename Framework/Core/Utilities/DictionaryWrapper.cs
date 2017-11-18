using System;
using System.Collections.Generic;

using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    // The DictionaryInjector would need a lot of reflection to do its job, wrapping the asset while we still know the generic types works around that limitation
    abstract class DictionaryWrapper
    {
        public abstract Type DataType { get; }
        public abstract void ApplyPatches(IAssetData data);
    }
    class DictionaryWrapper<TKey, TValue> : DictionaryWrapper
    {
        public override Type DataType { get; } = typeof(Dictionary<TKey, TValue>);
        private List<(IContentHelper, Dictionary<TKey, TValue>)> _Map = new List<(IContentHelper, Dictionary<TKey, TValue>)>();
        public void Register(IContentHelper helper, Dictionary<TKey, TValue> data)
        {
            this._Map.Add((helper, data));
        }

        public override void ApplyPatches(IAssetData data)
        {
            var dict = data.AsDictionary<TKey, TValue>().Data;
            var merge = new Dictionary<TKey, TValue>();
            foreach (var patch in this._Map)
                foreach(var pair in patch.Item2)
                {
                    if (dict.ContainsKey(pair.Key) && dict[pair.Key].Equals(pair.Value))
                        continue;
                    if (!merge.ContainsKey(pair.Key))
                        merge.Add(pair.Key, pair.Value);
                    else if(!merge[pair.Key].Equals(pair.Value))
                        ModEntry.Logger.Log("[IContentHelper] The `" + Globals.GetModName(patch.Item1) + "` mod tried to modify the `" + data.AssetName + "` dictionary by setting the `" + pair.Key + "` key, but another mod has already set it", LogLevel.Warn);
                }
            foreach(var pair in merge)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] = pair.Value;
                else
                    dict.Add(pair.Key, pair.Value);
            }
        }
    }
}
