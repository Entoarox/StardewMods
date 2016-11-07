using System.Collections.Generic;

using StardewValley;

using xTile.ObjectModel;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.Framework
{
    internal class LocationHelper : ILocationHelper
    {
        internal static ILocationHelper Singleton { get; } = new LocationHelper();
        void ILocationHelper.SetStaticTile(LocationReference location, string layer, int x, int y, int index, string sheet)
        {
            GameLocation loc = location;
            Layer l = loc.map.GetLayer(layer);
            if (l == null || x > l.LayerWidth || y > l.LayerHeight)
                return;
            Tile t = l.Tiles[x, y];
            TileSheet r = sheet == null ? null : loc.map.GetTileSheet(sheet);
            l.Tiles[x, y] = new StaticTile(l, r == null ? t == null ? loc.map.TileSheets[0] : t.TileSheet : r, BlendMode.Alpha, index);
        }
        void ILocationHelper.SetAnimatedTile(LocationReference location, string layer, int x, int y, int[] indexes, int interval, string sheet)
        {
            GameLocation loc = location;
            Layer l = loc.map.GetLayer(layer);
            if (l == null || x > l.LayerWidth || y > l.LayerHeight)
                return;
            Tile t = l.Tiles[x, y];
            TileSheet r = sheet == null ? null : loc.map.GetTileSheet(sheet);
            TileSheet s = r == null ? t == null ? loc.map.TileSheets[0] : t.TileSheet : r;
            List<StaticTile> ts = new List<StaticTile>();
            foreach (int i in indexes)
                ts.Add(new StaticTile(l, s, BlendMode.Alpha, i));
            l.Tiles[x, y] = new AnimatedTile(l, ts.ToArray(), interval);
        }
        void ILocationHelper.SetTileProperty(LocationReference location, string layer, int x, int y, string key, PropertyValue value)
        {
            GameLocation loc = location;
            Layer l = loc.map.GetLayer(layer);
            if (l == null || x > l.LayerWidth || y > l.LayerHeight)
                return;
            Tile t = l.Tiles[x, y];
            if (t == null)
                return;
            if (t.Properties.ContainsKey(key))
                t.Properties[key] = value;
            else
                t.Properties.Add(key, value);
        }
        PropertyValue ILocationHelper.GetTileProperty(LocationReference location, string layer, int x, int y, string key)
        {
            GameLocation loc = location;
            Layer l = loc.map.GetLayer(layer);
            if (l == null || x > l.LayerWidth || y > l.LayerHeight)
                return null;
            Tile t = l.Tiles[x, y];
            if (t == null)
                return null;
            if (t.Properties.ContainsKey(key))
                return t.Properties[key];
            return null;
        }
        void ILocationHelper.RemoveTile(LocationReference location, string layer, int x, int y)
        {
            GameLocation loc = location;
            Layer l = loc.map.GetLayer(layer);
            if (l == null || x > l.LayerWidth || y > l.LayerHeight)
                return;
            if (l.Tiles[x, y] != null)
                l.Tiles[x, y] = null;
        }
        void ILocationHelper.SetTilesheetProperty(LocationReference location, string sheet, string key, PropertyValue value)
        {
            GameLocation loc = location;
            TileSheet s = loc.map.GetTileSheet(sheet);
            if (s == null)
                return;
            if (s.Properties.ContainsKey(key))
                s.Properties[key] = value;
            else
                s.Properties.Add(key, value);
        }
        PropertyValue ILocationHelper.GetTilesheetProperty(LocationReference location, string sheet, string key)
        {
            GameLocation loc = location;
            TileSheet s = loc.map.GetTileSheet(sheet);
            if (s == null)
                return null;
            if (s.Properties.ContainsKey(key))
                return s.Properties[key];
            return null;
        }
    }
}
