using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewModdingAPI.Content.Utilities
{
    static class DictionaryHelper
    {
        public static bool Is(this Type @in, Type @as)
        {
            while (@in != null)
            {
                if (@in.IsGenericType && @in.GetGenericTypeDefinition() == @as)
                    return true;
                @in = @in.BaseType;
            }
            return false;
        }
        public static void InjectPairs<TKey, TValue>(Dictionary<TKey, TValue> dictionary, List<object> patches)
        {
            foreach (IDictionary patch in patches)
            {
                if (patch is Dictionary<TKey, TValue>)
                {
                    foreach (KeyValuePair<TKey, TValue> pair in patch)
                    {
                        if (dictionary.ContainsKey(pair.Key))
                            dictionary[pair.Key] = pair.Value;
                        else
                            dictionary.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
    }
}
