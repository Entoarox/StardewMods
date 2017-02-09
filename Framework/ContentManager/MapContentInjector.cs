using System;
using System.Collections.Generic;

using xTile;
using xTile.Layers;
using xTile.Tiles;
using xTile.ObjectModel;

namespace Entoarox.Framework.ContentManager
{
    class MapContentInjector : ContentHandler
    {
        private static Dictionary<string, List<string>> Mapping = new Dictionary<string, List<string>>();
        private static Dictionary<string, Map> Cache = new Dictionary<string, Map>();
        private static void PatchMap(string assetName, Map @base, List<Map> patches)
        {
            List<string> Warps=new List<string>();
            Map edits = new Map();
            foreach (Map patch in patches)
            {
                foreach (KeyValuePair<string, PropertyValue> prop in patch.Properties)
                    if(prop.Key.Equals("Warp"))
                    {
                        string[] split = ((string)prop.Value).Split(' ');
                        string[] warp = new string[5];
                        for(var c=0;c<split.Length;c++)
                        {
                            warp[c%5] = split[c];
                            if (c % 5 != 5)
                                continue;
                            string final = string.Join(" ", warp);
                            if (!Warps.Contains(final))
                                Warps.Add(final);
                            warp = new string[5];
                        }
                    }
                    else if (!edits.Properties.ContainsKey(prop.Value))
                        edits.Properties.Add(prop);
                    else
                        EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate map property `{prop.Key}` in patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                foreach (TileSheet sheet in patch.TileSheets)
                {
                    if (edits.GetTileSheet(sheet.Id) == null)
                        edits.AddTileSheet(new TileSheet(sheet.Id, @base, sheet.ImageSource, sheet.SheetSize, sheet.TileSize));
                    else
                        EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate tilesheet Id `{sheet.Id}` in patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                    TileSheet editSheet = edits.GetTileSheet(sheet.Id);
                    foreach (KeyValuePair<string, PropertyValue> prop in sheet.Properties)
                        if (!editSheet.Properties.ContainsKey(prop.Key))
                            editSheet.Properties.Add(prop);
                        else
                            EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate tilesheet property `{prop.Key}` for tilesheet `{sheet.Id}` in patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                    for (var c = 0; c < sheet.TileCount; c++)
                        foreach (KeyValuePair<string, PropertyValue> prop in sheet.TileIndexProperties[c])
                            if (!editSheet.TileIndexProperties[c].ContainsKey(prop.Key))
                                editSheet.TileIndexProperties[c].Add(prop);
                            else
                                EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate tile index property `{prop.Key}` for tile index [{c}] in tilesheet `{sheet.Id}` in patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                }
                foreach (Layer layer in patch.Layers)
                {
                    if (edits.GetLayer(layer.Id) == null)
                        edits.AddLayer(new Layer(layer.Id, edits, layer.LayerSize, layer.TileSize));
                    Layer lay = edits.GetLayer(layer.Id);
                    Layer mapLayer = @base.GetLayer(layer.Id);
                    if (lay.LayerWidth < layer.LayerWidth)
                        lay.LayerWidth = layer.LayerWidth;
                    if (lay.LayerHeight < layer.LayerHeight)
                        lay.LayerHeight = layer.LayerHeight;
                    for (var x = 0; x < layer.LayerWidth; x++)
                        for (var y = 0; y < layer.LayerHeight; y++)
                            if (layer.Tiles[x, y] != null)
                            {
                                Tile tile = layer.Tiles[x, y];
                                if (mapLayer != null && mapLayer.LayerWidth > x && mapLayer.LayerHeight > y)
                                {
                                    Tile mapTile = mapLayer.Tiles[x, y];
                                    if (tile.TileIndex == mapTile.TileIndex || tile.TileSheet.Id == mapTile.TileSheet.Id)
                                        continue;
                                }
                                Tile oldTile = lay.Tiles[x, y];
                                if (oldTile == null)
                                {
                                    if (tile is StaticTile)
                                        lay.Tiles[x, y] = new StaticTile(lay, edits.GetTileSheet(tile.TileSheet.Id), tile.BlendMode, tile.TileIndex);
                                    else
                                        lay.Tiles[x, y] = new AnimatedTile(lay, (tile as AnimatedTile).TileFrames, (tile as AnimatedTile).FrameInterval);
                                }
                                else if (oldTile is AnimatedTile || oldTile.TileIndex != tile.TileIndex || oldTile.TileSheet.Id != tile.TileSheet.Id)
                                    EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate tile change for tile at [{x},{y}] on layer `{lay.Id}` in the patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                                foreach (KeyValuePair<string, PropertyValue> prop in tile.Properties)
                                    if (!oldTile.Properties.ContainsKey(prop.Key))
                                        oldTile.Properties.Add(prop);
                                    else
                                        EntoFramework.Logger.Log($"ContentManager: MapContentInjector encountered duplicate tile property `{prop.Key}` for tile at [{x},{y}] on layer `{layer.Id}` in patch at index [{patches.IndexOf(patch)}], using first.", StardewModdingAPI.LogLevel.Warn);
                            }

                }
            }
            if (@base.Properties.ContainsKey("Warp"))
            {
                string[] split = ((string)@base.Properties["Warp"]).Split(' ');
                string[] warp = new string[5];
                for (var c = 0; c < split.Length; c++)
                {
                    warp[c % 5] = split[c];
                    if (c % 5 != 5)
                        continue;
                    string final = string.Join(" ", warp);
                    if(!Warps.Contains(final))
                        Warps.Add(final);
                    warp = new string[5];
                }
            }
            @base.Properties["Warp"] = string.Join(" ", Warps);
            foreach (TileSheet sheet in edits.TileSheets)
            {
                TileSheet old = @base.GetTileSheet(sheet.Id);
                if (old == null)
                {
                    old = new TileSheet(sheet.Id, @base, sheet.ImageSource, sheet.SheetSize, sheet.TileSize);
                    @base.AddTileSheet(old);
                }
                else
                {
                    old.SheetSize = sheet.SheetSize;
                    old.TileSize = sheet.TileSize;
                    old.ImageSource = sheet.ImageSource;
                }
                foreach (KeyValuePair<string, PropertyValue> prop in sheet.Properties)
                    if (!old.Properties.ContainsKey(prop.Key))
                        old.Properties.Add(prop);
                    else
                        old.Properties[prop.Key] = prop.Value;
                for(var c=0;c<sheet.TileCount;c++)
                    foreach (KeyValuePair<string, PropertyValue> prop in sheet.TileIndexProperties[c])
                        if (!old.TileIndexProperties[c].ContainsKey(prop.Key))
                            old.TileIndexProperties[c].Add(prop);
                        else
                            old.TileIndexProperties[c][prop.Key] = prop.Value;
            }
            foreach (Layer layer in edits.Layers)
            {
                if (@base.GetLayer(layer.Id) == null)
                    @base.AddLayer(new Layer(layer.Id, @base, layer.LayerSize, layer.TileSize));
                Layer lay = @base.GetLayer(layer.Id);
                if (lay.LayerWidth < layer.LayerWidth)
                    lay.LayerWidth = layer.LayerWidth;
                if (lay.LayerHeight < layer.LayerHeight)
                    lay.LayerHeight = layer.LayerHeight;
                for (var x = 0; x < layer.LayerWidth; x++)
                    for (var y = 0; y < layer.LayerHeight; y++)
                        if (layer.Tiles[x, y] != null)
                        {
                            Tile tile = layer.Tiles[x, y];
                            if (tile is StaticTile)
                            {
                                StaticTile oldTile = (StaticTile)lay.Tiles[x, y];
                                if (oldTile.TileIndex != tile.TileIndex || oldTile.TileSheet.Id != tile.TileSheet.Id)
                                    lay.Tiles[x, y] = new StaticTile(lay, @base.GetTileSheet(tile.TileSheet.Id), tile.BlendMode, tile.TileIndex);
                            }
                            else
                                lay.Tiles[x, y] = new AnimatedTile(lay, (tile as AnimatedTile).TileFrames, (tile as AnimatedTile).FrameInterval);
                            foreach (KeyValuePair<string, PropertyValue> prop in tile.Properties)
                                if (!lay.Tiles[x,y].Properties.ContainsKey(prop.Key))
                                    lay.Tiles[x, y].Properties.Add(prop);
                                else
                                    lay.Tiles[x, y].Properties[prop.Key] = prop.Value;
                        }

            }
        }
        public static void Register(string assetName, string filePath)
        {
            if (!Mapping.ContainsKey(assetName))
                Mapping.Add(assetName, new List<string>());
            Mapping[assetName].Add(GetPlatformSafePath(filePath));
        }
        public override bool Injector { get; } = true;
        public override bool CanInject<T>(string assetName)
        {
            return typeof(T)==typeof(Map)&&Mapping.ContainsKey(assetName);
        }
        public override void Inject<T>(string assetName, ref T asset)
        {
            List<Map> Maps = new List<Map>();
            foreach(string file in Mapping[assetName])
                try
                {
                    Maps.Add(ModManager.Load<Map>(GetModsRelativePath(file)));
                    EntoFramework.Logger.Log($"ContentManager: MapContentInjector has collected injection data for `{assetName}` at index [{(Maps.Count - 1)}].", StardewModdingAPI.LogLevel.Trace);
                }
                catch(Exception err)
                {
                    EntoFramework.Logger.Log(StardewModdingAPI.LogLevel.Error, "ContentManager: MapContentInjector failed to load injection data for `" + assetName + "` from file:" + file, err);
                }
            PatchMap(assetName, (Map)(object)asset, Maps);
        }
        public override T Load<T>(string assetName, Func<string, T> loadBase)
        {
            throw new NotImplementedException();
        }
    }
}
