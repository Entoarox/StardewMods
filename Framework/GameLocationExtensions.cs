using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Entoarox.Framework
{
    public static class GameLocationExtensions
    {
        /*********
        ** Public methods
        *********/
        public static bool HasTile(this GameLocation self, int x, int y, string layer)
        {
            Layer _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            return _layer.Tiles[x, y] != null;
        }

        public static void SetTile(this GameLocation self, int x, int y, string layer, int index, string sheet = null)
        {
            Layer _layer = self.map.GetLayer(layer);
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
            if (_sheet.TileCount < index || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), $"Must be at least 0 and less then {_sheet.TileCount} for tilesheet with Id `{_sheet.Id}`");
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
        }

        public static void SetTile(this GameLocation self, int x, int y, string layer, int[] indexes, int interval, string sheet = null)
        {
            if (interval < 0)
                throw new ArgumentOutOfRangeException(nameof(interval));
            Layer _layer = self.map.GetLayer(layer);
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
                    throw new ArgumentOutOfRangeException($"{nameof(indexes)}[{c}]", $"Must be at least 0 and less then {_sheet.TileCount} for tilesheet with Id `{_sheet.Id}`");
            List<StaticTile> Frames = new List<StaticTile>();
            foreach (int index in indexes)
                Frames.Add(new StaticTile(_layer, _sheet, BlendMode.Alpha, index));
            _layer.Tiles[x, y] = new AnimatedTile(_layer, Frames.ToArray(), interval);
        }

        public static void RemoveTile(this GameLocation self, int x, int y, string layer)
        {
            Layer _layer = self.map.GetLayer(layer);
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
            TileSheet _sheet = sheet == null ? self.map.TileSheets[0] : self.map.GetTileSheet(sheet);
            if (_sheet == null || _sheet.TileCount < index || index < 0)
                return false;
            Layer _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
            return true;
        }

        public static bool TrySetTile(this GameLocation self, int x, int y, string layer, int[] indexes, int interval, string sheet = null)
        {
            TileSheet _sheet = sheet == null ? self.map.TileSheets[0] : self.map.GetTileSheet(sheet);
            if (_sheet == null || interval < 0)
                return false;
            for (int c = 0; c < indexes.Length; c++)
                if (_sheet.TileCount < indexes[c] || indexes[c] < 0)
                    return false;
            Layer _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            List<StaticTile> Frames = new List<StaticTile>();
            foreach (int index in indexes)
                Frames.Add(new StaticTile(_layer, _sheet, BlendMode.Alpha, index));
            _layer.Tiles[x, y] = new AnimatedTile(_layer, Frames.ToArray(), interval);
            return true;
        }

        public static bool TryRemoveTile(this GameLocation self, int x, int y, string layer)
        {
            Layer _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0)
                return false;
            _layer.Tiles[x, y] = null;
            return true;
        }

        public static void SetTileProperty(this GameLocation self, int x, int y, string layer, string key, string value)
        {
            Layer _layer = self.map.GetLayer(layer);
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
            Layer _layer = self.map.GetLayer(layer);
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
            Layer _layer = self.map.GetLayer(layer);
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
            Layer _layer = self.map.GetLayer(layer);
            if (_layer == null || _layer.LayerWidth < x || x < 0 || _layer.LayerHeight < y || y < 0 || !_layer.Tiles[x, y].Properties.ContainsKey(key))
                return false;
            value = _layer.Tiles[x, y].Properties[key];
            return true;
        }

        public static void AddWarp(this GameLocation self, int x, int y, string target, int targetX, int targetY, bool replace = true)
        {
            if (!replace && self.warps.Any(a => a.X == x && a.Y == y))
                throw new ArgumentException($"Index already set {x},{y}");
            else
                self.warps.Filter(a => a.X != x || a.Y != y);
            self.warps.Add(new Warp(x, y, target, x, y, false));
        }

        public static void RemoveWarp(this GameLocation self, int x, int y)
        {
            self.warps.Filter(a => a.X != x || a.Y != y);
        }

        public static bool IsPassable(this GameLocation self, int x, int y)
        {
            Layer layer = self.map.GetLayer("Back");
            if (layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            Tile tile = layer.Tiles[x, y];
            if (tile.TileIndexProperties.TryGetValue("Passable", out PropertyValue value) && value != null)
                return true;
            if (self.objects.ContainsKey(new Vector2(x, y)))
                return false;
            if (self.isObjectAt(x, y))
                self.getObjectAt(x, y).isPassable();
            if (self.isTerrainFeatureAt(x, y))
            {
                Rectangle rectangle = new Rectangle(x * Game1.tileSize, y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                foreach (LargeTerrainFeature largeTerrainFeature in self.largeTerrainFeatures)
                {
                    if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
                        return largeTerrainFeature.isPassable();
                }
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
            Layer layer = self.map.GetLayer("Back");
            if (layer.LayerWidth < x || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (layer.LayerHeight < y || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            Tile tile = layer.Tiles[x, y];
            tile.TileIndexProperties.TryGetValue("Water", out PropertyValue value);
            return value != null;
        }
    }
}
