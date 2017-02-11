using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile;
using xTile.Layers;
using xTile.Tiles;
using xTile.ObjectModel;

namespace Entoarox.Framework.ContentManager
{
    static class MapMergeHelper
    {
        private static void Log(string message)
        {
            EntoFramework.Logger.Log(message, StardewModdingAPI.LogLevel.Warn);
        }
        public static void MergeMaps(string assetName, Map @base, List<Map> patches)
        {
            // define vars
            Map edits = new Map();
            List<string> sets = new List<string>();
            // Insert base warps into warp merger
            if (@base.Properties.ContainsKey("Warp"))
                sets.Add(@base.Properties["Warp"]);
            // First pass
            foreach (Map patch in patches)
            {
                MergeTilesheets(assetName, @base, edits, patch);
                if (patch.Properties.ContainsKey("Warp"))
                    sets.Add(patch.Properties["Warp"]);
            }
            // Second pass
            foreach (Map patch in patches)
            {
                MergeTilesheetProperties(assetName, @base, edits, patch);
            }
            // Merge all warps together in a way that removes duplicate statements
            @base.Properties["Warp"] = ResolveWarps(sets);
        }
        private static string ResolveWarps(List<string> sets)
        {
            List<string> Warps = new List<string>();
            foreach (string set in sets)
            {
                string[] split = set.Split(' ');
                string[] warp = new string[5];
                for (var c = 0; c < split.Length; c++)
                {
                    warp[c % 5] = split[c];
                    if (c % 5 != 5)
                        continue;
                    string final = string.Join(" ", warp);
                    if (!Warps.Contains(final))
                        Warps.Add(final);
                    warp = new string[5];
                }
            }
            return string.Join(" ", Warps);
        }
        private static void MergeTilesheets(string assetName, Map @base, Map edits, Map patch)
        {
            foreach(TileSheet sheet in patch.TileSheets)
            {
                TileSheet baseSheet = @base.GetTileSheet(sheet.Id);
                TileSheet editsSheet = edits.GetTileSheet(sheet.Id);
                if (editsSheet != null)
                {
                    if (editsSheet.ImageSource == sheet.ImageSource)
                        continue;
                    else
                        Log($"ContentManager/MapContentInjector: the `{assetName}` asset has duplicate tilesheet Id `{sheet.Id}`, using first.");
                }
                else if (baseSheet != null)
                {
                    if (baseSheet.ImageSource != sheet.ImageSource)
                        edits.AddTileSheet(new TileSheet(sheet.Id, edits, sheet.ImageSource, sheet.SheetSize, sheet.TileSize));
                }
            }
        }
        private static void MergeTilesheetProperties(string assetName, Map @base, Map edits, Map patch)
        {
            foreach(TileSheet sheet in patch.TileSheets)
            {
                TileSheet editsSheet = edits.GetTileSheet(sheet.Id);
                TileSheet baseSheet = @base.GetTileSheet(sheet.Id);
                if (editsSheet == null)
                {
                    editsSheet = new TileSheet(sheet.Id, edits, sheet.ImageSource, sheet.SheetSize, sheet.TileSize);
                    edits.AddTileSheet(editsSheet);
                }
                foreach(KeyValuePair<string,PropertyValue> prop in sheet.Properties)
                {
                    if (editsSheet.Properties.ContainsKey(prop.Key))
                    {
                        if (editsSheet.Properties[prop.Key].Equals(prop.Value))
                            continue;
                        else
                            Log($"ContentManager/MapContentInjector: the `{assetName}` asset has duplicate tilesheet property `{prop.Key}` in tilesheet `{sheet.Id}`, using first.");
                    }
                    else if(baseSheet == null || !baseSheet.Properties.ContainsKey(prop.Key) || !baseSheet.Properties[prop.Key].Equals(prop.Value))
                        editsSheet.Properties.Add(prop);
                }
            }
        }
    }
}
