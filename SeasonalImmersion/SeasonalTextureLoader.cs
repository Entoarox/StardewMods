using System.IO;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Entoarox.SeasonalImmersion
{
    class SeasonalTextureLoader : IAssetLoader
    {
        private bool Valid(IAssetInfo asset, string name)
        {
            return SeasonalImmersionMod.SeasonTextures.ContainsKey(name) && asset.AssetNameEquals(Path.Combine("TileSheets", name));
        }
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return typeof(Texture2D).IsAssignableFrom(typeof(T)) && asset.AssetName.Contains("Craftables") && (Valid(asset, "Craftables") || Valid(asset, "Craftables_indoor") || Valid(asset, "Craftables_outdoor"));
        }

        public T Load<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("TileSheets\\Craftables_indoor"))
                return (T)(object)SeasonalImmersionMod.SeasonTextures["Craftables_indoor"][Game1.currentSeason];
            if (asset.AssetNameEquals("TileSheets\\Craftables_outdoor"))
                return (T)(object)SeasonalImmersionMod.SeasonTextures["Craftables_outdoor"][Game1.currentSeason];
            if (asset.AssetNameEquals("TileSheets\\Craftables"))
                return (T)(object)SeasonalImmersionMod.SeasonTextures["Craftables"][Game1.currentSeason];
            return default(T);
        }
    }
}
