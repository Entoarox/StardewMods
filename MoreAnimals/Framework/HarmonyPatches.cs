using System;
using System.Reflection;

using StardewValley;
using StardewValley.Characters;

using Harmony;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    [HarmonyPatch]
    class PetHarmony
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(Type.GetType("StardewValley.Characters.Pet, Stardew Valley") ?? Type.GetType("StardewValley.Characters.Pet, StardewValley"), "setAtFarmPosition");
        }

        internal static void Postfix(Pet __instance)
        {
            if (!Game1.isRaining)
                __instance.position.X-=new Random(__instance.Age).Next(8)*32f;
        }
    }
}
