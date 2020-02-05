using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using SObject = StardewValley.Object;

namespace SundropCity.Hotel
{
    class HotelItem : SObject
    {
        internal static Texture2D SpriteSheet;
        public HotelItem(int id, int amount) : base(id, amount)
        {
            if (SpriteSheet == null)
                SpriteSheet = SundropCityMod.SHelper.Content.Load<Texture2D>("assets/MiscSprites/SundropHotelIcons.png");
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), Game1.shadowTexture.Bounds, color * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(SpriteSheet, location + new Vector2((float)(int)(32f * scaleSize), (int)(32f * scaleSize)), Game1.getSourceRectForStandardTileSheet(SpriteSheet, this.ParentSheetIndex, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
            if(this.Stack>1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((64 - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);
        }
    }
}
