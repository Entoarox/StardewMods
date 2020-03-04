using System;
using System.Collections.Generic;
using StardewValley;

namespace Entoarox.Utilities
{
    public interface IEntoUtilsApi
    {
        /// <summary>
        /// Registers a new type-handler, this is anything before the : in a TypeId string.
        /// </summary>
        /// <param name="type">The type-handler, preferably in the format SOURCE.TYPE</param>
        /// <param name="handler">The function to call when the type given is used as the Type section of a TypeId string, should return a Item instance if a match is found, or null if not.</param>
        void RegisterItemTypeHandler(string type, Func<string, Item> handler);
        /// <summary>
        /// Registers a new type-resolver, resolvers attempt to find a TypeId which will return the item given
        /// A resolver can return either a certain (Always works) or uncertain (Currently works, but might not always) typeId
        /// </summary>
        /// <param name="resolver">The resolver, see <see cref="EUGlobals.TypeIdResolverDelegate"/> for the signature of a resolver delegate.</param>
        void RegisterItemTypeResolver(EUGlobals.TypeIdResolverDelegate resolver);
        /// <summary>
        /// Returns the Item matching the given TypeId, or null if no item can be matched to the given TypeId.
        /// </summary>
        /// <param name="typeId">The TypeId to try and find a match for.</param>
        /// <returns>The resolved item or null.</returns>
        Item ResolveItemTypeId(string typeId);
        /// <summary>
        /// Attempts to resolve a TypeId for the given item, if strict mode is enabled, only a guaranteed match will succeed.
        /// </summary>
        /// <param name="item">The item to attempt to find a matching TypeId for.</param>
        /// <param name="strict">If only guaranteed TypeId's should be considered valid.</param>
        /// <returns>A TypeId string, or null if no match can be found.</returns>
        string TryResolveTypeIdFromItem(Item item, bool strict = false);
        /// <summary>
        /// Look through all registered TypeHandlers for a given Id, and return the full TypeId for the first one which matches a Item
        /// </summary>
        /// <param name="id">The Id section of a TypeId, meaning everything after the : symbol.</param>
        /// <param name="excludedTypes">A list of Types (meaning everything before the : symbol) which should not be matched.</param>
        /// <returns>The full TypeId string which maps to the matched item.</returns>
        string ResolveFirstMatchingTypeId(string id, List<string> excludedTypes = null);
        /// <summary>
        /// Look through all registered TypeHandlers for a given Id, and return the full TypeId for the first one which is of the T class
        /// </summary>
        /// <typeparam name="T">The class the item must match to be considered valid.</typeparam>
        /// <param name="id">The Id section of a TypeId, meaning everything after the : symbol.</param>
        /// <param name="excludedTypes">A list of Types (meaning everything before the : symbol) which should not be matched.</param>
        /// <returns>The full TypeId string which maps to the matched item.</returns>
        string ResolveFirstMatchingTypeId<T>(string id, List<string> excludedTypes = null) where T : Item;
    }
}