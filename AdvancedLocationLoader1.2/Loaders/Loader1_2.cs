using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;

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
                AdvancedLocationLoaderMod.Logger.Log(LogLevel.Error, "Unable to load manifest, json cannot be parsed: " + filepath, err);
                return;
            }
            try
            {
                Parse(Path.GetDirectoryName(filepath), conf);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Log(LogLevel.Error, "Unable to load manifest, a unexpected error occured: " + filepath, err);
            }
        }
        private static bool FileCheck(string path, string file)
        {
            string full = Path.Combine(path, file + ".xnb");
            if (!File.Exists(full))
            {
                AdvancedLocationLoaderMod.Logger.Log("File does not exist: " + full, LogLevel.Error);
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
                AdvancedLocationLoaderMod.Logger.Log("Location is already being modified: " + name, LogLevel.Error);
                return false;
            }
            AffectedLocations.Add(name);
            return true;
        }
        public static void Parse(string filepath, LocationConfig1_2 config)
        {
            // Print mod info into the log
            AdvancedLocationLoaderMod.Logger.Log((config.About.ModName == null ? "Legacy Mod" : config.About.ModName) + ", version `" + config.About.Version + "` by " + config.About.Author, LogLevel.Info);
            // Parse locations
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Locations` section...");
            if (config.Locations != null)
                foreach (Location loc in config.Locations)
                    if (LocationChecks(filepath, loc.FileName, loc.MapName))
                    {
                        if (!LocationTypes.Contains(loc.Type))
                        {
                            AdvancedLocationLoaderMod.Logger.Log("Unknown location Type, using `Default` instead: " + loc.ToString(),LogLevel.Error);
                            loc.Type = "Default";
                        }
                        loc.FileName = Path.Combine(filepath, loc.FileName);
                        Compound.Locations.Add(loc);

                    }
            // Parse overrides
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Overrides` section...",LogLevel.Trace);
            if (config.Overrides != null)
                foreach (Override ovr in config.Overrides)
                    if (LocationChecks(filepath, ovr.FileName, ovr.MapName))
                    {
                        ovr.FileName = Path.Combine(filepath, ovr.FileName);
                        Compound.Overrides.Add(ovr);
                    }
            // Parse redirects
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Redirects` section...", LogLevel.Trace);
            if (config.Redirects != null)
                foreach (Redirect red in config.Redirects)
                    if (FileCheck(Game1.content.RootDirectory, red.FromFile) && FileCheck(filepath, red.ToFile))
                    {
                        red.ToFile = Path.Combine(filepath, red.ToFile);
                        Compound.Redirects.Add(red);
                    }
            // Parse tilesheets
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Tilesheets` section...", LogLevel.Trace);
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
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `tiles` section...", LogLevel.Trace);
            if (config.Tiles != null)
                foreach (Tile til in config.Tiles)
                {
                    if (til.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(til.Conditions);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Log("Tile `" + til.ToString() + "` Condition Error: " + err,LogLevel.Error);
                            continue;
                        }
                    }
                    if (!Layers.Contains(til.LayerId))
                        AdvancedLocationLoaderMod.Logger.Log("Cannot place tile `" + til.ToString() + "` on unknown layer: " + til.LayerId,LogLevel.Error);
                    else
                        Compound.Tiles.Add(til);
                }
            // Parse properties
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Properties` section...", LogLevel.Trace);
            if (config.Properties != null)
                foreach (Property pro in config.Properties)
                {
                    if (pro.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(pro.Conditions);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Log("Property `" + pro.ToString() + "` Condition Error: " + err,LogLevel.Error);
                            continue;
                        }
                    }
                    if (!Layers.Contains(pro.LayerId))
                        AdvancedLocationLoaderMod.Logger.Log("Cannot apply property `" + pro.ToString() + "` to tile unknown layer: " + pro.LayerId,LogLevel.Error);
                    else
                        Compound.Properties.Add(pro);
                }
            // Parse warps
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Warps` section...", LogLevel.Trace);
            if (config.Warps != null)
                foreach (Warp war in config.Warps)
                {
                    if (war.Conditions != null)
                    {
                        string err = Conditions.FindConflictingConditions(war.Conditions);
                        if (err != null)
                        {
                            AdvancedLocationLoaderMod.Logger.Log("Warp `" + war.ToString() + "` Condition Error: " + err,LogLevel.Error);
                            continue;
                        }
                    }
                    Compound.Warps.Add(war);
                }
            // Parse conditionals
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Conditionals` section...", LogLevel.Trace);
            if (config.Conditionals != null)
                foreach (Conditional con in config.Conditionals)
                    if (con.Item < -1)
                        AdvancedLocationLoaderMod.Logger.Log("Unable to parse conditional, it references a null item: " + con.ToString(), LogLevel.Error);
                    else if (con.Amount < 1)
                        AdvancedLocationLoaderMod.Logger.Log("Unable to validate conditional, the item amount is less then 1: " + con.ToString(), LogLevel.Error);
                    else if (Conditionals.Contains(con.Name))
                        AdvancedLocationLoaderMod.Logger.Log("Unable to validate conditional, another condition with this name already exists: " + con.ToString(), LogLevel.Error);
                    else
                    {
                        Configs.Compound.Conditionals.Add(con);
                        Conditionals.Add(con.Name);
                    }
            // Parse minecarts
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Teleporters` section...",LogLevel.Trace);
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
                                    AdvancedLocationLoaderMod.Logger.Log("Unable to add teleporter destination for the `" + min.ListName + "` teleporter, the destination already exists: `" + dest.ToString(), LogLevel.Error);

                        }
                    if (add)
                        Compound.Teleporters.Add(min);
                }
            // Parse shops
            AdvancedLocationLoaderMod.Logger.Log("Parsing the `Shops` section...", LogLevel.Trace);
            if (config.Shops != null)
                foreach (string shop in config.Shops)
                {
                    string path = Path.Combine(filepath, shop + ".json");
                    if (!File.Exists(path))
                        AdvancedLocationLoaderMod.Logger.Log("Unable to load shop, file does not exist: " + path, LogLevel.Error);
                    else
                    {
                        ShopConfig cfg = JsonConvert.DeserializeObject<ShopConfig>(File.ReadAllText(path));
                        cfg.Portrait = Path.Combine(filepath, cfg.Portrait);
                        foreach(ShopItem item in cfg.Items)
                            if (item.Conditions != null)
                            {
                                string err = Conditions.FindConflictingConditions(item.Conditions);
                                if (err != null)
                                {
                                    AdvancedLocationLoaderMod.Logger.Log("Shop item `" + item.ToString() + "` Condition Error: " + err, LogLevel.Error);
                                    continue;
                                }
                            }
                        Configs.Compound.Shops.Add(shop, cfg);
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
            AdvancedLocationLoaderMod.Logger.Log("Applying Patches...", LogLevel.Info);
            // First we need to check any things we couldnt before
            foreach (Location obj in Compound.Locations)
                if (Game1.getLocationFromName(obj.MapName) != null)
                {
                    AdvancedLocationLoaderMod.Logger.Log("Unable to add location, it already exists: " + obj.ToString(), LogLevel.Error);
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
                        AdvancedLocationLoaderMod.Logger.Log(LogLevel.Error,"Unable to add location, the map file caused a error when loaded: " + obj.ToString(), err);
                    }
                }
            foreach (Override obj in Compound.Overrides)
                if (Game1.getLocationFromName(obj.MapName) == null)
                {
                    AdvancedLocationLoaderMod.Logger.Log("Unable to override location, it does not exist: " + obj.ToString(), LogLevel.Error);
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
                        AdvancedLocationLoaderMod.Logger.Log(LogLevel.Error,"Unable to override location, the map file caused a error when loaded: " + obj.ToString(), err);
                    }
                }
            trueCompound.Redirects = Compound.Redirects;
            foreach (Tilesheet obj in Compound.Tilesheets)
                if (Game1.getLocationFromName(obj.MapName) == null && !AffectedLocations.Contains(obj.MapName))
                {
                    AdvancedLocationLoaderMod.Logger.Log("Unable to patch tilesheet, location does not exist: " + obj.ToString(), LogLevel.Error);
                }
                else
                    trueCompound.Tilesheets.Add(obj);
            foreach (Tile obj in Compound.Tiles)
            {
                string info = CheckTileInfo(obj);
                if (info != null)
                {
                    if (info != "OPTIONAL")
                        AdvancedLocationLoaderMod.Logger.Log("Unable to apply tile patch, " + info + ":" + obj.ToString(), LogLevel.Error);
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
                        AdvancedLocationLoaderMod.Logger.Log("Unable to apply property patch, " + info + ":" + obj.ToString(), LogLevel.Error);
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
                        AdvancedLocationLoaderMod.Logger.Log("Unable to apply warp patch, " + info + ":" + obj.ToString(), LogLevel.Error);
                }
                trueCompound.Warps.Add(obj);
            }
            // At this point any edits that showed problems have been removed, so now we can actually process everything
            foreach (Location obj in trueCompound.Locations)
                Processors.ApplyLocation(obj);
            foreach (Override obj in trueCompound.Overrides)
                Processors.ApplyOverride(obj);
            foreach(Redirect obj in trueCompound.Redirects)
                EntoFramework.GetContentRegistry().RegisterXnb(obj.FromFile, obj.ToFile);
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
            AdvancedLocationLoaderMod.Logger.Log("Patches have been applied",LogLevel.Info);
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
                                AdvancedLocationLoaderMod.Logger.ExitGameImmediately("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is treated as seasonal but does not have proper seasonal formatting, this will cause bugs");
                            foreach (string season in seasons)
                                if (!File.Exists(Path.Combine(Game1.content.RootDirectory, "Maps\\" + season + "_" + path[1] + ".xnb")))
                                    AdvancedLocationLoaderMod.Logger.ExitGameImmediately("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is seasonal but is missing the tilesheet for the `" + season + "` season");
                        }
            }
        }
    }
}
