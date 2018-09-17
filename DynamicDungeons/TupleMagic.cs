using Entoarox.DynamicDungeons.Tiles;
using xTile;

namespace Entoarox.DynamicDungeons
{
    internal static class TupleMagic
    {
        /*********
        ** Public methods
        *********/
        public static void ApplyTo(this (int x, int y, string layer, int index, string sheet) tuple, Map map)
        {
            ((Tile)tuple).Apply(0, 0, map);
        }

        public static void ApplyTo(this (int x, int y, string layer, int[] layers, string sheet, int interval) tuple, Map map)
        {
            ((Tile)tuple).Apply(0, 0, map);
        }

        public static void ApplyTo(this (int x, int y, string layer, (string sheet, int index)[] layers, int interval) tuple, Map map)
        {
            ((Tile)tuple).Apply(0, 0, map);
        }

        public static void ApplyTo(this (int x, int y, string layer, string key, string value) tuple, Map map)
        {
            ((Tile)tuple).Apply(0, 0, map);
        }
    }
}
