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
        private static bool rememberRain;
        private static bool rememberDebris;
        private static bool rememberLightning;
        private static bool rememberSnow;
        private static MethodInfo reset= typeof(GameLocation).GetMethod("resetForPlayerEntry", BindingFlags.Public | BindingFlags.Instance);
        private static MethodInfo cleanup = typeof(GameLocation).GetMethod("cleanupBeforePlayerExit", BindingFlags.Public | BindingFlags.Instance);
        private static MethodInfo update = typeof(GameLocation).GetMethod("UpdateWhenCurrentLocation", BindingFlags.Public | BindingFlags.Instance);
        private static MethodInfo drawer = typeof(GameLocation).GetMethod("draw", BindingFlags.Public | BindingFlags.Instance);
        public Desert()
        {
        }
        public Desert(string mapPath, string name)
            : base(mapPath, name)
        {
        }
        public override void draw(SpriteBatch spriteBatch)
        {
            drawer.Invoke(this, new object[] { spriteBatch });
        }
        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            update.Invoke(this, new object[] { time });
        }

        protected override void resetLocalState()
        {
            reset.Invoke(this, null);
            rememberRain = Game1.isRaining;
            rememberDebris = Game1.isDebrisWeather;
            rememberLightning = Game1.isLightning;
            rememberSnow = Game1.isSnowing;
            Game1.isRaining = false;
            Game1.isDebrisWeather = false;
            Game1.isLightning = false;
            Game1.isSnowing = false;
            Game1.ambientLight = Color.White;
        }

        public override void cleanupBeforePlayerExit()
        {
            cleanup.Invoke(this,null);
            Game1.isRaining = rememberRain;
            Game1.isDebrisWeather = rememberDebris;
            Game1.isLightning = rememberLightning;
            Game1.isSnowing = rememberSnow;
        }
    }
}
