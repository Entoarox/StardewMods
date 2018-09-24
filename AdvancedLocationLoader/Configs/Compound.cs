using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Compound
    {
        /*********
        ** Accessors
        *********/
        public IDictionary<IContentPack, Tilesheet[]> SeasonalTilesheets { get; }
        public Tile[] DynamicTiles { get; }
        public Property[] DynamicProperties { get; }
        public Warp[] DynamicWarps { get; }
        public Conditional[] Conditionals { get; }
        public TeleporterList[] Teleporters { get; }
        public ShopConfig[] Shops { get; }


        /*********
        ** Public methods
        *********/
        public Compound(IDictionary<IContentPack, Tilesheet[]> seasonalTilesheets, IEnumerable<Tile> dynamicTiles, IEnumerable<Property> dynamicProperties, IEnumerable<Warp> dynamicWarps, IEnumerable<Conditional> conditionals, IEnumerable<TeleporterList> teleporters, IEnumerable<ShopConfig> shops)
        {
            this.SeasonalTilesheets = seasonalTilesheets;
            this.DynamicTiles = dynamicTiles.ToArray();
            this.DynamicProperties = dynamicProperties.ToArray();
            this.DynamicWarps = dynamicWarps.ToArray();
            this.Conditionals = conditionals.ToArray();
            this.Teleporters = teleporters.ToArray();
            this.Shops = shops.ToArray();
        }
    }
}
