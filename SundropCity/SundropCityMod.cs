using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;

using StardewValley;

namespace SundropCity
{
    public class SundropCityMod : Mod
    {
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        private List<string> Maps;

        public override void Entry(IModHelper helper)
        {
            // Define internals
            SHelper = helper;
            SMonitor = this.Monitor;

            // Add custom loader to make custom tree work
            helper.Content.AssetLoaders.Add(new SundropLoader());

            // Load maps
            List<string> maps = new List<string>();
            foreach(string file in Directory.EnumerateFiles(Path.Combine(helper.DirectoryPath,"assets","Maps")))
            {
                string ext = Path.GetExtension(file);
                this.Monitor.Log($"Checking file: {file} (ext: {ext})", LogLevel.Trace);
                if (!ext.Equals(".tbin"))
                    continue;
                string map = Path.GetFileName(file);
                this.Monitor.Log("Found sundrop location: " + map, LogLevel.Trace);
                maps.Add(map);
                helper.Content.Load<xTile.Map>(Path.Combine("assets", "Maps", map));
            }
            this.Maps = maps;

            // Setup events
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        private void Setup()
        {
            // Handle patches specific to other mods
            var registry = this.Helper.Data.ReadJsonFile<Dictionary<string, string>>("assets/TownPatches/registry.json");
            string patchVersion = "Vanilla";
            foreach(var pair in registry)
            {
                if (!this.Helper.ModRegistry.IsLoaded(pair.Key))
                    continue;
                patchVersion = pair.Value;
                break;
            }
            // Setup for patching
            var town = Game1.getLocationFromName("Town");
            var patch = this.Helper.Content.Load<xTile.Map>("assets/TownPatches/"+patchVersion+".tbin");
            var layers = town.map.Layers.Where(a => patch.GetLayer(a.Id) != null).Select(a => a.Id);
            // Perform patching
            foreach (string layer in layers)
            {
                xTile.Layers.Layer toLayer = town.map.GetLayer(layer),
                    fromLayer = patch.GetLayer(layer);
                // First patch area: The road
                for (int x = 96; x < 120; x++)
                    for (int y = 53; y < 61; y++)
                    {
                        var tile = fromLayer.Tiles[x, y];
                        if (tile == null)
                            toLayer.Tiles[x, y] = null;
                        else
                            toLayer.Tiles[x, y] = new xTile.Tiles.StaticTile(toLayer, town.map.GetTileSheet(tile.TileSheet.Id), xTile.Tiles.BlendMode.Additive, tile.TileIndex);
                        var vect = new Microsoft.Xna.Framework.Vector2(x, y);
                        town.largeTerrainFeatures.Filter(a => a.tilePosition.Value != vect);
                    }
                // Second patch area: replacing the pink tree
                for (int x = 110; x < 116; x++)
                    for (int y = 46; y < 53; y++)
                    {
                        var tile = fromLayer.Tiles[x, y];
                        if (tile == null)
                            toLayer.Tiles[x, y] = null;
                        else
                            toLayer.Tiles[x, y] = new xTile.Tiles.StaticTile(toLayer, town.map.GetTileSheet(tile.TileSheet.Id), xTile.Tiles.BlendMode.Additive, tile.TileIndex);
                    }
            }
            // Setup warps to sundrop [TEMP: Will become warps to SundropBusStop map in the future]
            town.warps.Add(new Warp(120, 55, "SundropPromenade", 1, 29, false));
            town.warps.Add(new Warp(120, 56, "SundropPromenade", 1, 30, false));
            town.warps.Add(new Warp(120, 57, "SundropPromenade", 1, 31, false));
            town.warps.Add(new Warp(120, 58, "SundropPromenade", 1, 32, false));
            // Add new locations
            foreach (string map in this.Maps)
            {
                this.Monitor.Log("Adding sundrop location: "+ Path.GetFileNameWithoutExtension(map), LogLevel.Trace);
                Game1.locations.Add(new SundropLocation(this.Helper.Content.GetActualAssetKey(Path.Combine("assets", "Maps", map)), Path.GetFileNameWithoutExtension(map)));
            }
            // Add warp back to Pelican [TEMP: Will be removed once proper travel is implemented]
            Game1.getLocationFromName("SundropPromenade").setTileProperty(3, 37, "Buildings", "Action", "Warp 119 56 Town");
        }

        private void OnSaveLoaded(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaved(object s, EventArgs e)
        {
            this.Setup();
        }
        private void OnSaving(object s, EventArgs e)
        {
            Game1.locations = Game1.locations.Where(a => !(a is SundropLocation)).ToList();
        }
    }
}
