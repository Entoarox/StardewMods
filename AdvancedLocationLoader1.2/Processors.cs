using System;
using System.IO;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using Warp = StardewValley.Warp;

using Entoarox.Framework;

using Entoarox.AdvancedLocationLoader.Configs;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class Processors
    {
        internal static ILocationHelper LocationHelper = EntoFramework.GetLocationHelper();
        internal static IContentRegistry ContentRegistry = EntoFramework.GetContentRegistry();
        internal static void ApplyTile(Tile tile)
        {
            AdvancedLocationLoaderMod.Logger.Trace(tile.ToString());
            try
            {
                if (!string.IsNullOrEmpty(tile.Conditions) && !Conditions.CheckConditionList(tile.Conditions))
                    return;
                if (tile.TileIndex != null)
                    if (tile.TileIndex < 0)
                        LocationHelper.RemoveTile(tile.MapName, tile.LayerId, tile.TileX, tile.TileY);
                    else
                        LocationHelper.SetStaticTile(tile.MapName, tile.LayerId, tile.TileX, tile.TileY, (int)tile.TileIndex, tile.SheetId);
                else
                    LocationHelper.SetAnimatedTile(tile.MapName, tile.LayerId, tile.TileX, tile.TileY, tile.TileIndexes, (int)tile.Interval, tile.SheetId);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to patch tile, a unexpected error occured: "+tile.ToString(), err);
            }
        }
        internal static void ApplyProperty(Property property)
        {
            AdvancedLocationLoaderMod.Logger.Trace(property.ToString());
            try
            {
                if (string.IsNullOrEmpty(property.Conditions) || Conditions.CheckConditionList(property.Conditions))
                    LocationHelper.SetTileProperty(property.MapName, property.LayerId, property.TileX, property.TileY, property.Key, property.Value);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to patch property, a unexpected error occured: " + property.ToString(), err);
            }
        }
        internal static void ApplyWarp(Configs.Warp warp)
        {
            AdvancedLocationLoaderMod.Logger.Trace(warp.ToString());
            try
            {
                if (!string.IsNullOrEmpty(warp.Conditions) && !Conditions.CheckConditionList(warp.Conditions))
                    return;
                Warp _warp = new Warp(warp.TileX, warp.TileY, warp.TargetName, warp.TargetX, warp.TargetY, false);
                GameLocation loc = Game1.getLocationFromName(warp.MapName);
                if (loc.warps.Contains(_warp))
                    loc.warps.Remove(_warp);
                loc.warps.Add(_warp);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to patch warp, a unexpected error occured: " + warp.ToString(), err);
            }
        }
        internal static void ApplyTilesheet(Tilesheet tilesheet)
        {
            AdvancedLocationLoaderMod.Logger.Trace(tilesheet.ToString());
            try
            {
                GameLocation location = Game1.getLocationFromName(tilesheet.MapName);
                string fakepath = Path.Combine(Path.GetDirectoryName(tilesheet.FileName), "all_sheet_paths_objects", tilesheet.SheetId, Path.GetFileName(tilesheet.FileName));
                if (tilesheet.Seasonal)
                    fakepath = fakepath.Replace("all_sheet_paths_objects", Path.Combine("all_sheet_paths_objects", Game1.currentSeason));
                ContentRegistry.RegisterXnb(fakepath, tilesheet.Seasonal ? (tilesheet.FileName + "_" + Game1.currentSeason) : tilesheet.FileName);
                if (location.map.GetTileSheet(tilesheet.SheetId) != null)
                {
                    AdvancedLocationLoaderMod.Logger.Trace("Detected pre-existing tilesheet, patching to refer to correct file location");
                    location.map.GetTileSheet(tilesheet.SheetId).ImageSource = fakepath;
                }
                else
                {
                    Texture2D sheet = Game1.content.Load<Texture2D>(fakepath);
                    location.map.AddTileSheet(new xTile.Tiles.TileSheet(location.map, fakepath, new xTile.Dimensions.Size(sheet.Width, sheet.Height), new xTile.Dimensions.Size(16, 16)));
                }
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to patch tilesheet, a unexpected error occured: " + tilesheet.ToString(), err);
            }
        }
        internal static void ApplyLocation(Location location)
        {
            AdvancedLocationLoaderMod.Logger.Trace(location.ToString());
            try
            {
                GameLocation loc;
                ContentRegistry.RegisterXnb(location.FileName, location.FileName);
                xTile.Map map = Game1.content.Load<xTile.Map>(location.FileName);
                switch (location.Type)
                {
                    case "Cellar":
                        loc = new StardewValley.Locations.Cellar(map, location.MapName);
                        loc.objects = new SerializableDictionary<Microsoft.Xna.Framework.Vector2, StardewValley.Object>();
                        break;
                    default:
                        loc = new GameLocation(map, location.MapName);
                        break;
                }
                loc.isOutdoors = location.Outdoor;
                loc.isFarm = location.Farmable;
                Game1.locations.Add(loc);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to add custom location, a unexpected error occured: " + location.ToString(), err);
            }
        }
        internal static void ApplyOverride(Override obj)
        {
            AdvancedLocationLoaderMod.Logger.Trace(obj.ToString());
            try
            {
                ContentRegistry.RegisterXnb(obj.FileName, obj.FileName);
                Game1.locations[Game1.locations.FindIndex(l => l.name == obj.MapName)] = (GameLocation)Activator.CreateInstance(Game1.getLocationFromName(obj.MapName).GetType(), Game1.content.Load<xTile.Map>(obj.FileName), obj.MapName);
            }
            catch (Exception err)
            {
                AdvancedLocationLoaderMod.Logger.Error("Unable to override location, a unexpected error occured: "+obj.ToString(), err);
            }
        }
    }
}
