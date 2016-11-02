using System;
using System.IO;
using System.Collections.Generic;

using StardewValley;

using Microsoft.Xna.Framework;

using Entoarox.AdvancedLocationLoader.Configs;
using Warp = Entoarox.AdvancedLocationLoader.Configs.Warp;
using Entoarox.Framework;

using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Loaders
{
    static class Loader1_2
    {
        private static List<string> LocationTypes = new List<string> { "Default", "Cellar", "Greenhouse", "Sewer", "BathHousePool", "Desert", "Decoratable" };
        private static List<string> Layers = new List<string>() { "Back", "Buildings", "Paths", "Front", "AlwaysFront" };
        private static List<string> AffectedLocations = new List<string>();
        private static List<string> Conditionals = new List<string>();
        private static LocationConfig1_2 Compound = new LocationConfig1_2();
        private static Dictionary<string, Point> MapSizes = new Dictionary<string, Point>();
        public static void Load(string filepath)
        {
            LocationConfig1_2 conf;
            try
            {
                conf = JsonConvert.DeserializeObject<LocationConfig1_2>(File.ReadAllText(filepath));
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to load manifest, json cannot be parsed: " + filepath, err);
                return;
            }
            try
            {
                Parse(Path.GetDirectoryName(filepath), conf);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to load manifest, a unexpected error occured: " + filepath, err);
            }
        }
        private static bool FileCheck(string path, string file)
        {
            string full = Path.Combine(path, file + ".xnb");
            if (!File.Exists(full))
            {
                AdvancedLocationLoaderMod.Logger.Error("File does not exist: " + full);
                return false;
            }
            return true;
        }
        private static bool LocationChecks(string path, string file, string name)
        {
            if (!FileCheck(path, file))
                return false;
            if (AffectedLocations.Contains(name))
            {
                AdvancedLocationLoaderMod.Logger.Error("Location is already being modified: " + name);
                return false;
            }
            AffectedLocations.Add(name);
            return true;
        }
        public static void Parse(string filepath, LocationConfig1_2 config)
        {
            // Print mod info into the log
            AdvancedLocationLoaderMod.Logger.Log("INFO", (config.About.ModName == null ? "Legacy Mod" : config.About.ModName) + ", version `" + config.About.Version + "` by " + config.About.Author, ConsoleColor.Magenta);
            // Parse locations
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Locations` section...");
            if (config.Locations != null)
                foreach (Location loc in config.Locations)
                    if (LocationChecks(filepath, loc.FileName, loc.MapName))
                    {
                        if (!LocationTypes.Contains(loc.Type))
                        {
                            AdvancedLocationLoaderMod.Logger.Error("Unknown location Type, using `Default` instead: " + loc.ToString());
                            loc.Type = "Default";
                        }
                        loc.FileName = Path.Combine(filepath, loc.FileName);
                        Compound.Locations.Add(loc);

                    }
            // Parse overrides
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Overrides` section...");
            if (config.Overrides != null)
                foreach (Override ovr in config.Overrides)
                    if (LocationChecks(filepath, ovr.FileName, ovr.MapName))
                    {
                        ovr.FileName = Path.Combine(filepath, ovr.FileName);
                        Compound.Overrides.Add(ovr);
                    }
            // Parse redirects
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Redirects` section...");
            if (config.Redirects != null)
                foreach (Redirect red in config.Redirects)
                    if (FileCheck(Game1.content.RootDirectory, red.FromFile) && FileCheck(filepath, red.ToFile))
                        EntoFramework.GetContentRegistry().RegisterXnb(red.FromFile, Path.Combine(filepath, red.ToFile));
            // Parse tilesheets
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Tilesheets` section...");
            if (config.Tilesheets != null)
                foreach (Tilesheet sht in config.Tilesheets)
                {
                    if (sht.Seasonal)
                    {
                        if (!FileCheck(filepath, sht.FileName + "_spring"))
                            continue;
                        if (!FileCheck(filepath, sht.FileName + "_summer"))
                            continue;
                        if (!FileCheck(filepath, sht.FileName + "_fall"))
                            continue;
                        if (!FileCheck(filepath, sht.FileName + "_winter"))
                            continue;
                    }
                    else if (!FileCheck(filepath, sht.FileName))
                        continue;
                    sht.FileName = Path.Combine(filepath, sht.FileName);
                    Compound.Tilesheets.Add(sht);
                }
            // Parse tiles
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `tiles` section...");
            if (config.Tiles != null)
                foreach (Tile til in config.Tiles)
                {
                    if (til.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(til.Conditions.Split(','), 5, true);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Error("Tile `" + til.ToString() + "` Condition Error: " + err);
                            continue;
                        }
                    }
                    if (!Layers.Contains(til.LayerId))
                        AdvancedLocationLoaderMod.Logger.Error("Cannot place tile `" + til.ToString() + "` on unknown layer: " + til.LayerId);
                    else
                        Compound.Tiles.Add(til);
                }
            // Parse properties
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Properties` section...");
            if (config.Properties != null)
                foreach (Property pro in config.Properties)
                {
                    if (pro.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(pro.Conditions.Split(','), 5, true);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Error("Property `" + pro.ToString() + "` Condition Error: " + err);
                            continue;
                        }
                    }
                    if (!Layers.Contains(pro.LayerId))
                        AdvancedLocationLoaderMod.Logger.Error("Cannot apply property `" + pro.ToString() + "` to tile unknown layer: " + pro.LayerId);
                    else
                        Compound.Properties.Add(pro);
                }
            // Parse warps
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Warps` section...");
            if (config.Warps != null)
                foreach (Warp war in config.Warps)
                {
                    if (war.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(war.Conditions.Split(','), 5, true);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Error("Warp `" + war.ToString() + "` Condition Error: " + err);
                            continue;
                        }
                    }
                    Compound.Warps.Add(war);
                }
            // Parse conditionals
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Conditionals` section...");
            if (config.Conditionals != null)
                foreach (Conditional con in config.Conditionals)
                    if (con.Item < -1)
                        AdvancedLocationLoaderMod.Logger.Error("Unable to parse conditional, it references a null item: " + con.ToString());
                    else if (con.Amount < 1)
                        AdvancedLocationLoaderMod.Logger.Error("Unable to validate conditional, the item amount is less then 1: " + con.ToString());
                    else if (Conditionals.Contains(con.Name))
                        AdvancedLocationLoaderMod.Logger.Error("Unable to validate conditional, another condition with this name already exists: " + con.ToString());
                    else
                    {
                        Configs.Compound.Conditionals.Add(con);
                        Conditionals.Add(con.Name);
                    }
            // Parse minecarts
            AdvancedLocationLoaderMod.Logger.Trace("Parsing the `Teleporters` section...");
            if (config.Teleporters != null)
                foreach (TeleporterList min in config.Teleporters)
                {
                    bool add = true;
                    foreach (TeleporterList tes in Compound.Teleporters)
                        if (tes.ListName == min.ListName)
                        {
                            add = false;
                            foreach (TeleporterDestination dest in min.Destinations)
                                if (tes.Destinations.TrueForAll(a => { return !a.Equals(dest); }))
                                    tes.Destinations.Add(dest);
                                else
                                    AdvancedLocationLoaderMod.Logger.Error("Unable to add teleporter destination for the `" + min.ListName + "` teleporter, the destination already exists: `" + dest.ToString());

                        }
                    if (add)
                        Compound.Teleporters.Add(min);
                    // Parse shops
                    if (config.Shops != null)
                        foreach (string shop in config.Shops)
                        {
                            string path = Path.Combine(filepath, shop + ".json");
                            if (!File.Exists(path))
                                AdvancedLocationLoaderMod.Logger.Error("Unable to load shop, file does not exist: " + path);
                            else
                            {
                                ShopConfig cfg = JsonConvert.DeserializeObject<ShopConfig>(File.ReadAllText(path));
                                cfg.Portrait = Path.Combine(filepath, cfg.Portrait);
                                Configs.Compound.Shops.Add(shop, cfg);
                            }
                        }
                }
        }
        internal static string CheckTileInfo(TileInfo info)
        {
            if (Game1.getLocationFromName(info.MapName) == null && !AffectedLocations.Contains(info.MapName))
                if (info.Optional != true)
                    return "location does not exist";
                else
                    return "OPTIONAL";
            Point size;
            if (!AffectedLocations.Contains(info.MapName))
            {
                xTile.Map map = Game1.getLocationFromName(info.MapName).map;
                size = new Point(map.DisplayWidth, map.DisplayHeight);
            }
            else
                size = MapSizes[info.MapName];
            int minX = 0;
            int minY = 0;
            int maxX = size.X;
            int maxY = size.Y;
            if (info is Warp)
            {
                minX--;
                minY--;
                maxX++;
                maxY++;
            }
            if (info.TileX < minX || info.TileY < minY || info.TileX > maxX || info.TileY > maxY)
                return "placement is outside location bounds";
            return null;
        }
        public static void ApplyPatches()
        {
            LocationConfig1_2 trueCompound = new LocationConfig1_2();
            AdvancedLocationLoaderMod.Logger.Info("Applying Patches...");
            // First we need to check any things we couldnt before
            foreach (Location obj in Compound.Locations)
                if (Game1.getLocationFromName(obj.MapName) != null)
                {
                    AdvancedLocationLoaderMod.Logger.Error("Unable to add location, it already exists: " + obj.ToString());
                }
                else
                {
                    try
                    {
                        xTile.Map map = new LocalizedContentManager(Game1.content.ServiceProvider, Path.GetDirectoryName(obj.FileName)).Load<xTile.Map>(Path.GetFileName(obj.FileName));
                        MapSizes.Add(obj.MapName, new Point(map.DisplayWidth, map.DisplayHeight));
                        trueCompound.Locations.Add(obj);
                    }
                    catch (Exception err)
                    {
                        AdvancedLocationLoaderMod.Logger.Error("Unable to add location, the map file caused a error when loaded: " + obj.ToString(), err);
                    }
                }
            foreach (Override obj in Compound.Overrides)
                if (Game1.getLocationFromName(obj.MapName) == null)
                {
                    AdvancedLocationLoaderMod.Logger.Error("Unable to override location, it does not exist: " + obj.ToString());
                }
                else
                {
                    try
                    {
                        xTile.Map map = new LocalizedContentManager(Game1.content.ServiceProvider, Path.GetDirectoryName(obj.FileName)).Load<xTile.Map>(Path.GetFileName(obj.FileName));
                        MapSizes.Add(obj.MapName, new Point(map.DisplayWidth, map.DisplayHeight));
                        trueCompound.Overrides.Add(obj);
                    }
                    catch (Exception err)
                    {
                        AdvancedLocationLoaderMod.Logger.Error("Unable to override location, the map file caused a error when loaded: " + obj.ToString(), err);
                    }
                }
            foreach (Tilesheet obj in Compound.Tilesheets)
                if (Game1.getLocationFromName(obj.MapName) == null && !AffectedLocations.Contains(obj.MapName))
                {
                    AdvancedLocationLoaderMod.Logger.Error("Unable to patch tilesheet, location does not exist: " + obj.ToString());
                }
                else
                    trueCompound.Tilesheets.Add(obj);
            foreach (Tile obj in Compound.Tiles)
            {
                string info = CheckTileInfo(obj);
                if (info != null)
                {
                    if (info != "OPTIONAL")
                        AdvancedLocationLoaderMod.Logger.Error("Unable to apply tile patch, " + info + ":" + obj.ToString());
                }
                else
                    trueCompound.Tiles.Add(obj);
            }
            foreach (Property obj in Compound.Properties)
            {
                string info = CheckTileInfo(obj);
                if (info != null)
                {
                    if (info != "OPTIONAL")
                        AdvancedLocationLoaderMod.Logger.Error("Unable to apply property patch, " + info + ":" + obj.ToString());
                }
                else
                    trueCompound.Properties.Add(obj);
            }
            foreach (Warp obj in Compound.Warps)
            {
                string info = CheckTileInfo(obj);
                if (info != null)
                {
                    if (info != "OPTIONAL")
                        AdvancedLocationLoaderMod.Logger.Error("Unable to apply warp patch, " + info + ":" + obj.ToString());
                }
                trueCompound.Warps.Add(obj);
            }
            // At this point any edits that showed problems have been removed, so now we can actually process everything
            foreach (Location obj in trueCompound.Locations)
                Processors.ApplyLocation(obj);
            foreach (Override obj in trueCompound.Overrides)
                Processors.ApplyOverride(obj);
            foreach (Tilesheet obj in trueCompound.Tilesheets)
            {
                Processors.ApplyTilesheet(obj);
                if (obj.Seasonal)
                    Configs.Compound.SeasonalTilesheets.Add(obj);
            }
            foreach (Tile obj in trueCompound.Tiles)
            {
                Processors.ApplyTile(obj);
                if (!string.IsNullOrEmpty(obj.Conditions))
                    Configs.Compound.DynamicTiles.Add(obj);
            }
            foreach (Property obj in trueCompound.Properties)
            {
                Processors.ApplyProperty(obj);
                if (!string.IsNullOrEmpty(obj.Conditions))
                    Configs.Compound.DynamicProperties.Add(obj);
            }
            foreach (Warp obj in trueCompound.Warps)
            {
                Processors.ApplyWarp(obj);
                if (!string.IsNullOrEmpty(obj.Conditions))
                    Configs.Compound.DynamicWarps.Add(obj);
            }
            foreach (Conditional obj in trueCompound.Conditionals)
                Configs.Compound.Conditionals.Add(obj);
            foreach (TeleporterList obj in trueCompound.Teleporters)
                Configs.Compound.Teleporters.Add(obj);
            NPC.populateRoutesFromLocationToLocationList();
            Compound = null;
            Conditionals = null;
            AffectedLocations = null;
            MapSizes = null;
            VerifyPatchIntegrity();
            AdvancedLocationLoaderMod.Logger.Info("Patches have been applied");
        }
        internal static void VerifyPatchIntegrity()
        {
            string[] seasons = new string[] { "spring", "summer", "fall", "winter" };
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc.isOutdoors && !loc.Name.Equals("Desert"))
                    foreach (xTile.Tiles.TileSheet sheet in loc.map.TileSheets)
                        if (!sheet.ImageSource.Contains("path") && !sheet.ImageSource.Contains("object"))
                        {
                            string[] path = sheet.ImageSource.Split('_');
                            if (path.Length != 2)
                                AdvancedLocationLoaderMod.Logger.Error("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is treated as seasonal but does not have proper seasonal formatting, this will cause bugs");
                            foreach (string season in seasons)
                                if (!File.Exists(Path.Combine(Game1.content.RootDirectory, "Maps\\" + season + "_" + path[1] + ".xnb")))
                                    AdvancedLocationLoaderMod.Logger.Error("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is seasonal but is missing the tilesheet for the `" + season + "` season");
                        }
            }
        }
    }
}
