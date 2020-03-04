using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.Internals.AssetHandlers
{
    class HelperSheetLoader : IAssetLoader
    {
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("__EMU_HELPER_TILESHEET.png") || asset.AssetNameEquals("Maps/__EMU_HELPER_TILESHEET.png") || asset.AssetNameEquals("__EMU_HELPER_TILESHEET") || asset.AssetNameEquals("Maps/__EMU_HELPER_TILESHEET");
        }
        
        public T Load<T>(IAssetInfo asset)
        {
            return (T)(object)new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, 16, 16);
        }
    }
}
