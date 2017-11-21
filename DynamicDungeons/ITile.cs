using xTile.Layers;
using xTile.Tiles;

namespace DynamicDungeons
{
    public interface ITile
    {
        Tile Get();
        int X { get; set; }
        int Y { get; set; }
        Layer Layer { get; set; }
    }
}
