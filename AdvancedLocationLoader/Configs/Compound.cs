using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal static class Compound
    {
        internal static List<Tile> DynamicTiles;
        internal static List<Property> DynamicProperties;
        internal static List<Warp> DynamicWarps;
        internal static List<Tilesheet> SeasonalTilesheets;
        internal static Dictionary<string, ShopConfig> Shops;
        internal static List<TeleporterList> Teleporters;
        internal static List<Conditional> Conditionals;

        static Compound()
        {
            DynamicTiles = new List<Tile>();
            DynamicProperties = new List<Property>();
            DynamicWarps = new List<Warp>();
            SeasonalTilesheets = new List<Tilesheet>();
            Shops = new Dictionary<string, ShopConfig>();
            Teleporters = new List<TeleporterList>();
            Conditionals = new List<Conditional>();
        }
    }
}