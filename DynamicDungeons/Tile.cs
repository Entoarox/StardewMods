using System;
using System.Linq;

using xTile;

namespace Entoarox.DynamicDungeons
{
    public abstract class Tile
    {
        private class StaticTile : Tile
        {
            private int Index;
            private string Sheet;
            public StaticTile(int x, int y, string layer, int index, string sheet) : base(x, y, layer)
            {
                this.Index = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(index));
                this.Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            }
            internal override void Apply(int x, int y, Map map)
            {
                var layer = map.GetLayer(this.Layer);
                layer.Tiles[this.X+x, this.Y+y] = new xTile.Tiles.StaticTile(layer, map.GetTileSheet(this.Sheet), xTile.Tiles.BlendMode.Additive, this.Index);
            }
        }
        private class AnimatedTile : Tile
        {
            private (string sheet, int index)[] Frames;
            private int Interval;
            public AnimatedTile(int x, int y, string layer, int[] frames, string sheet, int interval) : this(x, y, layer, frames.Select(a => (sheet, a)).ToArray(), interval)
            {

            }
            public AnimatedTile(int x, int y, string layer, (string sheet, int index)[] frames, int interval) : base(x, y, layer)
            {
                this.Frames = frames;
                this.Interval = interval;
            }
            internal override void Apply(int x, int y, Map map)
            {
                var layer = map.GetLayer(this.Layer);
                layer.Tiles[this.X+x, this.Y+y] = new xTile.Tiles.AnimatedTile(layer, this.Frames.Select(a => new xTile.Tiles.StaticTile(layer, map.GetTileSheet(a.sheet), xTile.Tiles.BlendMode.Additive, a.index)).ToArray(), this.Interval);
            }
        }
        private class PropertyTile : Tile
        {
            private string Key;
            private string Value;
            public PropertyTile(int x, int y, string layer, string key, string value) : base(x, y, layer)
            {
                this.Key = key;
                this.Value = value;
            }
            internal override void Apply(int x, int y, Map map)
            {
                map.GetLayer(this.Layer).Tiles[this.X+x, this.Y+y]?.Properties.Add(this.Key, this.Value);
            }
        }
        protected int X;
        protected int Y;
        protected string Layer;
        internal Tile(int x, int y, string layer)
        {
            this.X = x >= 0 ? x : throw new ArgumentOutOfRangeException(nameof(x));
            this.Y = y >= 0 ? y : throw new ArgumentOutOfRangeException(nameof(y));
            this.Layer = layer ?? throw new ArgumentNullException(nameof(layer));
        }
        internal abstract void Apply(int x, int y, Map map);

        public static implicit operator Tile((int x, int y, string layer, int index, string sheet) tuple)
        {
            return new StaticTile(tuple.x, tuple.y, tuple.layer, tuple.index, tuple.sheet);
        }
        public static implicit operator Tile((int x, int y, string layer, int[] frames, string sheet, int interval) tuple)
        {
            return new AnimatedTile(tuple.x, tuple.y, tuple.layer, tuple.frames, tuple.sheet, tuple.interval);
        }
        public static implicit operator Tile((int x, int y, string layer, (string sheet, int index)[] frames, int interval) tuple)
        {
            return new AnimatedTile(tuple.x, tuple.y, tuple.layer, tuple.frames, tuple.interval);
        }
        public static implicit operator Tile((int x, int y, string layer, string key, string value) tuple)
        {
            return new PropertyTile(tuple.x, tuple.y, tuple.layer, tuple.key, tuple.value);
        }
    }
}
