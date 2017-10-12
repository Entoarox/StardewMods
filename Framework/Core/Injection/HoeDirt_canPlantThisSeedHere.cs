using System;

using StardewValley;
using StardewValley.TerrainFeatures;

using Harmony;

namespace Entoarox.Framework.Core.Injection
{
    [HarmonyPatch(typeof(HoeDirt), "canPlantThisSeedHere", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool)})]
    class HoeDirt_canPlantThisSeedHere
    {
        static bool Prefix(HoeDirt __instance, bool __return, int objectIndex, int tileX, int tileY, bool isFertilizer)
        {
            return (isFertilizer || __instance.crop != null || !(Core.ModEntry.Config.GreenhouseEverywhere || (Game1.currentLocation is IAugmentedLocation && (Game1.currentLocation as IAugmentedLocation).IsGreenhouse)));
        }
        static void Postfix(HoeDirt __instance, bool __return, int objectIndex, int tileX, int tileY, bool isFertilizer)
        {
            Crop crop = new Crop(objectIndex, tileX, tileY);
            if (crop.seasonsToGrowIn.Count == 0)
                __return = false;
            else
                __return =  !crop.raisedSeeds || !Utility.doesRectangleIntersectTile(Game1.player.GetBoundingBox(), tileX, tileY);
        }
    }
}
