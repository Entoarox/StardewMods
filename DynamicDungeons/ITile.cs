using xTile.Layers;

namespace Entoarox.DynamicDungeons
{
    public interface ITile
    {
        xTile.Tiles.Tile Get();
        int X { get; set; }
        int Y { get; set; }
        Layer Layer { get; set; }
    }
}
