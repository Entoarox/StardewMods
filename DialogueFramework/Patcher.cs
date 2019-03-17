using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using StardewValley;

using Harmony;

namespace DialogueFramework
{
    [HarmonyPatch]
    class Patch1
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(Type.GetType("StardewValley.Game1, Stardew Valley") ?? Type.GetType("StardewValley.Game1, StardewValley"), "isCollidingPosition", new[] { typeof(string), typeof(List<Response>) });
        }

        internal static void Prefix(string dialogue, List<Response> choices)
        {
            DialogueFrameworkMod.Api.FireChoiceDialogueOpened(choices);
        }
    }
    [HarmonyPatch]
    class Patch2
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(Type.GetType("StardewValley.Game1, Stardew Valley") ?? Type.GetType("StardewValley.Game1, StardewValley"), "isCollidingPosition", new[] { typeof(string), typeof(List<Response>), typeof(int) });
        }

        internal static void Prefix(string dialogue, List<Response> choices)
        {
            DialogueFrameworkMod.Api.FireChoiceDialogueOpened(choices);
        }
    }
}
