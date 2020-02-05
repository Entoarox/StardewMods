using System;
using System.Collections.Generic;
using StardewValley;

namespace Entoarox.Utilities
{
    public interface IEntoUtilsApi
    {
        void RegisterItemTypeHandler(string type, Func<string, Item> handler);
        void RegisterItemTypeResolver(EUGlobals.TypeIdResolverDelegate resolver);
        Item ResolveItemTypeId(string typeId);
        string TryResolveTypeIdFromItem(Item item, bool strict = false);
        string ResolveFirstMatchingTypeId(string id, List<string> excludedTypes = null);
        string ResolveFirstMatchingTypeId<T>(string id, List<string> excludedTypes = null) where T : Item;
    }
}