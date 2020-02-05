using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

using StardewValley;

namespace Entoarox.Utilities.Internals.Api
{
    public class EntoUtilsApi : IEntoUtilsApi
    {
        public static readonly EntoUtilsApi Instance;

        private static readonly ConditionalWeakTable<Item, string> ReverseCache = new ConditionalWeakTable<Item, string>();
        private static readonly ConditionalWeakTable<Item, string> ReverseCacheStrict = new ConditionalWeakTable<Item, string>();
        private static readonly Dictionary<string, Item> TypeIdCache = new Dictionary<string, Item>();

        static EntoUtilsApi()
        {
            Instance = new EntoUtilsApi();
        }
        private EntoUtilsApi()
        {
        }

        public void RegisterItemTypeHandler(string type, Func<string, Item> handler)
        {
            Data.HandlerRegistry.Add(type, handler);
        }
        public void RegisterItemTypeResolver(EUGlobals.TypeIdResolverDelegate resolver)
        {
            Data.ResolverRegistry.Add(resolver);
        }
        public Item ResolveItemTypeId(string typeId)
        {
            if (!TypeIdCache.ContainsKey(typeId))
            {
                try
                {
                    string[] split = typeId.Split(':');
                    string type = split[0];
                    string id = string.Join(":", split.Skip(1));
                    if (Data.HandlerRegistry.ContainsKey(type))
                        TypeIdCache.Add(typeId, Data.HandlerRegistry[type](id));
                    else
                        TypeIdCache.Add(typeId, null);
                }
                catch
                {
                    TypeIdCache.Add(typeId, null);
                }
            }
            return TypeIdCache[typeId]?.getOne();
        }
        public string TryResolveTypeIdFromItem(Item item, bool strict = false)
        {
            if (!ReverseCacheStrict.TryGetValue(item, out string result) && (strict || !ReverseCache.TryGetValue(item, out result)))
            {
                foreach (var resolver in Data.ResolverRegistry)
                    try
                    {
                        if (resolver(item, ref result))
                        {
                            ReverseCacheStrict.Add(item, result);
                            break;
                        }
                        if (strict)
                            result = null;
                    }
                    catch { }
                if(!strict)
                    ReverseCache.Add(item, result);
            }
            return result;
        }
        public string ResolveFirstMatchingTypeId(string id, List<string> excludedTypes = null)
        {
            return this.ResolveFirstMatchingTypeId<Item>(id, excludedTypes);
        }
        public string ResolveFirstMatchingTypeId<T>(string id, List<string> excludedTypes = null) where T : Item
        {
            excludedTypes = excludedTypes ?? new List<string>();
            foreach (var pair in Data.HandlerRegistry)
            {
                if (excludedTypes.Contains(pair.Key))
                    continue;
                try
                {
                    Item obj = pair.Value(id);
                    if (obj != null && obj is T)
                        return pair.Key + ':' + id;
                }
                catch { }
            }
            return null;
        }
    }
}
