using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    internal class DictionaryAssetInfo<TKey, TValue>
    {
        /*********
        ** Accessors
        *********/
        public IContentHelper ContentHelper { get; }
        public IDictionary<TKey, TValue> Data { get; }


        /*********
        ** Public methods
        *********/
        public DictionaryAssetInfo(IContentHelper contentHelper, IDictionary<TKey, TValue> data)
        {
            this.ContentHelper = contentHelper;
            this.Data = data;
        }
    }
}
