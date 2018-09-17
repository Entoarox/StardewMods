using System;
using xTile;

namespace Entoarox.DynamicDungeons.Tiles
{
    internal abstract class Tile
    {
        /*********
        ** Fields
        *********/
        protected string Layer;
        protected int X;
        protected int Y;


        /*********
        ** Public methods
        *********/
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

        public abstract void Apply(int x, int y, Map map);


        /*********
        ** Protected methods
        *********/
        protected Tile(int x, int y, string layer)
        {
            this.X = x >= 0 ? x : throw new ArgumentOutOfRangeException(nameof(x));
            this.Y = y >= 0 ? y : throw new ArgumentOutOfRangeException(nameof(y));
            this.Layer = layer ?? throw new ArgumentNullException(nameof(layer));
        }
    }
}
