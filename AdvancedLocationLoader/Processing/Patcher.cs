using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entoarox.AdvancedLocationLoader.Configs;
using Entoarox.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using xTile;
using xTile.Tiles;
using Tile = Entoarox.AdvancedLocationLoader.Configs.Tile;
using Warp = Entoarox.AdvancedLocationLoader.Configs.Warp;

namespace Entoarox.AdvancedLocationLoader.Processing
{
    /// <summary>Applies content pack data.</summary>
    internal class Patcher
    {
        /*********
        ** Fields
        *********/
        /// <summary>The content helper to use when interacting with the game's content files.</summary>
        private readonly IContentHelper CoreContentHelper;

        /// <summary>The content pack data to apply.</summary>
        private readonly ContentPackData[] Data;

        /// <summary>Writes messages to the log.</summary>
        private readonly IMonitor Monitor;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Writes messages to the log.</param>
        /// <param name="contentHelper">The content helper to use when interacting with the game's content files.</param>
        /// <param name="data">The content pack data to apply.</param>
        public Patcher(IMonitor monitor, IContentHelper contentHelper, IEnumerable<ContentPackData> data)
        {
            this.Monitor = monitor;
            this.Data = data.ToArray();
            this.CoreContentHelper = contentHelper;
        }

        /// <summary>Apply content data to the game.</summary>
        /// <param name="compoundData">Compound data loaded from the content packs.</param>
        public void ApplyPatches(out Compound compoundData)
        {
            // track info
            Dictionary<IContentPack, Tilesheet[]> seasonalTilesheetsByContentPack = new Dictionary<IContentPack, Tilesheet[]>();
            HashSet<string> customLocationNames = new HashSet<string>();
            Dictionary<string, Point> mapSizes = new Dictionary<string, Point>();
            Dictionary<string, Map> mapCache = new Dictionary<string, Map>();
            Dictionary<string, List<string>> tilesheetCache = new Dictionary<string, List<string>>();
            List<Tile> dynamicTiles = new List<Tile>();
            List<Property> dynamicProperties = new List<Property>();
            List<Warp> dynamicWarps = new List<Warp>();
            List<Conditional> conditionals = new List<Conditional>();
            List<TeleporterList> teleporters = new List<TeleporterList>();
            List<ShopConfig> shops = new List<ShopConfig>();

            // apply content packs
            foreach (ContentPackData pack in this.Data)
            {
                this.Monitor.Log($"Applying {pack.ContentPack.Manifest.Name}...", LogLevel.Debug);
                string stage = "entry";
                try
                {
                    // collect tilesheet info
                    IDictionary<string, Tilesheet[]> tilesheetQueue = pack.Tilesheets
                        .GroupBy(p => p.MapName)
                        .ToDictionary(p => p.Key, p => p.ToArray(), StringComparer.InvariantCultureIgnoreCase);
                    IList<Tilesheet> seasonalTilesheets = new List<Tilesheet>();

                    // apply custom locations
                    stage = "locations";
                    foreach (Location location in pack.Locations)
                    {
                        if (Game1.getLocationFromName(location.MapName) != null)
                        {
                            this.Monitor.Log($"  Can't add location {location.MapName}, it already exists.", LogLevel.Warn);
                            continue;
                        }

                        try
                        {
                            // cache info
                            Map map = pack.ContentPack.LoadAsset<Map>(location.FileName);
                            mapSizes.Add(location.MapName, new Point(map.DisplayWidth, map.DisplayHeight));
                            mapCache.Add(location.MapName, map);
                            customLocationNames.Add(location.MapName);

                            // preload tilesheets
                            if (tilesheetQueue.TryGetValue(location.MapName, out Tilesheet[] tilesheets))
                            {
                                tilesheetQueue.Remove(location.MapName);
                                foreach (Tilesheet tilesheet in tilesheets)
                                {
                                    Processors.ApplyTilesheet(this.CoreContentHelper, pack.ContentPack, tilesheet, map);
                                    if (tilesheet.Seasonal)
                                        seasonalTilesheets.Add(tilesheet);
                                }
                            }

                            // load location
                            Processors.ApplyLocation(pack.ContentPack, location);
                        }
                        catch (Exception err)
                        {
                            this.Monitor.Log($"   Can't add location {location.MapName}, an error occurred.", LogLevel.Error,
                                err);
                        }
                    }

                    // apply remaining tilesheets
                    stage = "tilesheets";
                    foreach (Tilesheet obj in tilesheetQueue.Values.SelectMany(p => p))
                    {
                        GameLocation location = Game1.getLocationFromName(obj.MapName);
                        if (location == null)// && !customLocationNames.Contains(obj.MapName))
                        {
                            this.Monitor.Log($"   Can't apply tilesheet '{obj.SheetId}', location '{obj.MapName}' doesn't exist.", LogLevel.Error);
                            continue;
                        }

                        Processors.ApplyTilesheet(this.CoreContentHelper, pack.ContentPack, obj, location.map);
                        if (obj.Seasonal)
                            seasonalTilesheets.Add(obj);
                    }
                    if (seasonalTilesheets.Any())
                        seasonalTilesheetsByContentPack[pack.ContentPack] = seasonalTilesheets.ToArray();

                    // apply overrides
                    stage = "overrides";
                    foreach (Override obj in pack.Overrides)
                    {
                        if (Game1.getLocationFromName(obj.MapName) == null)
                        {
                            this.Monitor.Log($"   Can't override location {obj.MapName}, it doesn't exist.",
                                LogLevel.Error);
                            continue;
                        }

                        try
                        {
                            // load map
                            Map map = pack.ContentPack.LoadAsset<Map>(obj.FileName);
                            mapSizes.Add(obj.MapName, new Point(map.DisplayWidth, map.DisplayHeight));

                            // fix custom tilesheet paths
                            foreach (TileSheet tilesheet in map.TileSheets)
                            {
                                string fakePath = Processors.GetFakePath(tilesheet);
                                if (fakePath != null)
                                    tilesheet.ImageSource = fakePath;
                            }

                            // apply location override
                            Processors.ApplyOverride(pack.ContentPack, obj);
                        }
                        catch (Exception err)
                        {
                            this.Monitor.Log($"   Can't override location {obj.MapName}, an error occurred.", LogLevel.Error, err);
                        }
                    }

                    // apply redirects
                    stage = "redirects";
                    {
                        HashSet<string> redirCache = new HashSet<string>();
                        foreach (Redirect obj in pack.Redirects)
                        {
                            if (!redirCache.Contains(obj.ToFile))
                            {
                                string toAssetPath = pack.ContentPack.GetRelativePath(ModEntry.SHelper.DirectoryPath, obj.ToFile);
                                this.CoreContentHelper.RegisterXnbReplacement(obj.FromFile, toAssetPath);
                                redirCache.Add(obj.ToFile);
                            }
                        }
                    }

                    // apply tiles
                    stage = "tiles";
                    foreach (Tile obj in pack.Tiles)
                    {
                        if (!this.PreprocessTile(obj, customLocationNames, mapSizes, out string error))
                        {
                            if (error != null)
                                this.Monitor.Log($"   Can't apply tile {obj}: {error}", LogLevel.Error);
                        }

                        else if (obj.SheetId != null && (!tilesheetCache.ContainsKey(obj.MapName) || !tilesheetCache[obj.MapName].Contains(obj.SheetId)))
                        {
                            Map map = mapCache.ContainsKey(obj.MapName)
                                ? mapCache[obj.MapName]
                                : Game1.getLocationFromName(obj.MapName).map;

                            if (map.GetTileSheet(obj.SheetId) == null && (!tilesheetCache.ContainsKey(map.Id) || !tilesheetCache[map.Id].Contains(obj.SheetId)))
                            {
                                this.Monitor.Log($"   Can't apply tile {obj}, tilesheet doesn't exist.", LogLevel.Error);
                                continue;
                            }
                        }

                        Processors.ApplyTile(obj);
                        if (!string.IsNullOrEmpty(obj.Conditions))
                            dynamicTiles.Add(obj);
                    }

                    // apply properties
                    stage = "properties";
                    foreach (Property obj in pack.Properties)
                    {
                        if (!this.PreprocessTile(obj, customLocationNames, mapSizes, out string error))
                        {
                            if (error != null)
                                this.Monitor.Log($"   Can't apply property patch {obj}: {error}.", LogLevel.Error);
                            continue;
                        }

                        Processors.ApplyProperty(obj);
                        if (!string.IsNullOrEmpty(obj.Conditions))
                            dynamicProperties.Add(obj);
                    }

                    // apply warps
                    stage = "warps";
                    foreach (Warp obj in pack.Warps)
                    {
                        if (!this.PreprocessTile(obj, customLocationNames, mapSizes, out string error))
                        {
                            if (error != null)
                                this.Monitor.Log($"   Can't apply warp {obj}: {error}.", LogLevel.Error);
                            continue;
                        }

                        Processors.ApplyWarp(obj);
                        if (!string.IsNullOrEmpty(obj.Conditions))
                            dynamicWarps.Add(obj);
                    }

                    // save conditionals
                    stage = "conditionals";
                    foreach (Conditional obj in pack.Conditionals)
                        conditionals.Add(obj);

                    // save teleporters
                    stage = "teleporters";
                    foreach (TeleporterList obj in pack.Teleporters)
                        teleporters.Add(obj);

                    // save shops
                    stage = "shops";
                    foreach (ShopConfig obj in pack.Shops)
                        shops.Add(obj);
                }
                catch (Exception ex)
                {
                    this.Monitor.ExitGameImmediately($"Failed applying changes from the {pack.ContentPack.Manifest.Name} content pack ({stage} step).", ex);
                }
            }

            // postprocess
            try
            {
                NPC.populateRoutesFromLocationToLocationList();
                this.VerifyGameIntegrity();
                this.Monitor.Log("Patches applied!", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                this.Monitor.ExitGameImmediately("Failed post-processing after content pack changes.", ex);
            }

            // create compound info
            compoundData = new Compound(seasonalTilesheetsByContentPack, dynamicTiles, dynamicProperties, dynamicWarps, conditionals, teleporters, shops);
        }


        /*********
        ** Protected methods
        *********/
        private bool PreprocessTile(TileInfo info, HashSet<string> customLocationNames, IDictionary<string, Point> mapSizes, out string error)
        {
            error = null;

            if (Game1.getLocationFromName(info.MapName) == null && !customLocationNames.Contains(info.MapName))
            {
                if (!info.Optional)
                    error = "location does not exist";
                return false;
            }

            Point size;
            if (!customLocationNames.Contains(info.MapName))
            {
                Map map = Game1.getLocationFromName(info.MapName).map;
                size = new Point(map.DisplayWidth, map.DisplayHeight);
                customLocationNames.Add(info.MapName);
                mapSizes[info.MapName] = size;
            }
            else
                size = mapSizes[info.MapName];

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
            {
                error = "placement is outside location bounds";
                return false;
            }

            return true;
        }

        private void VerifyGameIntegrity()
        {
            string[] seasons = { "spring", "summer", "fall", "winter" };
            foreach (GameLocation loc in Game1.locations)
            {
                if (!loc.IsOutdoors || loc.Name.Equals("Desert"))
                    continue;

                foreach (TileSheet sheet in loc.map.TileSheets)
                {
                    string fileName = Path.GetFileName(sheet.ImageSource);
                    if (fileName.StartsWith("spring_") || fileName.StartsWith("summer_") || fileName.StartsWith("fall_") || fileName.StartsWith("winter_"))
                    {
                        string path = Path.GetDirectoryName(sheet.ImageSource);
                        if (string.IsNullOrWhiteSpace(path))
                            path = "Maps";

                        foreach (string season in seasons)
                        {
                            try
                            {
                                Game1.content.Load<Texture2D>(Path.Combine(path, $"{season}_{fileName.Split(new[] { '_' }, 2)[1]}"));
                            }
                            catch
                            {
                                this.Monitor.ExitGameImmediately($"The `{sheet.Id}` (`{sheet.ImageSource}`) tileSheet in the `{loc.Name}` location is seasonal, but ALL can't find the tilesheet for the `{season}` season. This will cause bugs!");
                            }
                        }
                    }
                }
            }
        }
    }
}
