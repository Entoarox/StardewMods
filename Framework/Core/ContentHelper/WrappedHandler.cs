using StardewModdingAPI;

using StardewValley;

namespace Entoarox.Framework.Core.ContentHelper
{
    class WrappedHandler : IAssetEditor, IAssetLoader
    {
        private IContentHandler _Handler;
        internal WrappedHandler(IContentHandler handler) => _Handler = handler;
        bool CanLoad<T>(IAssetInfo assetInfo) => _Handler.CanInject<T>(assetInfo.AssetName);
        T Load<T>(IAssetInfo assetInfo) => _Handler.Load<T>(assetInfo.AssetName, Game1.content.Load<T>);
        bool CanEdit<T>(IAssetInfo assetInfo) => _Handler.CanInject<T>(assetInfo.AssetName);
        void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.AsData<T>();
            _Handler.Inject<T>(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
