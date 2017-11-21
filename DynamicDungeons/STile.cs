using xTile.Layers;
using xTile.Tiles;

namespace DynamicDungeons
{
    public struct STile : ITile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }
        public TileSheet Sheet;
        public int Index;
        public STile(int x, int y, Layer layer, TileSheet sheet, int index)
        {
            this.X = x;
            this.Y = y;
            this.Layer = layer;
            this.Sheet = sheet;
            this.Index = index;
        }
        public Tile Get()
        {
            return new StaticTile(this.Layer, this.Sheet, BlendMode.Additive, this.Index);
        }
    }
}
