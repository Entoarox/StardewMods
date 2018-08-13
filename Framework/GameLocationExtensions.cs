using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

using StardewValley;

using xTile.Tiles;
using xTile.Layers;
using xTile.ObjectModel;

namespace Entoarox.Framework
{
    public static class GameLocationExtensions
    {
        public static bool HasTile(this GameLocation self, int x, int y, string layer)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            return _layer.Tiles[x, y] != null;
        }
        public static void SetTile(this GameLocation self, int x, int y, string layer, int index, string sheet = null)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (_layer.LayerWidth < x || x< 0 )
                throw new ArgumentOutOfRangeException(nameof(x));
            if(_layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            TileSheet _sheet;
            if (sheet != null)
                _sheet = self.map.GetTileSheet(sheet);
            else if (_layer.Tiles[x, y] != null)
                _sheet = _layer.Tiles[x, y].TileSheet;
            else
                _sheet = self.map.TileSheets[0];
            if (_sheet == null)
                throw new ArgumentNullException(nameof(sheet));
            if (_sheet.TileCount < index || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Must be at least 0 and less then " + _sheet.TileCount + " for tilesheet with Id `" + _sheet.Id + '`');
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
        }
        public static void SetTile(this GameLocation self, int x, int y, string layer, int[] indexes, int interval, string sheet=null)
        {
            if (interval < 0)
                throw new ArgumentOutOfRangeException(nameof(interval));
            var _layer = self.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (_layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (_layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            TileSheet _sheet;
            if (sheet != null)
                _sheet = self.map.GetTileSheet(sheet);
            else if (_layer.Tiles[x, y] != null)
                _sheet = _layer.Tiles[x, y].TileSheet;
            else
                _sheet = self.map.TileSheets[0];
            if (_sheet == null)
                throw new ArgumentNullException(nameof(sheet));
            for (int c = 0; c < indexes.Length; c++)
                if (_sheet.TileCount < indexes[c] || indexes[c] < 0)
                    throw new ArgumentOutOfRangeException(nameof(indexes) + '[' + c.ToString() + ']', "Must be at least 0 and less then " + _sheet.TileCount + " for tilesheet with Id `" + _sheet.Id + '`');
            _layer.Tiles[x, y] = new AnimatedTile(_layer, indexes.Select(index => new StaticTile(_layer, _sheet, BlendMode.Alpha, index)).ToArray(), interval);
        }
        public static void RemoveTile(this GameLocation self, int x, int y, string layer)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (_layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (_layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            _layer.Tiles[x, y] = null;
        }
        public static bool TrySetTile(this GameLocation self, int x, int y, string layer, int index, string sheet = null)
        {
            var _sheet = sheet == null ? self.map.TileSheets[0] : self.map.GetTileSheet(sheet);
            if (_sheet == null || _sheet.TileCount < index || index < 0)
                return false;
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
            return true;
        }
        public static bool TrySetTile(this GameLocation self, int x, int y, string layer, int[] indexes, int interval, string sheet = null)
        {
            var _sheet = sheet == null ? self.map.TileSheets[0] : self.map.GetTileSheet(sheet);
            if (_sheet == null || interval < 0)
                return false;
            if (indexes.Any(t => _sheet.TileCount < t || t < 0))
            {
                return false;
            }
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            _layer.Tiles[x, y] = new AnimatedTile(_layer, indexes.Select(index => new StaticTile(_layer, _sheet, BlendMode.Alpha, index)).ToArray(), interval);
            return true;
        }
        public static bool TryRemoveTile(this GameLocation self, int x, int y, string layer)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            _layer.Tiles[x, y] = null;
            return true;
        }
        public static void SetTileProperty(this GameLocation self, int x, int y, string layer, string key, string value)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (_layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (_layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (_layer.Tiles[x, y] == null)
                throw new ArgumentNullException(nameof(x) + ',' + nameof(y));
            if (_layer.Tiles[x, y].Properties.ContainsKey(key))
                _layer.Tiles[x, y].Properties[key] = value;
            else
                _layer.Tiles[x, y].Properties.Add(key, value);
        }
        public static bool TrySetTileProperty(this GameLocation self, int x, int y, string layer, string key, string value)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            if (_layer.Tiles[x, y].Properties.ContainsKey(key))
                _layer.Tiles[x, y].Properties[key] = value;
            else
                _layer.Tiles[x, y].Properties.Add(key, value);
            return true;
        }
        public static string GetTileProperty(this GameLocation self, int x, int y, string layer, string key)
        {
            var _layer = self.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (_layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (_layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (_layer.Tiles[x, y].Properties.ContainsKey(key))
                return _layer.Tiles[x, y].Properties[key];
            throw new ArgumentNullException(nameof(key));
        }
        public static bool TryGetTileProperty(this GameLocation self, int x, int y, string layer, string key, out string value)
        {
            value = null;
            var _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0 || !_layer.Tiles[x, y].Properties.ContainsKey(key))
                return false;
            value = _layer.Tiles[x, y].Properties[key];
            return true;

        }

        public static void AddWarp(this GameLocation self, int x, int y, string target, int targetX, int targetY, bool replace=true)
        {
            List<Warp> warps = null;
            foreach (var w in self.warps)
            {
                warps.Add(w);
            }
            if (!replace && warps.Exists(a => a.X == x && a.Y == y))
                throw new ArgumentException("Index already set " + x.ToString() + ',' + y.ToString());
            else if (warps != null)
                foreach (var warp in warps.Where(a => a.X == x && a.Y == y))
                    self.warps.Remove(warp);
            self.warps.Add(new Warp(x, y, target, x, y, false));
        }
        public static void RemoveWarp(this GameLocation self, int x, int y)
        {
            List<Warp> warps = null;
            foreach (var w in self.warps)
            {
                warps.Add(w);
            }
            if (warps == null) return;
            foreach (var w in warps.Where(a => a.X ==x && a.Y == y))
            {
                self.warps.Remove(w);
            }
        }
        public static bool IsPassable(this GameLocation self, int x, int y)
        {
            var layer = self.map.GetLayer("Back");
            if (layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            var tile = layer.Tiles[x, y];
            if (tile.TileIndexProperties.TryGetValue("Passable", out var value) && value != null)
                return true;
            if (self.objects.ContainsKey(new Vector2(x, y)))
                return false;
            if (self.isObjectAt(x, y))
                self.getObjectAt(x, y).isPassable();
            if (self.isTerrainFeatureAt(x, y))
            {
                var rectangle = new Rectangle(x * Game1.tileSize, y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                foreach (var largeTerrainFeature in self.largeTerrainFeatures)
                    if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
                        return largeTerrainFeature.isPassable();
            }
            if (self.terrainFeatures.ContainsKey(new Vector2(x, y)))
                return self.terrainFeatures[new Vector2(x, y)].isPassable();
            if (tile.TileIndexProperties.TryGetValue("Water", out value) && value != null)
                return false;
            tile = self.map.GetLayer("Buildings").Tiles[x, y];
            if (tile != null && tile.TileIndexProperties.TryGetValue("Passable", out value) && value != null && tile.TileIndexProperties.TryGetValue("Shadow", out value) && value != null)
                return true;
            return !self.isTileOccupied(new Vector2(x, y));
        }
        public static bool IsWater(this GameLocation self, int x, int y)
        {
            var layer = self.map.GetLayer("Back");
            if (layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            var tile = layer.Tiles[x, y];
            tile.TileIndexProperties.TryGetValue("Water", out var value);
            return value != null;
        }
    }
}
