using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader.Locations
{
    [XmlType("ALLDesert")]
    public class Desert : StardewValley.Locations.Desert
    {
        /*********
        ** Fields
        *********/
        private static bool RememberRain;
        private static bool RememberDebris;
        private static bool RememberLightning;
        private static bool RememberSnow;
        private static readonly MethodInfo Reset = typeof(GameLocation).GetMethod("resetForPlayerEntry", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo Cleanup = typeof(GameLocation).GetMethod("cleanupBeforePlayerExit", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo Update = typeof(GameLocation).GetMethod("UpdateWhenCurrentLocation", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo Drawer = typeof(GameLocation).GetMethod("draw", BindingFlags.Public | BindingFlags.Instance);


        /*********
        ** Public methods
        *********/
        public Desert() { }

        public Desert(string mapPath, string name)
            : base(mapPath, name) { }

        public override void draw(SpriteBatch spriteBatch)
        {
            Desert.Drawer.Invoke(this, new object[] { spriteBatch });
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            Desert.Update.Invoke(this, new object[] { time });
        }

        public override void cleanupBeforePlayerExit()
        {
            Desert.Cleanup.Invoke(this, null);
            Game1.isRaining = Desert.RememberRain;
            Game1.isDebrisWeather = Desert.RememberDebris;
            Game1.isLightning = Desert.RememberLightning;
            Game1.isSnowing = Desert.RememberSnow;
        }


        /*********
        ** Protected methods
        *********/
        protected override void resetLocalState()
        {
            Desert.Reset.Invoke(this, null);
            Desert.RememberRain = Game1.isRaining;
            Desert.RememberDebris = Game1.isDebrisWeather;
            Desert.RememberLightning = Game1.isLightning;
            Desert.RememberSnow = Game1.isSnowing;
            Game1.isRaining = false;
            Game1.isDebrisWeather = false;
            Game1.isLightning = false;
            Game1.isSnowing = false;
            Game1.ambientLight = Color.White;
        }
    }
}
