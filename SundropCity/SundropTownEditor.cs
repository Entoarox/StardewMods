using System.Collections.Generic;
using System.IO;

using xTile;

using StardewModdingAPI;

namespace SundropCity
{
    class SundropTownEditor : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/Town");
        }

        public void Edit<T>(IAssetData asset)
        {
            SundropCityMod.SMonitor.Log("Patching town...", LogLevel.Trace);
            // Cross-mod compatibility
            var registry = SundropCityMod.SHelper.Data.ReadJsonFile<Dictionary<string, string>>("assets/TownPatches/registry.json");
            string patchVersion = "Vanilla";
            foreach (var pair in registry)
            {
                if (!SundropCityMod.SHelper.ModRegistry.IsLoaded(pair.Key))
                    continue;
                patchVersion = pair.Value;
                break;
            }
            var map = asset.GetData<Map>();
            var patch = File.OpenRead(Path.Combine(SundropCityMod.SHelper.DirectoryPath, "assets/TownPatches/" + patchVersion + ".tdiff"));
            Entoarox.TDiffPatcher.V1R0P0.TDiffPatcher.ApplyPatch(map, patch, _ => SundropCityMod.SMonitor.Log(_, LogLevel.Trace));
        }
    }
}
