using System;
using System.IO;
using System.Collections.Generic;

using StardewModdingAPI;

using StardewValley;

using Microsoft.Xna.Framework;

using Entoarox.Framework;

using Entoarox.AdvancedLocationLoader.Configs;
using Warp = Entoarox.AdvancedLocationLoader.Configs.Warp;

using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Loaders
{
    static class Loader1_2
    {
        private static List<string> LocationTypes = new List<string> { "Default", "Cellar", "Greenhouse", "Sewer", "BathHousePool", "Desert", "Decoratable" };
        private static List<string> Layers = new List<string>() { "Back", "Buildings", "Paths", "Front", "AlwaysFront" };
        private static List<string> AffectedLocations = new List<string>();
        private static List<string> Conditionals = new List<string>();
        private static SecondaryLocationManifest1_2 Compound = new SecondaryLocationManifest1_2();
        private static Dictionary<string, Point> MapSizes = new Dictionary<string, Point>();
        private static Dictionary<string, xTile.Map> MapCache = new Dictionary<string, xTile.Map>();
        private static Dictionary<string, List<string>> TilesheetCache = new Dictionary<string, List<string>>();
        public static void Load(string filepath)
        {
            MainLocationManifest1_2 conf;
            try
            {
                conf = JsonConvert.DeserializeObject<MainLocationManifest1_2>(File.ReadAllText(filepath));
                if (conf.Includes.Count > 0)
                    foreach (string include in conf.Includes)
                    {
                        string childPath = Path.Combine(Path.GetDirectoryName(filepath), include + ".json");
                        SecondaryLocationManifest1_2 secondary;
                        try
                        {
                            secondary = JsonConvert.DeserializeObject<SecondaryLocationManifest1_2>(File.ReadAllText(childPath));
                        }
                        catch(Exception err)
                        {
                            ModEntry.Logger.Log("Unable to merge child manifest, json cannot be parsed: " + childPath,LogLevel.Error,  err);
                            continue;
                        }
                        string point="Unknown";
                        try
                        {
                            point = "Conditionals";
                            conf.Conditionals.AddRange(secondary.Conditionals);
                            point = "Locations";
                            conf.Locations.AddRange(secondary.Locations);
                            point = "Overrides";
                            conf.Overrides.AddRange(secondary.Overrides);
                            point = "Properties";
                            conf.Properties.AddRange(secondary.Properties);
                            point = "Redirects";
                            conf.Redirects.AddRange(secondary.Redirects);
                            point = "Shops";
                            conf.Shops.AddRange(secondary.Shops);
                            point = "Teleporters";
                            conf.Teleporters.AddRange(secondary.Teleporters);
                            point = "Tiles";
                            conf.Tiles.AddRange(secondary.Tiles);
                            point = "Tilesheets";
                            conf.Tilesheets.AddRange(secondary.Tilesheets);
                            point = "Warps";
                            conf.Warps.AddRange(secondary.Warps);
                        }
                        catch(Exception err)
                        {
                            ModEntry.Logger.Log("Unable to merge child manifest, a unexpected error occured trying to merge the `"+point+"` section: " + childPath,LogLevel.Error,  err);
                        }
                    }
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("Unable to load manifest, json cannot be parsed: " + filepath,LogLevel.Error,  err);
                return;
            }
            try
            {
                Parse(Path.GetDirectoryName(filepath), conf);
            }
            catch (Exception err)
            {
                ModEntry.Logger.Log("Unable to load manifest, a unexpected error occured: " + filepath,LogLevel.Error,  err);
            }
        }
        private static bool FileCheck(string path, string file)
        {
            string full = Path.Combine(path, file + ".xnb");
            if (!File.Exists(full))
            {
                ModEntry.Logger.Log("File does not exist: " + full, LogLevel.Error);
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
                ModEntry.Logger.Log("Location is already being modified: " + name, LogLevel.Error);
                return false;
            }
            AffectedLocations.Add(name);
            return true;
        }
        private static string GetRelativePath(string filepath)
        {
            Uri fromUri = new Uri(ModEntry.SHelper.DirectoryPath + Path.DirectorySeparatorChar);
            Uri toUri = new Uri(filepath);
            if (fromUri.Scheme != toUri.Scheme) { throw new InvalidOperationException("Unable to make path relative to the DLL: " + filepath); }
            return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
        }
        public static void Parse(string filepath, MainLocationManifest1_2 config)
        {
            string stage = "Entry";
            try
            {
                string relpath = GetRelativePath(filepath);
                // Print mod info into the log
                ModEntry.Logger.Log((config.About.ModName == null ? "Legacy Mod" : config.About.ModName) + ", version `" + config.About.Version + "` by " + config.About.Author, LogLevel.Info);
                // Parse locations
                stage = "Locations";
                if (config.Locations != null)
                    foreach (Location loc in config.Locations)
                        if (LocationChecks(filepath, loc.FileName, loc.MapName))
                        {
                            if (!LocationTypes.Contains(loc.Type))
                            {
                                ModEntry.Logger.Log("Unknown location Type, using `Default` instead: " + loc.ToString(), LogLevel.Error);
                                loc.Type = "Default";
                            }
                            Compound.Locations.Add(loc);
                            loc.FileName = Path.Combine(relpath, loc.FileName);
                        }
                // Parse overrides
                stage = "Overrides";
                if (config.Overrides != null)
                    foreach (Override ovr in config.Overrides)
                        if (LocationChecks(filepath, ovr.FileName, ovr.MapName))
                        {
                            ovr.FileName = Path.Combine(relpath, ovr.FileName);
                            Compound.Overrides.Add(ovr);
                        }
                // Parse redirects
                stage = "Redirects";
                if (config.Redirects != null)
                    foreach (Redirect red in config.Redirects)
                        if (FileCheck(Game1.content.RootDirectory, red.FromFile) && FileCheck(filepath, red.ToFile))
                        {
                            red.ToFile = Path.Combine(relpath, red.ToFile);
                            Compound.Redirects.Add(red);
                        }
                // Parse tilesheets
                stage = "Tilesheets";
                if (config.Tilesheets != null)
                    foreach (Tilesheet sht in config.Tilesheets)
                    {
                        if (sht.FileName != null)
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
                        }
                        sht.FileName = Path.Combine(filepath, sht.FileName);
                        if (!TilesheetCache.ContainsKey(sht.MapName))
                            TilesheetCache.Add(sht.MapName, new List<string>());
                        TilesheetCache[sht.MapName].Add(sht.SheetId);
                        Compound.Tilesheets.Add(sht);
                    }
                // Parse tiles
                stage = "Tiles";
                if (config.Tiles != null)
                    foreach (Tile til in config.Tiles)
                    {
                        if (!Layers.Contains(til.LayerId))
                            ModEntry.Logger.Log("Cannot place tile `" + til.ToString() + "` on unknown layer: " + til.LayerId, LogLevel.Error);
                        else
                            Compound.Tiles.Add(til);
                    }
                // Parse properties
                stage = "Properties";
                if (config.Properties != null)
                    foreach (Property pro in config.Properties)
                    {
                        if (!Layers.Contains(pro.LayerId))
                            ModEntry.Logger.Log("Cannot apply property `" + pro.ToString() + "` to tile unknown layer: " + pro.LayerId, LogLevel.Error);
                        else
                            Compound.Properties.Add(pro);
                    }
                // Parse warps
                stage = "Warps";
                if (config.Warps != null)
                    foreach (Warp war in config.Warps)
                        Compound.Warps.Add(war);
                // Parse conditionals
                stage = "Conditionals";
                if (config.Conditionals != null)
                    foreach (Conditional con in config.Conditionals)
                        if (con.Item < -1)
                            ModEntry.Logger.Log("Unable to parse conditional, it references a null item: " + con.ToString(), LogLevel.Error);
                        else if (con.Amount < 1)
                            ModEntry.Logger.Log("Unable to validate conditional, the item amount is less then 1: " + con.ToString(), LogLevel.Error);
                        else if (Conditionals.Contains(con.Name))
                            ModEntry.Logger.Log("Unable to validate conditional, another condition with this name already exists: " + con.ToString(), LogLevel.Error);
                        else
                        {
                            Configs.Compound.Conditionals.Add(con);
                            Conditionals.Add(con.Name);
                        }
                // Parse minecarts
                stage = "Teleporters";
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
                                        ModEntry.Logger.Log("Unable to add teleporter destination for the `" + min.ListName + "` teleporter, the destination already exists: `" + dest.ToString(), LogLevel.Error);
                                ModEntry.Logger.Log("Teleporter updated: " + tes.ToString(), LogLevel.Trace);
                                break;
                            }
                        if (add)
                        {
                            Compound.Teleporters.Add(min);
                            ModEntry.Logger.Log("Teleporter created: " + min.ToString(), LogLevel.Trace);
                        }
                    }
                // Parse shops
                stage = "Shops";
                if (config.Shops != null)
                    foreach (string shop in config.Shops)
                    {
                        string path = Path.Combine(filepath, shop + ".json");
                        if (!File.Exists(path))
                            ModEntry.Logger.Log("Unable to load shop, file does not exist: " + path, LogLevel.Error);
                        else
                        {
                            try
                            {
                                ShopConfig cfg = JsonConvert.DeserializeObject<ShopConfig>(File.ReadAllText(path));
                                cfg.Portrait = Path.Combine(relpath, cfg.Portrait);
                                Configs.Compound.Shops.Add(shop, cfg);
                            }
                            catch (Exception err)
                            {
                                ModEntry.Logger.Log("Could not load shop due to unexpected error: " + path, LogLevel.Error, err);
                                continue;
                            }
                        }
                    }
            }
            catch(Exception err)
            {
                ModEntry.Logger.Log("Unable to load manifest, a unexpected error occured at stage `"+stage+"`: " + filepath, LogLevel.Error, err);
            }
        }
        internal static string CheckTileInfo(TileInfo info)
        {
            if (Game1.getLocationFromName(info.MapName) == null && !AffectedLocations.Contains(info.MapName))
                return info.Optional ? "OPTIONAL" : "location does not exist";
            Point size;
            if (!AffectedLocations.Contains(info.MapName))
            {
                xTile.Map map = Game1.getLocationFromName(info.MapName).map;
                size = new Point(map.DisplayWidth, map.DisplayHeight);
                AffectedLocations.Add(info.MapName);
                MapSizes.Add(info.MapName, size);
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
            int stage = 0;
            try
            {
                stage++; // 1
                SecondaryLocationManifest1_2 trueCompound = new SecondaryLocationManifest1_2();
                ModEntry.Logger.Log("Applying Patches...", LogLevel.Trace);
                // First we need to check any things we couldnt before
                foreach (Location obj in Compound.Locations)
                    if (Game1.getLocationFromName(obj.MapName) != null)
                    {
                        ModEntry.Logger.Log("Unable to add location, it already exists: " + obj.ToString(), LogLevel.Error);
                    }
                    else
                    {
                        try
                        {
                            xTile.Map map = ModEntry.SHelper.Content.Load<xTile.Map>(obj.FileName);
                            MapSizes.Add(obj.MapName, new Point(map.DisplayWidth, map.DisplayHeight));
                            MapCache.Add(obj.MapName, map);
                            AffectedLocations.Add(obj.MapName);
                            trueCompound.Locations.Add(obj);
                        }
                        catch (Exception err)
                        {
                            ModEntry.Logger.Log("Unable to add location, the map file caused a error when loaded: " + obj.ToString(), LogLevel.Error, err);
                        }
                    }
                stage++; // 2
                foreach (Override obj in Compound.Overrides)
                    if (Game1.getLocationFromName(obj.MapName) == null)
                    {
                        ModEntry.Logger.Log("Unable to override location, it does not exist: " + obj.ToString(), LogLevel.Error);
                    }
                    else
                    {
                        try
                        {
                            xTile.Map map = ModEntry.SHelper.Content.Load<xTile.Map>(obj.FileName);
                            MapSizes.Add(obj.MapName, new Point(map.DisplayWidth, map.DisplayHeight));
                            trueCompound.Overrides.Add(obj);
                        }
                        catch (Exception err)
                        {
                            ModEntry.Logger.Log("Unable to override location, the map file caused a error when loaded: " + obj.ToString(), LogLevel.Error, err);
                        }
                    }
                stage++; // 3
                trueCompound.Redirects = Compound.Redirects;
                stage++; // 4
                foreach (Tilesheet obj in Compound.Tilesheets)
                    if (Game1.getLocationFromName(obj.MapName) == null && !AffectedLocations.Contains(obj.MapName))
                    {
                        ModEntry.Logger.Log("Unable to patch tilesheet, location does not exist: " + obj.ToString(), LogLevel.Error);
                    }
                    else
                        trueCompound.Tilesheets.Add(obj);
                stage++; // 5
                foreach (Tile obj in Compound.Tiles)
                {
                    string info = CheckTileInfo(obj);
                    if (info != null)
                    {
                        if (info != "OPTIONAL")
                        {
                            ModEntry.Logger.Log("Unable to apply tile patch, " + info + ":" + obj.ToString(), LogLevel.Error);
                            continue;
                        }
                    }
                    else if (obj.SheetId != null && (!TilesheetCache.ContainsKey(obj.MapName) || !TilesheetCache[obj.MapName].Contains(obj.SheetId)))
                    {
                        xTile.Map map = MapCache.ContainsKey(obj.MapName) ? MapCache[obj.MapName] : Game1.getLocationFromName(obj.MapName).map;
                        if (map.GetTileSheet(obj.SheetId) == null && (!TilesheetCache.ContainsKey(map.Id) || !TilesheetCache[map.Id].Contains(obj.SheetId)))
                        {
                            ModEntry.Logger.Log("Unable to apply tile patch, tilesheet does not exist:" + obj.ToString(), LogLevel.Error);
                            continue;
                        }
                    }
                    trueCompound.Tiles.Add(obj);
                }
                stage++; // 6
                foreach (Property obj in Compound.Properties)
                {
                    string info = CheckTileInfo(obj);
                    if (info != null)
                    {
                        if (info != "OPTIONAL")
                            ModEntry.Logger.Log("Unable to apply property patch, " + info + ":" + obj.ToString(), LogLevel.Error);
                    }
                    else
                        trueCompound.Properties.Add(obj);
                }
                stage++; // 7
                foreach (Warp obj in Compound.Warps)
                {
                    string info = CheckTileInfo(obj);
                    if (info != null)
                    {
                        if (info != "OPTIONAL")
                            ModEntry.Logger.Log("Unable to apply warp patch, " + info + ":" + obj.ToString(), LogLevel.Error);
                    }
                    trueCompound.Warps.Add(obj);
                }
                stage++; // 8
                foreach (Conditional obj in Compound.Conditionals)
                    Configs.Compound.Conditionals.Add(obj);
                stage++; // 9
                foreach (TeleporterList obj in Compound.Teleporters)
                    Configs.Compound.Teleporters.Add(obj);
                stage++; // 10
                // At this point any edits that showed problems have been removed, so now we can actually process everything
                foreach (Location obj in trueCompound.Locations)
                    Processors.ApplyLocation(obj);
                stage++; // 11
                foreach (Override obj in trueCompound.Overrides)
                    Processors.ApplyOverride(obj);
                stage++; // 12
                List<string> redirCache = new List<string>();
                foreach (Redirect obj in trueCompound.Redirects)
                    if (!redirCache.Contains(obj.ToFile))
                    {
                        ModEntry.SHelper.Content.RegisterXnbReplacement(obj.FromFile, obj.ToFile);
                        redirCache.Add(obj.ToFile);
                    }
                redirCache = null;
                stage++; // 13
                foreach (Tilesheet obj in trueCompound.Tilesheets)
                {
                    Processors.ApplyTilesheet(obj);
                    if (obj.Seasonal)
                        Configs.Compound.SeasonalTilesheets.Add(obj);
                }
                stage++; // 14
                foreach (Tile obj in trueCompound.Tiles)
                {
                    Processors.ApplyTile(obj);
                    if (!string.IsNullOrEmpty(obj.Conditions))
                        Configs.Compound.DynamicTiles.Add(obj);
                }
                stage++; // 15
                foreach (Property obj in trueCompound.Properties)
                {
                    Processors.ApplyProperty(obj);
                    if (!string.IsNullOrEmpty(obj.Conditions))
                        Configs.Compound.DynamicProperties.Add(obj);
                }
                stage++; // 16
                foreach (Warp obj in trueCompound.Warps)
                {
                    Processors.ApplyWarp(obj);
                    if (!string.IsNullOrEmpty(obj.Conditions))
                        Configs.Compound.DynamicWarps.Add(obj);
                }
                stage++; // 17
                NPC.populateRoutesFromLocationToLocationList();
                stage++; // 18
                Compound = null;
                Conditionals = null;
                AffectedLocations = null;
                MapSizes = null;
                stage++; // 19
                VerifyGameIntegrity();
                stage++; // 20
                ModEntry.Logger.Log("Patches have been applied", LogLevel.Debug);
            }
            catch(Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Unable to patch the game, a unexpected error occured at stage "+stage.ToString(), err);
            }
        }
        internal static void VerifyGameIntegrity()
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
                                ModEntry.Logger.ExitGameImmediately("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is treated as seasonal but does not have proper seasonal formatting, this will cause bugs!");
                            foreach (string season in seasons)
                            {
                                try
                                {
                                    Game1.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(Path.Combine("Maps", season + "_" + path[1]));
                                }
                                catch
                                {
                                    ModEntry.Logger.ExitGameImmediately("The `" + sheet.Id + "` TileSheet in the `" + loc.Name + "` location is seasonal but ALL cant find the tilesheet for the `" + season + "` season, this will cause bugs!");
                                }
                            }
                        }
            }
        }
    }
}
