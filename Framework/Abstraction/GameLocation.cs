using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.Framework.Abstraction
{
    public class GameLocation
    {
        protected StardewValley.GameLocation Location;
        internal GameLocation(StardewValley.GameLocation location)
        {
            Location = location;
        }
        internal static GameLocation Wrap(StardewValley.GameLocation location)
        {
            return new GameLocation(location);
        }
        public bool HasTile(int x, int y, string layer)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                return false;
            if (x < 0 || x >= _layer.LayerWidth)
                return false;
            if (y < 0 || y >= _layer.LayerHeight)
                return false;
            if (_layer.Tiles[x, y] == null)
                return false;
            return true;
        }
        public void RemoveTile(int x, int y, string layer)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (x < 0 || x >= _layer.LayerWidth)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= _layer.LayerHeight)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (_layer.Tiles[x, y] == null)
                throw new ArgumentException("Tile does not exist");
            _layer.Tiles[x, y] = null;
        }
        public bool TryRemoveTile(int x, int y, string layer)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                return false;
            if (x < 0 || x >= _layer.LayerWidth)
                return false;
            if (y < 0 || y >= _layer.LayerHeight)
                return false;
            if (_layer.Tiles[x, y] == null)
                return false;
            _layer.Tiles[x, y] = null;
            return true;
        }
        public void SetTile(int x, int y, string layer, int index, string sheet = null)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (x < 0 || x >= _layer.LayerWidth)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= _layer.LayerHeight)
                throw new ArgumentOutOfRangeException(nameof(y));
            TileSheet _sheet;
            if (sheet != null)
            {
                _sheet = Location.map.GetTileSheet(sheet);
                if (_sheet == null)
                    throw new ArgumentNullException(nameof(sheet));
            }
            else
            {
                Tile _tile = _layer.Tiles[x, y];
                if (_tile == null)
                    _sheet = Location.map.TileSheets[0];
                else
                    _sheet = _tile.TileSheet;
            }
            if (index < 0 || _sheet.TileCount <= index)
                throw new ArgumentOutOfRangeException(nameof(index));
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
        }
        public void SetTile(int x, int y, string layer, int[] indexes, long interval, string sheet = null)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException(nameof(layer));
            if (x < 0 || x >= _layer.LayerWidth)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= _layer.LayerHeight)
                throw new ArgumentOutOfRangeException(nameof(y));
            TileSheet _sheet;
            if (sheet != null)
            {
                _sheet = Location.map.GetTileSheet(sheet);
                if (_sheet == null)
                    throw new ArgumentNullException(nameof(sheet));
            }
            else
            {
                Tile _tile = _layer.Tiles[x, y];
                if (_tile == null)
                    _sheet = Location.map.TileSheets[0];
                else
                    _sheet = _tile.TileSheet;
            }
            List<StaticTile> _frames = new List<StaticTile>();
            for (var c = 0; c < indexes.Length; c++)
                if (indexes[c] < 0 || _sheet.TileCount <= indexes[c])
                    throw new ArgumentOutOfRangeException(nameof(indexes) + '[' + c + ']');
                else
                    _frames.Add(new StaticTile(_layer, _sheet, BlendMode.Alpha, indexes[c]));
            _layer.Tiles[x, y] = new AnimatedTile(_layer, _frames.ToArray(), interval);
        }
        public bool TrySetTile(int x, int y, string layer, int index, string sheet = null)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                return false;
            if (x < 0 || x >= _layer.LayerWidth)
                return false;
            if (y < 0 || y >= _layer.LayerHeight)
                return false;
            TileSheet _sheet;
            if (sheet != null)
            {
                _sheet = Location.map.GetTileSheet(sheet);
                if (_sheet == null)
                    return false;
            }
            else
            {
                Tile _tile = _layer.Tiles[x, y];
                if (_tile == null)
                    _sheet = Location.map.TileSheets[0];
                else
                    _sheet = _tile.TileSheet;
            }
            if (index < 0 || _sheet.TileCount <= index)
                return false;
            _layer.Tiles[x, y] = new StaticTile(_layer, _sheet, BlendMode.Alpha, index);
            return true;
        }
        public bool TrySetTile(int x, int y, string layer, int[] indexes, long interval, string sheet = null)
        {
            Layer _layer = Location.map.GetLayer(layer);
            if (_layer == null)
                return false;
            if (x < 0 || x >= _layer.LayerWidth)
                return false;
            if (y < 0 || y >= _layer.LayerHeight)
                return false;
            TileSheet _sheet;
            if (sheet != null)
            {
                _sheet = Location.map.GetTileSheet(sheet);
                if (_sheet == null)
                    return false;
            }
            else
            {
                Tile _tile = _layer.Tiles[x, y];
                if (_tile == null)
                    _sheet = Location.map.TileSheets[0];
                else
                    _sheet = _tile.TileSheet;
            }
            List<StaticTile> _frames = new List<StaticTile>();
            for (var c = 0; c < indexes.Length; c++)
                if (indexes[c] < 0 || _sheet.TileCount <= indexes[c])
                    return false;
                else
                    _frames.Add(new StaticTile(_layer, _sheet, BlendMode.Alpha, indexes[c]));
            _layer.Tiles[x, y] = new AnimatedTile(_layer, _frames.ToArray(), interval);
            return true;
        }
    }
}
