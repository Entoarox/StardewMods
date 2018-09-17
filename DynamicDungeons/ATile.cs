using System.Linq;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons
{
    internal struct ATile : ITile
    {
        /*********
        ** Accessors
        *********/
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }
        public STile[] Frames;
        public int Interval;


        /*********
        ** Public methods
        *********/
        public ATile(int x, int y, Layer layer, STile[] frames, int interval)
        {
            this.X = x;
            this.Y = y;
            this.Layer = layer;
            this.Frames = frames;
            this.Interval = interval;
        }

        public Tile Get()
        {
            return new AnimatedTile(this.Layer, this.Frames.Select(tile => (StaticTile)tile.Get()).ToArray(), this.Interval);
        }
    }
}
