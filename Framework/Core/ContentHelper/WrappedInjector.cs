using StardewModdingAPI;

namespace Entoarox.Framework.Core.ContentHelper
{
    internal class WrappedInjector : IAssetEditor
    {
        private IContentHandler _Handler;
        internal WrappedInjector(IContentHandler handler) => _Handler = handler;
        public bool CanEdit<T>(IAssetInfo assetInfo) => _Handler.CanInject<T>(assetInfo.AssetName);
        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            _Handler.Inject<T>(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
