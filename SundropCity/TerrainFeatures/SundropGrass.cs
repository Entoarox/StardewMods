using Microsoft.Xna.Framework;

using StardewValley;

namespace SundropCity.TerrainFeatures
{
    class SundropGrass : StardewValley.TerrainFeatures.Grass
    {
        public SundropGrass() : base()
        {
            this.numberOfWeeds.Value = 4;
            this.loadSprite();
            this.grassSourceOffset.Value = 0;
        }
        protected override string textureName()
        {
            return SundropCityMod.SHelper.Content.GetActualAssetKey("assets/TerrainFeatures/Grass.png");
        }
        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
            // Prevent growth like vanilla grass has.
        }
        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            if (location == null)
                location = Game1.currentLocation;
            return false;
        }
    }
}
