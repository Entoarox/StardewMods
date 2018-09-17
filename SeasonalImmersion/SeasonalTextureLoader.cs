using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Entoarox.SeasonalImmersion
{
    internal class SeasonalTextureLoader : IAssetLoader
    {
        /*********
        ** Public methods
        *********/
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return typeof(Texture2D).IsAssignableFrom(typeof(T)) && asset.AssetName.Contains("Craftables") && (this.Valid(asset, "Craftables") || this.Valid(asset, "Craftables_indoor") || this.Valid(asset, "Craftables_outdoor"));
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("TileSheets\\Craftables_indoor"))
                return (T)(object)ModEntry.SeasonTextures["Craftables_indoor"][Game1.currentSeason];
            if (asset.AssetNameEquals("TileSheets\\Craftables_outdoor"))
                return (T)(object)ModEntry.SeasonTextures["Craftables_outdoor"][Game1.currentSeason];
            if (asset.AssetNameEquals("TileSheets\\Craftables"))
                return (T)(object)ModEntry.SeasonTextures["Craftables"][Game1.currentSeason];
            return default(T);
        }


        /*********
        ** Protected methods
        *********/
        private bool Valid(IAssetInfo asset, string name)
        {
            return ModEntry.SeasonTextures.ContainsKey(name) && asset.AssetNameEquals(Path.Combine("TileSheets", name));
        }
    }
}
