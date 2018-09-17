using System;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DeferredAssetInfo
    {
        /*********
        ** Accessors
        *********/
        public Type Type { get; set; }
        public Delegate Handler { get; set; }


        /*********
        ** Public methods
        *********/
        public DeferredAssetInfo(Type type, Delegate handler)
        {
            this.Type = type;
            this.Handler = handler;
        }
    }
}
