using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public static class Stardew
    {
        public static WrappedContentManager ContentManager => new WrappedContentManager(StardewValley.Game1.content);
        public static Farmer Player => StardewValley.Game1.player == null ? null : new Farmer(StardewValley.Game1.player);
        public static LocationList Locations => new LocationList();
        public static GameLocation GetLocationFromName(string name)
        {
            StardewValley.GameLocation loc = StardewValley.Game1.getLocationFromName(name);
            if (loc == null)
                throw new ArgumentNullException(nameof(name));
            return GameLocation.Wrap(loc);
        }
        public static bool TryGetLocationFromName(string name, out GameLocation location)
        {
            location = default(GameLocation);
            StardewValley.GameLocation loc = StardewValley.Game1.getLocationFromName(name);
            if (loc == null)
                return false;
            location = GameLocation.Wrap(loc);
            return true;
        }
    }
}
