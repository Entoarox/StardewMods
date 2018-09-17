using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    // The DictionaryInjector would need a lot of reflection to do its job, wrapping the asset while we still know the generic types works around that limitation
    internal abstract class DictionaryWrapper
    {
        /*********
        ** Accessors
        *********/
        public abstract Type DataType { get; }
        public abstract void ApplyPatches(IAssetData data);
    }

    internal class DictionaryWrapper<TKey, TValue> : DictionaryWrapper
    {
        /*********
        ** Fields
        *********/
        private readonly List<(IContentHelper, Dictionary<TKey, TValue>)> Map = new List<(IContentHelper, Dictionary<TKey, TValue>)>();


        /*********
        ** Accessors
        *********/
        public override Type DataType { get; } = typeof(Dictionary<TKey, TValue>);


        /*********
        ** Public methods
        *********/
        public void Register(IContentHelper helper, Dictionary<TKey, TValue> data)
        {
            this.Map.Add((helper, data));
        }

        public override void ApplyPatches(IAssetData data)
        {
            IDictionary<TKey, TValue> dict = data.AsDictionary<TKey, TValue>().Data;
            Dictionary<TKey, TValue> merge = new Dictionary<TKey, TValue>();
            foreach ((IContentHelper, Dictionary<TKey, TValue>) patch in this.Map)
            {
                foreach (KeyValuePair<TKey, TValue> pair in patch.Item2)
                {
                    if (dict.ContainsKey(pair.Key) && dict[pair.Key].Equals(pair.Value))
                        continue;
                    if (!merge.ContainsKey(pair.Key))
                        merge.Add(pair.Key, pair.Value);
                    else if (!merge[pair.Key].Equals(pair.Value))
                        EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(patch.Item1)}` mod tried to modify the `{data.AssetName}` dictionary by setting the `{pair.Key}` key, but another mod has already set it", LogLevel.Warn);
                }
            }

            foreach (KeyValuePair<TKey, TValue> pair in merge)
            {
                if (dict.ContainsKey(pair.Key))
                    dict[pair.Key] = pair.Value;
                else
                    dict.Add(pair.Key, pair.Value);
            }
        }
    }
}
