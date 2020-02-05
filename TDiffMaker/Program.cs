using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile;
using xTile.Layers;
using xTile.Tiles;
using xTile.ObjectModel;

using ICSharpCode.SharpZipLib.GZip;

namespace Entoarox.TDiffMaker
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

        EOF=255
    }
    class Program
    {
        static void Main(string[] args)
        {
            var manager = xTile.Format.FormatManager.Instance;
            Console.WriteLine("TDiffMaker 1.0.0 by Entoarox\n\n");
            Console.WriteLine("Source file:");
            string source = Path.GetFullPath(Console.ReadLine());
            if (Path.GetExtension(source).Equals(string.Empty))
                source += ".tbin";
            while (!File.Exists(source) || !Path.GetExtension(source).Equals(".tbin"))
            {
                var memory = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                if (!Path.GetExtension(source).Equals(".tbin"))
                    Console.WriteLine("File does not have the tbin extension, please try again.");
                else
                    Console.WriteLine("File not found, please try again.");
                Console.ForegroundColor = memory;
                Console.WriteLine("Source file:");
                source = Path.GetFullPath(Console.ReadLine());
                if (Path.GetExtension(source).Equals(string.Empty))
                    source += ".tbin";
            }
            Console.WriteLine("Target file:");
            string target = Path.GetFullPath(Console.ReadLine());
            if (Path.GetExtension(target).Equals(string.Empty))
                target += ".tbin";
            while (!File.Exists(target) || !Path.GetExtension(target).Equals(".tbin"))
            {
                var memory = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                if(!Path.GetExtension(target).Equals(".tbin"))
                    Console.WriteLine("File does not have the tbin extension, please try again.");
                else
                    Console.WriteLine("File not found, please try again.");
                Console.ForegroundColor = memory;
                Console.WriteLine("Target file:");
                target = Path.GetFullPath(Console.ReadLine());
                if (Path.GetExtension(target).Equals(string.Empty))
                    target += ".tbin";
            }
            Console.WriteLine("Output filename:");
            string output = Console.ReadLine();
            if (Path.GetExtension(output).Equals(string.Empty))
                output += ".tdiff";
            output = Path.GetFullPath(output);
            while (File.Exists(output))
            {
                var memory = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File already exists, please try again.");
                Console.ForegroundColor = memory;
                Console.WriteLine("Output filename:");
                output = Console.ReadLine();
                if (Path.GetExtension(output).Equals(string.Empty))
                    output += ".tdiff";
                output = Path.GetFullPath(output);
            }
            try
            {
                Console.WriteLine("Loading maps for tdiff creation...");
                Map sourceMap = manager.LoadMap(source);
                Map targetMap = manager.LoadMap(target);
                Console.WriteLine("Parsing maps, this might take a while...");
                using (var stream = File.OpenWrite(output))
                using (var zipStream = new GZipOutputStream(stream))
                using (var writer = new BinaryWriter(zipStream))
                {
                    writer.Write("TDIFF".ToCharArray());
                    writer.Write((byte)0);
                    // Anything that needs the layers
                    foreach (Layer layer in sourceMap.Layers)
                    {
                        if (targetMap.GetLayer(layer.Id) == null)
                        {
                            writer.Write((byte)Actions.RemoveLayer);
                            writer.WriteTiny(layer.Id);
                        }
                    }
                    foreach (Layer layer in targetMap.Layers)
                    {
                        var sLayer = sourceMap.GetLayer(layer.Id);
                        if (sLayer == null)
                        {
                            writer.Write((byte)Actions.AddLayer);
                            writer.WriteTiny(layer.Id);
                            writer.Write((byte)layer.LayerWidth);
                            writer.Write((byte)layer.LayerHeight);
                        }
                        else if (sLayer.LayerWidth != layer.LayerWidth || sLayer.LayerHeight != layer.LayerHeight)
                        {
                            writer.Write((byte)Actions.ResizeLayer);
                            writer.WriteTiny(layer.Id);
                            writer.Write((byte)layer.LayerWidth);
                            writer.Write((byte)layer.LayerHeight);
                        }
                        // Check for tile changes
                        for (byte x = 0; x < layer.LayerWidth; x++)
                            for (byte y = 0; y < layer.LayerHeight; y++)
                            {
                                // Check if we should just completely skip checking this tile
                                if (sLayer!=null && (x >= sLayer.LayerWidth || y >= sLayer.LayerHeight || sLayer.Tiles[x, y] == null) && layer.Tiles[x, y] == null)
                                    continue;
                                // If we should do something with the current tile
                                if (sLayer==null || !AreEqualTiles(sLayer.Tiles[x, y], layer.Tiles[x, y]))
                                {
                                    if (sLayer!=null && layer.Tiles[x, y] == null)
                                    {
                                        writer.Write((byte)Actions.RemoveTile);
                                        writer.WriteTiny(layer.Id);
                                        writer.Write(x);
                                        writer.Write(y);
                                    }
                                    else if (layer.Tiles[x, y] is StaticTile sTile)
                                    {
                                        writer.Write((byte)Actions.SetStaticTile);
                                        writer.WriteTiny(layer.Id);
                                        writer.Write(x);
                                        writer.Write(y);
                                        writer.WriteTiny(sTile.TileSheet.Id);
                                        writer.Write(sTile.TileIndex);
                                        var buffer = new MemoryStream();
                                        var preface = new BinaryWriter(buffer);
                                        preface.WriteTiny(layer.Id);
                                        preface.Write(x);
                                        preface.Write(y);
                                        MapProperties(writer, Actions.RemoveTileProperty, Actions.SetTileProperty, buffer.ToArray(), sLayer?.Tiles[x, y]?.Properties ?? new PropertyCollection(), layer.Tiles[x, y].Properties);
                                    }
                                    else if (layer.Tiles[x, y] is AnimatedTile aTile)
                                    {
                                        writer.Write((byte)Actions.SetAnimatedTile);
                                        writer.WriteTiny(layer.Id);
                                        writer.Write(x);
                                        writer.Write(y);
                                        writer.Write(aTile.FrameInterval);
                                        writer.Write(aTile.TileFrames.Length);
                                        foreach (var fTile in aTile.TileFrames)
                                        {
                                            writer.WriteTiny(fTile.TileSheet.Id);
                                            writer.Write(fTile.TileIndex);
                                        }
                                        var buffer = new MemoryStream();
                                        var preface = new BinaryWriter(buffer);
                                        preface.WriteTiny(layer.Id);
                                        preface.Write(x);
                                        preface.Write(y);
                                        MapProperties(writer, Actions.RemoveTileProperty, Actions.SetTileProperty, buffer.ToArray(), sLayer?.Tiles[x, y]?.Properties ?? new PropertyCollection(), layer.Tiles[x, y].Properties);
                                    }
                                }
                                else
                                {
                                    var buffer = new MemoryStream();
                                    var preface = new BinaryWriter(buffer);
                                    preface.WriteTiny(layer.Id);
                                    preface.Write(x);
                                    preface.Write(y);
                                    MapProperties(writer, Actions.RemoveTileProperty, Actions.SetTileProperty, buffer.ToArray(), sLayer?.Tiles[x, y]?.Properties ?? new PropertyCollection(), layer.Tiles[x, y].Properties);
                                }
                            }
                        var lbuffer = new MemoryStream();
                        var lpreface = new BinaryWriter(lbuffer);
                        lpreface.WriteTiny(layer.Id);
                        MapProperties(writer, Actions.RemoveLayerProperty, Actions.SetLayerProperty, lbuffer.ToArray(), sLayer?.Properties ?? new PropertyCollection(), layer.Properties);
                    }
                    MapProperties(writer, Actions.RemoveMapProperty, Actions.SetMapProperty, new byte[0], sourceMap.Properties, targetMap.Properties);
                    foreach (var sheet in targetMap.TileSheets)
                    {
                        var sSheet = sourceMap.GetTileSheet(sheet.Id);
                        if (sSheet == null)
                            throw new KeyNotFoundException("Target map contains a tilesheet which is not present in the source map: " + sheet.Id);
                        var sBuffer = new MemoryStream();
                        var sPreface = new BinaryWriter(sBuffer);
                        sPreface.WriteTiny(sheet.Id);
                        MapProperties(writer, Actions.RemoveTilesheetProperty, Actions.SetTilesheetProperty, sBuffer.ToArray(), sheet.Properties, sSheet.Properties);
                        for (int c = 0; c < sheet.TileCount; c++)
                        {
                            var tBuffer = new MemoryStream();
                            var tPreface = new BinaryWriter(tBuffer);
                            tPreface.WriteTiny(sheet.Id);
                            tPreface.Write(c);
                            MapProperties(writer, Actions.RemoveTileIndexProperty, Actions.SetTileIndexProperty, tBuffer.ToArray(), sheet.TileIndexProperties[c], sSheet.TileIndexProperties[c]);
                        }
                    }
                    writer.Write((byte)Actions.EOF);
                }
                var memory = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsing complete, tdiff file has been created!");
                Console.ForegroundColor = memory;
            }
            catch(Exception err)
            {
                if (File.Exists(output))
                    File.Delete(output);
                var memory = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Patching failed, error follows:\n" + err);
                Console.ForegroundColor = memory;
            }
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
        private static bool AreEqualTiles(Tile first, Tile second)
        {
            if (first == null || second == null)
                return first == second;
            if(first is AnimatedTile fTile)
            {
                if (second is StaticTile)
                    return false;
                var sTile = second as AnimatedTile;
                if (fTile.FrameInterval != sTile.FrameInterval || fTile.TileFrames.Length != sTile.TileFrames.Length)
                    return false;
                for (int c = 0; c < fTile.TileFrames.Length; c++)
                    if (!AreEqualTiles(fTile.TileFrames[c], sTile.TileFrames[c]))
                        return false;
                return true;
            }
            else
            {
                if (second is AnimatedTile)
                    return false;
                return first.TileSheet.Id.Equals(second.TileSheet.Id) && first.TileIndex == second.TileIndex;
            }
        }
        private static void MapProperties(BinaryWriter writer, Actions remove, Actions set, byte[] preface, IPropertyCollection source, IPropertyCollection target)
        {
            foreach(var item in source)
            {
                if(!target.ContainsKey(item.Key))
                {
                    writer.Write((byte)remove);
                    writer.Write(preface);
                    writer.WriteTiny(item.Key);
                }
                else if(!target[item.Key].ToString().Equals(item.Value.ToString()))
                {
                    writer.Write((byte)set);
                    writer.Write(preface);
                    writer.WriteTiny(item.Key);
                    writer.Write((string)item.Value);
                }
            }
        }
    }
    static class ReaderHelper
    {
        public static void WriteTiny(this BinaryWriter writer, string value)
        {
            char[] arr = value.ToCharArray();
            if (arr.Length > byte.MaxValue)
                throw new IndexOutOfRangeException("Unable to create TDIFF file, writing of TinyString failed because string is bigger then allowed: " + value);
            writer.Write((byte)arr.Length);
            writer.Write(arr);
        }
    }
}
