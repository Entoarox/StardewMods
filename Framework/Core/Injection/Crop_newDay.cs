using System;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

using Harmony;

namespace Entoarox.Framework.Core.Injection
{
    [HarmonyPatch(typeof(Crop), "newDay", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(GameLocation) })]
    class Crop_newDay
    {
        static bool Prefix(Crop __instance, int state, int fertilizer, int xTile, int yTile, GameLocation environment)
        {
            if (Core.ModEntry.Config.GreenhouseEverywhere || (environment is IAugmentedLocation && (environment as IAugmentedLocation).IsGreenhouse))
            {
                if (state == 1)
                {
                    if (!__instance.fullyGrown)
                    {
                        __instance.dayOfCurrentPhase = Math.Min(__instance.dayOfCurrentPhase + 1, (__instance.phaseDays.Count > 0) ? __instance.phaseDays[Math.Min(__instance.phaseDays.Count - 1, __instance.currentPhase)] : 0);
                    }
                    else
                    {
                        __instance.dayOfCurrentPhase--;
                    }
                    if (__instance.dayOfCurrentPhase >= ((__instance.phaseDays.Count > 0) ? __instance.phaseDays[Math.Min(__instance.phaseDays.Count - 1, __instance.currentPhase)] : 0) && __instance.currentPhase < __instance.phaseDays.Count - 1)
                    {
                        __instance.currentPhase++;
                        __instance.dayOfCurrentPhase = 0;
                    }
                    while (__instance.currentPhase < __instance.phaseDays.Count - 1 && __instance.phaseDays.Count > 0 && __instance.phaseDays[__instance.currentPhase] <= 0)
                    {
                        __instance.currentPhase++;
                    }
                    if (__instance.rowInSpriteSheet == 23 && __instance.phaseToShow == -1 && __instance.currentPhase > 0)
                    {
                        __instance.phaseToShow = Game1.random.Next(1, 7);
                    }
                }
                if ((!__instance.fullyGrown || __instance.dayOfCurrentPhase <= 0) && __instance.currentPhase >= __instance.phaseDays.Count - 1 && __instance.rowInSpriteSheet == 23)
                {
                    Vector2 vector = new Vector2(xTile, yTile);
                    environment.objects.Remove(vector);
                    string season = Game1.currentSeason;
                    switch (__instance.whichForageCrop)
                    {
                        case 495:
                            season = "spring";
                            break;
                        case 496:
                            season = "summer";
                            break;
                        case 497:
                            season = "fall";
                            break;
                        case 498:
                            season = "winter";
                            break;
                    }
                    environment.objects.Add(vector, new SObject(vector, __instance.getRandomWildCropForSeason(season), 1)
                    {
                        isSpawnedObject = true,
                        canBeGrabbed = true
                    });
                    if (environment.terrainFeatures[vector] != null && environment.terrainFeatures[vector] is HoeDirt)
                    {
                        (environment.terrainFeatures[vector] as HoeDirt).crop = null;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
