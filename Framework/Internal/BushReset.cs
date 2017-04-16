using Microsoft.Xna.Framework;

using xTile.Tiles;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace Entoarox.Framework.Internal
{
    internal static class BushReset
    {
        internal static void Trigger(string name, string[] args)
        {
            foreach(GameLocation loc in Game1.locations)
            {
                loc.largeTerrainFeatures = loc.largeTerrainFeatures.FindAll(a => !(a is Bush));
                if ((loc.isOutdoors || loc.name.Equals("BathHouse_Entry") || loc.treatAsOutdoors) && loc.map.GetLayer("Paths") != null)
                {
                    for (int x = 0; x < loc.map.Layers[0].LayerWidth; ++x)
                    {
                        for (int y = 0; y < loc.map.Layers[0].LayerHeight; ++y)
                        {
                            Tile tile = loc.map.GetLayer("Paths").Tiles[x, y];
                            if (tile != null)
                            {
                                Vector2 vector2 = new Vector2(x, y);
                                switch (tile.TileIndex)
                                {
                                    case 24:
                                        if (!loc.terrainFeatures.ContainsKey(vector2))
                                            loc.largeTerrainFeatures.Add(new Bush(vector2, 2, loc));
                                        break;
                                    case 25:
                                        if (!loc.terrainFeatures.ContainsKey(vector2))
                                            loc.largeTerrainFeatures.Add(new Bush(vector2, 1, loc));
                                        break;
                                    case 26:
                                        if (!loc.terrainFeatures.ContainsKey(vector2))
                                            loc.largeTerrainFeatures.Add(new Bush(vector2, 0, loc));
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
