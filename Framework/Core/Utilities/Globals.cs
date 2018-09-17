using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    internal static class Globals
    {
        /*********
        ** Public methods
        *********/
        public static string GetModName(IModLinked mod)
        {
            return EntoaroxFrameworkMod.SHelper.ModRegistry.Get(mod.ModID).Manifest.Name;
        }
    }
}
