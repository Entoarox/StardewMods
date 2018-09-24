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
        public abstract void Apply(int x, int y, Map map);

        public void Apply(Map map)
        {
            this.Apply(0, 0, map);
        }


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
