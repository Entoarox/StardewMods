using StardewModdingAPI;

namespace SundropCity.Internal
{
    class SundropTreeLoader : IAssetLoader
    {
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TerrainFeatures/tree456_spring") || asset.AssetNameEquals("TerrainFeatures/tree456_summer") || asset.AssetNameEquals("TerrainFeatures/tree456_fall") || asset.AssetNameEquals("TerrainFeatures/tree456_winter");
        }

        public T Load<T>(IAssetInfo asset)
        {
            return SundropCityMod.SHelper.Content.Load<T>("assets/TerrainFeatures/TreePalm.png");
        }
    }
}
