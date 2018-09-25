using System;
using System.Collections.Generic;
using System.IO;
using Entoarox.AdvancedLocationLoader.Configs;
using Entoarox.AdvancedLocationLoader.Locations;
using Entoarox.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile;
using xTile.Dimensions;
using xTile.Tiles;
using Tile = Entoarox.AdvancedLocationLoader.Configs.Tile;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Processors
    {
        /*********
        ** Fields
        *********/
        private static readonly IDictionary<string, string> MappingCache = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);


        /*********
        ** Public methods
        *********/
        public static string GetFakePath(TileSheet tilesheet)
        {
            return Processors.MappingCache.TryGetValue(tilesheet.ImageSource, out string fakePath)
                ? fakePath
                : null;
        }

        public static void ApplyTile(Tile tile)
        {
            int stage = 0;
            int branch = 0;
            try
            {
                stage++; // 1
                if (!string.IsNullOrEmpty(tile.Conditions) && !ModEntry.SHelper.Conditions().ValidateConditions(tile.Conditions))
                    return;
                stage++; // 2
                GameLocation loc = Game1.getLocationFromName(tile.MapName);
                if (loc == null)
                    ModEntry.Logger.Log("Unable to set required tile, location does not exist: " + tile, LogLevel.Error);
                stage++; // 3
                if (tile.TileIndex != null)
                {
                    branch = 1;
                    if (tile.TileIndex < 0)
                    {
                        if (!loc.TryRemoveTile(tile.TileX, tile.TileY, tile.LayerId) && !tile.Optional)
                        {
                            ModEntry.Logger.Log("Unable to remove required tile, tile does not exist: " + tile, LogLevel.Error);
                            return;
                        }
                    }
                    else
                        try
                        {
                            loc.SetTile(tile.TileX, tile.TileY, tile.LayerId, (int)tile.TileIndex, tile.SheetId);
                        }
                        catch (Exception err)
                        {
                            if (!tile.Optional)
                                ModEntry.Logger.Log("Unable to set required tile: " + tile, LogLevel.Error, err);
                            return;
                        }
                }
                else
                {
                    branch = 2;
                    try
                    {
                        loc.SetTile(tile.TileX, tile.TileY, tile.LayerId, tile.TileIndexes, (int)tile.Interval, tile.SheetId);
                    }
                    catch (Exception err)
                    {
                        if (!tile.Optional)
                            ModEntry.Logger.Log("Unable to set required tile: " + tile, LogLevel.Error, err);
                        return;
                    }
                }

                stage++; // 4
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately($"Unable to patch tile, a unexpected error occured at stage {stage}{(branch > 0 ? ("-" + branch) : "")}: {tile}", err);
            }
        }

        public static void ApplyProperty(Property property)
        {
            try
            {
                if (string.IsNullOrEmpty(property.Conditions) || ModEntry.SHelper.Conditions().ValidateConditions(property.Conditions))
                {
                    if (Game1.getLocationFromName(property.MapName).HasTile(property.TileX, property.TileY, property.LayerId))
                        Game1.getLocationFromName(property.MapName).SetTileProperty(property.TileX, property.TileY, property.LayerId, property.Key, property.Value);
                    else if (!property.Optional)
                        ModEntry.Logger.Log("Unable to patch required property, tile does not exist: " + property, LogLevel.Error);
                }
            }
            catch (Exception err)
            {
                if (!property.Optional)
                    ModEntry.Logger.ExitGameImmediately("Unable to patch property, a unexpected error occured: " + property, err);
            }
        }

        public static void ApplyWarp(Configs.Warp config)
        {
            try
            {
                if (!string.IsNullOrEmpty(config.Conditions) && !ModEntry.SHelper.Conditions().ValidateConditions(config.Conditions))
                    return;
                StardewValley.Warp warp = new StardewValley.Warp(config.TileX, config.TileY, config.TargetName, config.TargetX, config.TargetY, false);
                GameLocation loc = Game1.getLocationFromName(config.MapName);
                loc.warps.Filter(a => a.X != warp.X || a.Y != warp.Y);
                loc.warps.Add(warp);
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Unable to patch warp, a unexpected error occured: " + config, err);
            }
        }

        public static void ApplyTilesheet(IContentHelper coreContentHelper, IContentPack contentPack, Tilesheet tilesheet, Map map)
        {
            int stage = 0;
            int branch = 0;
            int skip = 0;
            try
            {
                stage++; // 1
                if (tilesheet.FileName == null)
                    skip = 1;
                else
                {
                    skip = 2;
                    string fakepath = Path.Combine("AdvancedLocationLoader/FakePath_paths_objects", tilesheet.FileName);
                    if (tilesheet.Seasonal)
                        fakepath = fakepath.Replace("all_sheet_paths_objects", Path.Combine("all_sheet_paths_objects", Game1.currentSeason));
                    stage++; // 2
                    if (!Processors.MappingCache.ContainsKey(tilesheet.FileName))
                    {
                        string toAssetPath = contentPack.GetRelativePath(
                            fromAbsolutePath: ModEntry.SHelper.DirectoryPath,
                            toLocalPath: tilesheet.Seasonal ? ($"{tilesheet.FileName}_{Game1.currentSeason}") : tilesheet.FileName
                        );
                        coreContentHelper.RegisterXnbReplacement(fakepath, toAssetPath);
                        Processors.MappingCache[tilesheet.FileName] = fakepath;
                    }

                    stage++; // 3
                    if (map.GetTileSheet(tilesheet.SheetId) != null)
                    {
                        branch = 1;
                        map.GetTileSheet(tilesheet.SheetId).ImageSource = fakepath;
                    }
                    else
                    {
                        branch = 2;
                        Texture2D sheet = Game1.content.Load<Texture2D>(fakepath);
                        map.AddTileSheet(new TileSheet(tilesheet.SheetId, map, fakepath, new Size((int)Math.Ceiling(sheet.Width / 16.0), (int)Math.Ceiling(sheet.Height / 16.0)), new Size(16, 16)));
                    }
                }

                stage++; // 4 (skip 2)
                if (tilesheet.Properties.Count > 0)
                {
                    TileSheet sheet = map.GetTileSheet(tilesheet.SheetId);
                    foreach (string prop in tilesheet.Properties.Keys)
                        sheet.Properties[prop] = tilesheet.Properties[prop];
                }
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately($"Unable to patch tilesheet, a unexpected error occured at stage {stage}{(skip > 0 ? ("-" + skip) : "")}{(branch > 0 ? ("-" + branch) : "")}: {tilesheet}", err);
            }
        }

        public static void ApplyLocation(IContentPack contentPack, Configs.Location location)
        {
            try
            {
                GameLocation loc;
                string mapPath = contentPack.GetActualAssetKey(location.FileName);
                switch (location.Type)
                {
                    case "Cellar":
                        loc = new Cellar(mapPath, location.MapName);
                        break;
                    case "BathHousePool":
                        loc = new BathHousePool(mapPath, location.MapName);
                        break;
                    case "Decoratable":
                        loc = new Locations.DecoratableLocation(mapPath, location.MapName);
                        break;
                    case "Desert":
                        loc = new Locations.Desert(mapPath, location.MapName);
                        break;
                    case "Greenhouse":
                        loc = new Greenhouse(mapPath, location.MapName);
                        break;
                    case "Sewer":
                        loc = new Locations.Sewer(mapPath, location.MapName);
                        break;
                    default:
                        loc = new GameLocation(mapPath, location.MapName);
                        break;
                }

                loc.IsOutdoors = location.Outdoor;
                loc.IsFarm = location.Farmable;
                Game1.locations.Add(loc);
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Unable to add custom location, a unexpected error occured: " + location, err);
            }
        }

        public static void ApplyOverride(IContentPack contentPack, Override obj)
        {
            try
            {
                for (int i = 0; i < Game1.locations.Count; i++)
                {
                    GameLocation location = Game1.locations[i];
                    if (location.Name == obj.MapName)
                    {
                        Game1.locations[i] = (GameLocation)Activator.CreateInstance(Game1.getLocationFromName(obj.MapName).GetType(), contentPack.GetActualAssetKey(obj.FileName), obj.MapName);
                        break;
                    }
                }
            }
            catch (Exception err)
            {
                ModEntry.Logger.ExitGameImmediately("Unable to override location, a unexpected error occured: " + obj, err);
            }
        }
    }
}
