using System;
using System.Linq;
using StardewValley;

using Harmony;

namespace Entoarox.FurnitureAnywhere
{
    [HarmonyPatch(typeof(GameLocation))]
    [HarmonyPatch("isCollidingPosition")]
    [HarmonyPatch(new Type[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) })]
    static class LocationPatch
    {
        public static bool Prefix(ref bool __return, GameLocation __instance, Microsoft.Xna.Framework.Rectangle position)
        {
            foreach (AnywhereFurniture current in __instance.objects.Values.Select(a => a as AnywhereFurniture))
                if (current.furniture_type != 12 && current.getBoundingBox(current.tileLocation).Intersects(position))
                {
                    __return = true;
                    return true;
                }
            return false;
        }
    }
}
