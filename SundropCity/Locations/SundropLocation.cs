using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.TerrainFeatures;

using SundropCity.TerrainFeatures;

namespace SundropCity
{
    class SundropLocation : GameLocation
    {
        // Serializer constructor
        public SundropLocation() : base()
        {

        }
        public SundropLocation(string map, string name) : base(map, name)
        {
            if (this.map.GetLayer("SundropPaths") != null)
            {
                var layer = this.map.GetLayer("SundropPaths");
                for (int x = 0; x < this.map.DisplayWidth; x++)
                    for (int y = 0; y < this.map.DisplayHeight; y++)
                    {
                        var tile = layer.Tiles[x, y];
                        if (tile == null)
                            continue;
                        Vector2 vect = new Vector2(x, y);
                        switch(tile.TileIndex)
                        {
                            case 0:
                                if (!this.terrainFeatures.ContainsKey(vect))
                                    this.terrainFeatures.Add(vect, new SundropGrass());
                                break;
                            case 1:
                                if (!this.terrainFeatures.ContainsKey(vect))
                                    this.terrainFeatures.Add(vect, new Tree(456,5));
                                break;
                        }
                    }
            }
        }
    }
}
