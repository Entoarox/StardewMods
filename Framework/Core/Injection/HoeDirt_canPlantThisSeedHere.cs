using Harmony;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Entoarox.Framework.Core.Injection
{
    [HarmonyPatch(typeof(HoeDirt), "canPlantThisSeedHere", new[] { typeof(int), typeof(int), typeof(int), typeof(bool) })]
    internal class HoeDirt_canPlantThisSeedHere
    {
        /*********
        ** Protected methods
        *********/
        private static bool Prefix(HoeDirt __instance, bool __return, int objectIndex, int tileX, int tileY, bool isFertilizer)
        {
            return
                isFertilizer
                || __instance.crop != null
                || !(EntoaroxFrameworkMod.Config.GreenhouseEverywhere || Game1.currentLocation is IAugmentedLocation && (Game1.currentLocation as IAugmentedLocation).IsGreenhouse);
        }

        private static void Postfix(HoeDirt __instance, bool __return, int objectIndex, int tileX, int tileY, bool isFertilizer)
        {
            Crop crop = new Crop(objectIndex, tileX, tileY);
            __return =
                crop.seasonsToGrowIn.Count != 0
                && (!crop.raisedSeeds.Value || !Utility.doesRectangleIntersectTile(Game1.player.GetBoundingBox(), tileX, tileY));
        }
    }
}
