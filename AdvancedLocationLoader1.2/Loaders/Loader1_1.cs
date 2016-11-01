using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using StardewModdingAPI;

using Entoarox.AdvancedLocationLoader.Configs;

namespace Entoarox.AdvancedLocationLoader.Loaders
{
    static class Loader1_1
    {
        public static void Load(string filepath)
        {
            AdvancedLocationLoaderMod.Logger.Trace("Converting legacy 1.1 manifest to new 1.2 format...");
            LocationConfig1_1 Config = new LocationConfig1_1().InitializeConfig(filepath);
            LocationConfig1_2 Updated = new LocationConfig1_2();
            // Prepare the 1.2 properties
            Updated.LoaderVersion = "1.2.0";
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `about` section");
            // Convert the `about` section
            Updated.About = new About();
            if (Config.about.ContainsKey("author"))
                Updated.About.Author = Config.about["author"];
            if (Config.about.ContainsKey("description"))
                Updated.About.Description = Config.about["description"];
            if (Config.about.ContainsKey("version"))
                Updated.About.Version = Config.about["version"];
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `locations` section");
            // Convert the `locations` section
            if (Config.locations != null)
            {
                Updated.Locations = new List<Location>();
                foreach (Dictionary<string, string> loc in Config.locations)
                {
                    Location newLoc = new Location();
                    newLoc.Farmable = loc.ContainsKey("farmable") ? Convert.ToBoolean(loc["farmable"]) : false;
                    newLoc.Outdoor = loc.ContainsKey("outdoor") ? Convert.ToBoolean(loc["outdoor"]) : false;
                    newLoc.FileName = loc["file"];
                    newLoc.MapName = loc["name"];
                    newLoc.Type = "Default";
                    Updated.Locations.Add(newLoc);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `overrides` section");
            // Convert the `overrides` section
            if (Config.overrides != null)
            {
                Updated.Overrides = new List<Override>();
                foreach (Dictionary<string, string> ovr in Config.overrides)
                {
                    Override newOvr = new Override();
                    newOvr.FileName = ovr["file"];
                    newOvr.MapName = ovr["name"];
                    Updated.Overrides.Add(newOvr);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `tilesheets` section");
            // Convert the `tilesheets` section
            if (Config.tilesheets != null)
            {
                Updated.Tilesheets = new List<Tilesheet>();
                foreach (Dictionary<string, string> sheet in Config.tilesheets)
                {
                    Tilesheet newSheet = new Tilesheet();
                    newSheet.FileName = sheet["file"];
                    newSheet.MapName = sheet["map"];
                    newSheet.SheetId = sheet["sheet"];
                    newSheet.Seasonal = sheet.ContainsKey("seasonal") ? Convert.ToBoolean(sheet["seasonal"]) : false;
                    Updated.Tilesheets.Add(newSheet);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `tiles` section");
            // Convert the `tiles` section
            if (Config.tiles != null)
            {
                Updated.Tiles = new List<Tile>();
                foreach (Dictionary<string, string> tile in Config.tiles)
                {
                    Tile newTile = new Tile();
                    newTile.TileX = Convert.ToInt32(tile["x"]);
                    newTile.TileY = Convert.ToInt32(tile["y"]);
                    newTile.MapName = tile["map"];
                    newTile.LayerId = tile["layer"];
                    newTile.SheetId = tile.ContainsKey("sheet") ? tile["sheet"] : null;
                    if (tile.ContainsKey("interval"))
                    {
                        newTile.Interval = Convert.ToInt32(tile["interval"]);
                        newTile.TileIndexes = new List<string>(tile["tileIndex"].Split(',')).ConvertAll(Convert.ToInt32).ToArray();
                    }
                    else
                        newTile.TileIndex = Convert.ToInt32(tile["tileIndex"]);
                    newTile.Conditions = tile.ContainsKey("conditions") ? tile["conditions"] : null;
                    Updated.Tiles.Add(newTile);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `properties` section");
            // Convert the `properties` section
            if (Config.properties != null)
            {
                Updated.Properties = new List<Property>();
                foreach (List<string> prop in Config.properties)
                {
                    Property newProp = new Property();
                    newProp.MapName = prop[0];
                    newProp.LayerId = prop[1];
                    newProp.TileX = Convert.ToInt32(prop[2]);
                    newProp.TileY = Convert.ToInt32(prop[3]);
                    newProp.Key = prop[4];
                    newProp.Value = prop[5];
                    Updated.Properties.Add(newProp);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `warps` section");
            // Convert the `warps` section
            if (Config.warps != null)
            {
                Updated.Warps = new List<Warp>();
                foreach (List<string> warp in Config.warps)
                {
                    Warp newWarp = new Warp();
                    newWarp.MapName = warp[0];
                    newWarp.TileX = Convert.ToInt32(warp[1]);
                    newWarp.TileY = Convert.ToInt32(warp[2]);
                    newWarp.TargetName = warp[3];
                    newWarp.TargetX = Convert.ToInt32(warp[4]);
                    newWarp.TargetY = Convert.ToInt32(warp[5]);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `conditions` section");
            // Convert the `conditions` into the new `Conditionals` section
            if (Config.conditions != null)
            {
                Updated.Conditionals = new List<Conditional>();
                foreach (KeyValuePair<string, Dictionary<string, string>> cond in Config.conditions)
                {
                    Conditional newCond = new Conditional();
                    newCond.Name = cond.Key;
                    newCond.Item = Convert.ToInt32(cond.Value["item"]);
                    newCond.Amount = Convert.ToInt32(cond.Value["amount"]);
                    newCond.Question = cond.Value["question"];
                    Updated.Conditionals.Add(newCond);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `minecarts` section");
            // Convert the `minecarts` section
            if (Config.minecarts != null)
            {
                Updated.Teleporters = new List<TeleporterList>();
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> set in Config.minecarts)
                {
                    TeleporterList newSet = new TeleporterList();
                    newSet.ListName = set.Key;
                    foreach (KeyValuePair<string, List<string>> dest in set.Value)
                    {
                        TeleporterDestination newDest = new TeleporterDestination();
                        newDest.ItemText = dest.Key;
                        newDest.MapName = dest.Value[0];
                        newDest.TileX = Convert.ToInt32(dest.Value[1]);
                        newDest.TileY = Convert.ToInt32(dest.Value[2]);
                        newDest.Direction = Convert.ToInt32(dest.Value[3]);
                        newSet.Destinations.Add(newDest);
                    }
                    Updated.Teleporters.Add(newSet);
                }
            }
            AdvancedLocationLoaderMod.Logger.Trace("Converting the `shops` section");
            // Convert the `shops` section
            Updated.Shops = Config.shops;
            AdvancedLocationLoaderMod.Logger.Trace("Saving converted manifest to file...");
            // Save and then parse the updated config
            File.WriteAllText(filepath, JsonConvert.SerializeObject(Updated,new JsonSerializerSettings() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore }));
            AdvancedLocationLoaderMod.Logger.Trace("Loading the converted manifest");
            Loader1_2.Load(filepath);
        }
    }
}
