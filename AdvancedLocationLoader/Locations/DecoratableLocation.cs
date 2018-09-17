using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLDecoratableLocation")]
    public class DecoratableLocation : StardewValley.Locations.DecoratableLocation
    {
        /*********
        ** Fields
        *********/
        private static readonly MethodInfo Reset = typeof(GameLocation).GetMethod("resetForPlayerEntry", BindingFlags.Instance | BindingFlags.Public);


        /*********
        ** Public methods
        *********/
        public DecoratableLocation() { }

        public DecoratableLocation(string mapPath, string name)
            : base(mapPath, name) { }


        public override void cleanupBeforePlayerExit()
        {
            base.cleanupBeforePlayerExit();
            for (int c = 0; c < Game1.player.items.Count; c++)
            {
                if (Game1.player.items[c] is FakeWallpaper)
                    Game1.player.items[c] = ((FakeWallpaper)Game1.player.items[c]).Restore();
            }
        }

        public override void setFloor(int which, int whichRoom = -1, bool persist = false)
        {
        }

        protected override void doSetVisibleWallpaper(int whichRoom, int which)
        {
        }

        /*********
        ** Protected methods
        *********/
        protected override void resetLocalState()
        {
            DecoratableLocation.Reset.Invoke(this, null);
            foreach (Furniture furniture in this.furniture)
                furniture.resetOnPlayerEntry(this);
            for (int c = 0; c < Game1.player.items.Count; c++)
            {
                if (Game1.player.items[c] is Wallpaper)
                    Game1.player.items[c] = new FakeWallpaper((Wallpaper)Game1.player.items[c]);
            }
        }

        private class FakeWallpaper : Wallpaper
        {
            public FakeWallpaper(Wallpaper item)
            {
                this.isFloor.Value = item.isFloor;
                this.parentSheetIndex.Value = item.parentSheetIndex;
                this.name = this.isFloor ? "Flooring" : "Wallpaper";
                this.price.Value = 100;
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
                return new Wallpaper(this.parentSheetIndex, this.isFloor);
            }
        }
    }
}
