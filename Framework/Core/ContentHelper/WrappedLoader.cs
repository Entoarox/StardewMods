using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework.Core.ContentHelper
{
    internal class WrappedLoader : IAssetLoader
    {
        private IContentHandler _Handler;
        internal WrappedLoader(IContentHandler handler) => _Handler = handler;
        public bool CanLoad<T>(IAssetInfo assetInfo) => _Handler.CanInject<T>(assetInfo.AssetName);
        public T Load<T>(IAssetInfo assetInfo) => _Handler.Load<T>(assetInfo.AssetName, Game1.content.Load<T>);
    }
}
