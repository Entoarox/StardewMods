using xTile.Layers;

namespace Entoarox.DynamicDungeons
{
    internal interface ITile
    {
        /*********
        ** Accessors
        *********/
        int X { get; set; }
        int Y { get; set; }
        Layer Layer { get; set; }


        /*********
        ** Methods
        *********/
        xTile.Tiles.Tile Get();
    }
}
