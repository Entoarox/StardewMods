using System.Reflection;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Objects;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    class DecoratableLocation : StardewValley.Locations.DecoratableLocation
    {
        private class FakeWallpaper : Wallpaper
        {
            public FakeWallpaper(Wallpaper item)
            {
                isFloor = item.isFloor;
                parentSheetIndex = item.parentSheetIndex;
                name = isFloor ? "Flooring" : "Wallpaper";
                price = 100;
            }
            public override bool canBePlacedHere(GameLocation l, Vector2 tile)
            {
                return false;
            }
            public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
            {
                return false;
            }
            public Wallpaper Restore()
            {
                return new Wallpaper(parentSheetIndex, isFloor);
            }
        }
        private static MethodInfo reset = typeof(GameLocation).GetMethod("resetForPlayerEntry", BindingFlags.Instance | BindingFlags.Public);
        public DecoratableLocation()
        {

        }
        public DecoratableLocation(xTile.Map map, string name) : base(map,name)
        {

        }
        public override void resetForPlayerEntry()
        {
            reset.Invoke(this, null);
            foreach (Furniture furniture in furniture)
                furniture.resetOnPlayerEntry(this);
            for (int c=0;c<Game1.player.items.Count;c++)
            {
                Item i = Game1.player.items[c];
                if (Game1.player.items[c] is Wallpaper)
                    Game1.player.items[c] = new FakeWallpaper((Wallpaper)Game1.player.items[c]);
            }
        }
        public override void cleanupBeforePlayerExit()
        {
            base.cleanupBeforePlayerExit();
            for (int c = 0; c < Game1.player.items.Count; c++)
            {
                Item i = Game1.player.items[c];
                if (Game1.player.items[c] is FakeWallpaper)
                    Game1.player.items[c] = ((FakeWallpaper)Game1.player.items[c]).Restore();
            }
        }
        public override void setFloor(int which, int whichRoom = -1, bool persist = false)
        {
            return;
        }
        public override void setWallpaper(int which, int whichRoom = -1, bool persist = false)
        {
            return;
        }
    }
}
