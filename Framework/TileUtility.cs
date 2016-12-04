using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StardewValley;

namespace Entoarox.Framework
{
    public struct ReferencedLocation
    {
        public readonly GameLocation Location;
        private ReferencedLocation(GameLocation location)
        {
            Location = location;
            if (Location == null)
                throw new ArgumentNullException("location");
        }
        private ReferencedLocation(string location) : this(Game1.getLocationFromName(location))
        {
        }
        public static implicit operator ReferencedLocation(GameLocation location)
        {
            return new ReferencedLocation(location);
        }
        public static implicit operator ReferencedLocation(string location)
        {
            return new ReferencedLocation(location);
        }
        public static implicit operator GameLocation(ReferencedLocation reference)
        {
            return reference.Location;
        }
    }
    public struct ReferencedTile
    {
        public readonly GameLocation Location;
        public readonly string Layer;
        public readonly Point Position;
        public ReferencedTile(ReferencedLocation location, string layer, Point position)
        {
            Location = location.Location;
            Layer = layer;
            Position = position;
            // Error Resolve
            xTile.Layers.Layer _layer = location.Location.map.GetLayer(Layer);
            if (_layer == null)
                throw new ArgumentNullException("layer");
            if (Position.X < 0 || Position.X > _layer.LayerWidth)
                throw new ArgumentOutOfRangeException("x");
            if (Position.Y < 0 || Position.Y > _layer.LayerHeight)
                throw new ArgumentOutOfRangeException("y");
        }
        public ReferencedTile(ReferencedLocation location, string layer, int x, int y)
        {
            Location = location.Location;
            Layer = layer;
            Position = new Point(x,y);
            // Error resolve
            xTile.Layers.Layer _layer = location.Location.map.GetLayer(Layer);
            if (_layer == null)
                throw new ArgumentNullException("layer");
            if (Position.X < 0 || Position.X > _layer.LayerWidth)
                throw new ArgumentOutOfRangeException("x");
            if (Position.Y < 0 || Position.Y > _layer.LayerHeight)
                throw new ArgumentOutOfRangeException("y");
        }
    }
    public static class TileUtility
    {
        public static void SetTile(ReferencedTile tile, int index, string sheet=null)
        {
            SetTile(tile.Location, tile.Layer, tile.Position.X, tile.Position.Y, index, sheet);
        }
        public static void SetTile(ReferencedLocation location, string layer, Point position, int index, string sheet=null)
        {
            SetTile(location.Location, layer, position.X, position.Y, index, sheet);
        }
        public static void SetTile(ReferencedLocation location, string layer, int x, int y, int index, string sheet=null)
        {
            xTile.Layers.Layer _layer=location.Location.map.GetLayer(layer);
            if (_layer == null)
                throw new ArgumentNullException("layer");
            if (x < 0 || x > _layer.LayerWidth)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y > _layer.LayerHeight)
                throw new ArgumentOutOfRangeException("y");
        }
    }
}
