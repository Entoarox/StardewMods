using System;
using System.Collections.Generic;

namespace StardewModdingAPI.Content.Plugins
{
    class XnbLoader : IContentHandler
    {
        internal static Dictionary<string, string> AssetMap = new Dictionary<string, string>();

        public bool IsLoader { get; } = true;
        public bool CanLoad<T>(string assetName)
        {
            return AssetMap.ContainsKey(assetName);
        }
        public T Load<T>(string assetName, Func<string,T> loadBase)
        {
            return ExtendibleContentManager.ModContent.Load<T>(AssetMap[assetName]);
        }

        public bool IsInjector { get; } = false;
        public bool CanInject<T>(string assetName)
        {
            throw new NotImplementedException();
        }
        public void Inject<T>(string assetName, ref T asset)
        {
            throw new NotImplementedException();
        }
    }
}
