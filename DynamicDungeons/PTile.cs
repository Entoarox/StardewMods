using System;

using xTile.Layers;
using xTile.Tiles;

namespace DynamicDungeons
{
    public struct PTile : ITile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }
        public string Key;
        public string Value;
        public PTile(int x, int y, Layer layer, string key, string value)
        {
            this.X = x;
            this.Y = y;
            this.Layer = layer;
            this.Key = key;
            this.Value = value;
        }
        public Tile Get()
        {
            throw new NotImplementedException();
        }
    }
}
