using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.DynamicDungeons
{
    class PaymentPage : Page
    {
        private static Rectangle ItemBox = new Rectangle(293, 360, 24, 24);
        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            string page2 = Game1.parseText(DynamicDungeonsMod.SHelper.Translation.Get("Book_Page3"), Game1.smallFont, region.Width);
            string diffEasy = DynamicDungeonsMod.SHelper.Translation.Get("Difficulty_Easy");
            float diffEasyHeight = Game1.smallFont.MeasureString(diffEasy).Y;
            float offset = Game1.smallFont.MeasureString(page2).Y + 20;
            Utility.drawTextWithShadow(batch, page2, Game1.smallFont, new Vector2(region.X, region.Y), Game1.textColor);

            const float boxscale = 3.1f;

            var item = new StardewValley.Object(382, 1);
            var pos = new Vector2(region.X, region.Y + offset);
            batch.Draw(Game1.mouseCursors, pos, ItemBox, Color.White, 0, Vector2.Zero, boxscale, SpriteEffects.None, 0);
            pos.X += 6;
            pos.Y += 6;
            item.drawInMenu(batch, pos, 1, 1, 0, false);
            Utility.drawTextWithShadow(batch, diffEasy, Game1.smallFont, new Vector2(region.X + ItemBox.Width * boxscale + 4, region.Y + offset + (ItemBox.Height * boxscale - diffEasyHeight) / 2), Game1.textColor);

            offset += ItemBox.Height * boxscale + 4;
            item = new StardewValley.Object(72, 1);
            pos = new Vector2(region.X, region.Y + offset);
            batch.Draw(Game1.mouseCursors, pos, ItemBox, Color.White, 0, Vector2.Zero, boxscale, SpriteEffects.None, 0);
            pos.X += 6;
            pos.Y += 6;
            item.drawInMenu(batch, pos, 1, 1, 0, false);
            Utility.drawTextWithShadow(batch, DynamicDungeonsMod.SHelper.Translation.Get("Difficulty_Medium"), Game1.smallFont, new Vector2(region.X + ItemBox.Width * boxscale + 4, region.Y + offset + (ItemBox.Height * boxscale - diffEasyHeight) / 2), Game1.textColor);

            offset += ItemBox.Height * boxscale + 4;
            item = new StardewValley.Object(74, 1);
            pos = new Vector2(region.X, region.Y + offset);
            batch.Draw(Game1.mouseCursors, pos, ItemBox, Color.White, 0, Vector2.Zero, boxscale, SpriteEffects.None, 0);
            pos.X += 6;
            pos.Y += 6;
            item.drawInMenu(batch, pos, 1, 1, 0, false);
            Utility.drawTextWithShadow(batch, DynamicDungeonsMod.SHelper.Translation.Get("Difficulty_Hard"), Game1.smallFont, new Vector2(region.X + ItemBox.Width * boxscale + 4, region.Y + offset + (ItemBox.Height * boxscale - diffEasyHeight) / 2), Game1.textColor);
        }
    }
}
