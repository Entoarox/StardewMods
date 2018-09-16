using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    static class Globals
    {
        public static string GetModName(IModLinked mod)
        {
            return EntoaroxFrameworkMod.SHelper.ModRegistry.Get(mod.ModID).Manifest.Name;
        }
    }
}
