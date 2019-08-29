using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using xTile;
using xTile.Tiles;

namespace Entoarox.TDiffPatcher.V1R0P0
{
    enum Actions : byte
    {
        SetStaticTile,
        SetAnimatedTile,
        RemoveTile,
        SetTileProperty,
        RemoveTileProperty,
        SetTileIndexProperty,
        RemoveTileIndexProperty,
        SetTilesheetProperty,
        RemoveTilesheetProperty,
        SetLayerProperty,
        RemoveLayerProperty,
        SetMapProperty,
        RemoveMapProperty,
        AddLayer,
        RemoveLayer,
        ResizeLayer,
    }
    public static class TDiffPatcher
    {
        public static void ApplyPatch(Map map, Stream patch)
        {
            var reader = new BinaryReader(patch);
            string signature = string.Join(null, reader.ReadBytes(5).Cast<char>());
            if (!signature.Equals("TDIFF"))
                throw new InvalidDataException("Input stream is not a TDIFF file.");
            byte revision = reader.ReadByte();
            switch(revision)
            {
                case 0:
                    while(patch.Position<patch.Length)
                    {
                        Actions action = (Actions)reader.ReadByte();
                        switch(action)
                        {
                            case Actions.SetStaticTile:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte x = reader.ReadByte();
                                    byte y = reader.ReadByte();
                                    string sheetId = reader.ReadTiny();
                                    var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                    int tileIndex = reader.ReadInt32();
                                    layer.Tiles[x, y] = new StaticTile(layer, sheet, BlendMode.Additive, tileIndex);
                                }
                                break;
                            case Actions.SetAnimatedTile:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte x = reader.ReadByte();
                                    byte y = reader.ReadByte();
                                    long interval = reader.ReadInt64();
                                    int frames = reader.ReadInt32();
                                    var list = new StaticTile[frames];
                                    for (int c = 0; c < frames; c++)
                                    {
                                        string sheetId = reader.ReadTiny();
                                        var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                        int tileIndex = reader.ReadInt32();
                                        list[c] = new StaticTile(layer, sheet, BlendMode.Additive, tileIndex);
                                    }
                                    layer.Tiles[x, y] = new AnimatedTile(layer, list, interval);
                                }
                                break;
                            case Actions.RemoveTile:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte x = reader.ReadByte();
                                    byte y = reader.ReadByte();
                                    layer.Tiles[x, y] = null;
                                }
                                break;
                            case Actions.SetTileProperty:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte x = reader.ReadByte();
                                    byte y = reader.ReadByte();
                                    string propertyKey = reader.ReadTiny();
                                    string propertyValue = reader.ReadString();
                                    if (layer.Tiles[x, y] == null)
                                        throw new NullReferenceException("Map does not contain a tile at the given position: " + layerId + "[" + x + "," + y + "]");
                                    if (layer.Tiles[x, y].Properties.ContainsKey(propertyKey))
                                        layer.Tiles[x, y].Properties[propertyKey] = propertyValue;
                                    else
                                        layer.Tiles[x, y].Properties.Add(propertyKey, propertyValue);
                                }
                                break;
                            case Actions.RemoveTileProperty:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte x = reader.ReadByte();
                                    byte y = reader.ReadByte();
                                    string propertyKey = reader.ReadTiny();
                                    if (layer.Tiles[x, y] == null)
                                        throw new NullReferenceException("Map does not contain a tile at the given position: " + layerId + "[" + x + "," + y + "]");
                                    if (layer.Tiles[x, y].Properties.ContainsKey(propertyKey))
                                        layer.Tiles[x, y].Properties.Remove(propertyKey);
                                }
                                break;
                            case Actions.SetTileIndexProperty:
                                {
                                    string sheetId = reader.ReadTiny();
                                    var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                    int tileIndex = reader.ReadInt32();
                                    string propertyKey = reader.ReadTiny();
                                    string propertyValue = reader.ReadString();
                                    if(sheet.TileIndexProperties[tileIndex].ContainsKey(propertyKey))
                                        sheet.TileIndexProperties[tileIndex][propertyKey] = propertyValue;
                                    else
                                        sheet.TileIndexProperties[tileIndex].Add(propertyKey, propertyValue);
                                }
                                break;
                            case Actions.RemoveTileIndexProperty:
                                {
                                    string sheetId = reader.ReadTiny();
                                    var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                    int tileIndex = reader.ReadInt32();
                                    string propertyKey = reader.ReadTiny();
                                    if (sheet.TileIndexProperties[tileIndex].ContainsKey(propertyKey))
                                        sheet.TileIndexProperties[tileIndex].Remove(propertyKey);
                                }
                                break;
                            case Actions.SetTilesheetProperty:
                                {
                                    string sheetId = reader.ReadTiny();
                                    var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                    string propertyKey = reader.ReadTiny();
                                    string propertyValue = reader.ReadString();
                                    if (sheet.Properties.ContainsKey(propertyKey))
                                        sheet.Properties[propertyKey] = propertyValue;
                                    else
                                        sheet.Properties.Add(propertyKey, propertyValue);
                                }
                                break;
                            case Actions.RemoveTilesheetProperty:
                                {
                                    string sheetId = reader.ReadTiny();
                                    var sheet = map.GetTileSheet(sheetId) ?? throw new NullReferenceException("Map does not contain a tilesheet with given Id: " + sheetId);
                                    string propertyKey = reader.ReadTiny();
                                    if (sheet.Properties.ContainsKey(propertyKey))
                                        sheet.Properties.Remove(propertyKey);
                                }
                                break;
                            case Actions.SetLayerProperty:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    string propertyKey = reader.ReadTiny();
                                    string propertyValue = reader.ReadString();
                                    if (layer.Properties.ContainsKey(propertyKey))
                                        layer.Properties[propertyKey] = propertyValue;
                                    else
                                        layer.Properties.Add(propertyKey, propertyValue);
                                }
                                break;
                            case Actions.RemoveLayerProperty:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    string propertyKey = reader.ReadTiny();
                                    if (layer.Properties.ContainsKey(propertyKey))
                                        layer.Properties.Remove(propertyKey);
                                }
                                break;
                            case Actions.SetMapProperty:
                                {
                                    string propertyKey = reader.ReadTiny();
                                    string propertyValue = reader.ReadString();
                                    if (map.Properties.ContainsKey(propertyKey))
                                        map.Properties[propertyKey] = propertyValue;
                                    else
                                        map.Properties.Add(propertyKey, propertyValue);
                                }
                                break;
                            case Actions.RemoveMapProperty:
                                {
                                    string propertyKey = reader.ReadTiny();
                                    if (map.Properties.ContainsKey(propertyKey))
                                        map.Properties.Remove(propertyKey);
                                }
                                break;
                            case Actions.AddLayer:
                                {
                                    string layerId = reader.ReadTiny();
                                    if(map.GetLayer(layerId)==null)
                                    {
                                        byte width = reader.ReadByte();
                                        byte height = reader.ReadByte();
                                        map.AddLayer(new xTile.Layers.Layer(layerId, map, new xTile.Dimensions.Size(width, height), new xTile.Dimensions.Size(16)));
                                    }
                                }
                                break;
                            case Actions.RemoveLayer:
                                {
                                    string layerId = reader.ReadTiny();
                                    if (map.GetLayer(layerId) != null)
                                        map.RemoveLayer(map.GetLayer(layerId));
                                }
                                break;
                            case Actions.ResizeLayer:
                                {
                                    string layerId = reader.ReadTiny();
                                    var layer = map.GetLayer(layerId) ?? throw new NullReferenceException("Map does not contain a layer with given Id: " + layerId);
                                    byte width = reader.ReadByte();
                                    byte height = reader.ReadByte();
                                    layer.LayerSize = new xTile.Dimensions.Size(width, height);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new InvalidDataException("Input stream uses a unsupported TDIFF revision:" + revision);
            }
        }
        private static string ReadTiny(this BinaryReader reader)
        {
            return string.Join(null, reader.ReadChars(reader.ReadByte()));
        }
    }
}
