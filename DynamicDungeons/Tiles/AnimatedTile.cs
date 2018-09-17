using System.Linq;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace Entoarox.DynamicDungeons.Tiles
{
    internal class AnimatedTile : Tile
    {
        /*********
        ** Fields
        *********/
        private readonly (string sheet, int index)[] Frames;
        private readonly int Interval;


        /*********
        ** Public methods
        *********/
        public AnimatedTile(int x, int y, string layer, int[] frames, string sheet, int interval)
            : this(x, y, layer, frames.Select(a => (sheet, a)).ToArray(), interval) { }

        public AnimatedTile(int x, int y, string layer, (string sheet, int index)[] frames, int interval)
            : base(x, y, layer)
        {
            this.Frames = frames;
            this.Interval = interval;
        }

        public override void Apply(int x, int y, Map map)
        {
            Layer layer = map.GetLayer(this.Layer);
            layer.Tiles[this.X + x, this.Y + y] = new xTile.Tiles.AnimatedTile(layer, this.Frames.Select(a => new xTile.Tiles.StaticTile(layer, map.GetTileSheet(a.sheet), BlendMode.Additive, a.index)).ToArray(), this.Interval);
        }
    }
}
