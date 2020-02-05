using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace SundropCity.Hotel
{
    class Menu : IClickableMenu
    {
        private readonly List<SObject> Items = new List<SObject>();
        private const int SizeW = 1172;
        private const int SizeH = 668;
        public Menu() : base(0, 0, SizeW, SizeH)
        {
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.initializeUpperRightCloseButton();
            for (int c = 0; c < 24; c++)
            {
                this.Items.Add(new HotelItem(c, 1000));
            }
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (newBounds.Width - this.width) / 2;
            this.yPositionOnScreen = (newBounds.Height - this.height) / 2;
            this.initializeUpperRightCloseButton();
        }
        private const int MaxItems = 7;
        private void DrawInventory(SpriteBatch b, int x, int y, IEnumerable<SObject> inventory)
        {
            var arr = inventory.ToArray();
            int max = Math.Min(MaxItems, arr.Length);
            for (int c = 0; c < max; c++)
                arr[c].drawInMenu(b, new Vector2(x + 4 + c % 12 * 64 + c % 12 * 4, y), 1f, 1f, 0.865f);
        }
        private void DrawResearch(SpriteBatch b, int x, int y, int id, string name, IEnumerable<SObject> reqs, IEnumerable<SObject> ingredients, IEnumerable<SObject> output, bool hover=false)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), x, y, 5 * 68 + 32 + 196, 3 * 68 + 64 + 16, hover ? Color.Wheat : Color.White, 3f, false);
            Utility.drawTextWithShadow(b, name, Game1.dialogueFont, new Vector2(x + 64, y + 16), Game1.textColor);
            var location = new Vector2(x + 4, y + 4);
            b.Draw(Game1.shadowTexture, location + new Vector2(32, 48), Game1.shadowTexture.Bounds, Color.White * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, 0.8649f);
            b.Draw(HotelItem.SpriteSheet, location + new Vector2(32, 32), Game1.getSourceRectForStandardTileSheet(HotelItem.SpriteSheet, id+13, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.865f);
            //IClickableMenu.drawTextureBox(b, Game1.menuTexture, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), x + 72, y + 64, 68 * 7, 64, Color.White, 1, false);
            //IClickableMenu.drawTextureBox(b, Game1.menuTexture, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), x + 72, y + 64 + 68, 68 * 7, 64, Color.White, 1, false);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), x + 72, y + 64, 68 * MaxItems, 64, Color.White, 1, false);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), x + 72, y + 64 + 68, 68 * MaxItems, 64, Color.White, 1, false);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1), x + 72, y + 64 + 136, 68 * MaxItems, 64, Color.White, 1, false);
            this.DrawInventory(b, x + 72, y + 64, reqs);
            this.DrawInventory(b, x + 72, y + 64 + 68, ingredients);
            this.DrawInventory(b, x + 72, y + 64 + 136, output);
            b.Draw(HotelItem.SpriteSheet, new Vector2(x + 40, y + 32 + 64), Game1.getSourceRectForStandardTileSheet(HotelItem.SpriteSheet, 24, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.865f);
            b.Draw(HotelItem.SpriteSheet, new Vector2(x + 40, y + 32 + 64 + 68), Game1.getSourceRectForStandardTileSheet(HotelItem.SpriteSheet, 30, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.865f);
            b.Draw(HotelItem.SpriteSheet, new Vector2(x + 40, y + 32 + 64 + 136), Game1.getSourceRectForStandardTileSheet(HotelItem.SpriteSheet, 31, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.865f);
            //Utility.drawTextWithShadow(b, "Requirements:", Game1.smallFont, , Game1.textColor);
            //Utility.drawTextWithShadow(b, "Ingredients:", Game1.smallFont, new Vector2(x + 16, y + 16 + 64 + 68), Game1.textColor);
            //Utility.drawTextWithShadow(b, "Rewards:", Game1.smallFont, new Vector2(x + 16, y + 16 + 64 + 136), Game1.textColor);
        }
        public override void draw(SpriteBatch b)
        {
            int lx = this.xPositionOnScreen + 16;
            int ty = this.yPositionOnScreen + 80;
            int rx = this.yPositionOnScreen + 616;
            int by = this.yPositionOnScreen + 368;
            int w = 568;
            int h = 284;
            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen + 64, this.width, this.height - 64, Color.White);
            this.DrawResearch(b, lx, ty, 0, "Refurbish Lobby", new[]{ this.Items[0], this.Items[1], this.Items[2] }, new[] { this.Items[6], this.Items[7], this.Items[8], this.Items[9], this.Items[10] }, new[] { this.Items[3], this.Items[4], this.Items[5] }, new Rectangle(lx, ty, w, h).Contains(Game1.oldMouseState.X,Game1.oldMouseState.Y));
            this.DrawResearch(b, rx, ty, 3, "Restore Exterior", new[] { this.Items[0], this.Items[1], this.Items[2] }, new[] { this.Items[6], this.Items[7], this.Items[8], this.Items[9], this.Items[10] }, new[] { this.Items[3], this.Items[4], this.Items[5] }, new Rectangle(rx, ty, w, h).Contains(Game1.oldMouseState.X, Game1.oldMouseState.Y));
            this.DrawResearch(b, lx, by, 3, "Replace Floortiles", new[] { this.Items[0], this.Items[1], this.Items[2] }, new[] { this.Items[6], this.Items[7], this.Items[8], this.Items[9], this.Items[10] }, new[] { this.Items[3], this.Items[4], this.Items[5] }, new Rectangle(lx, by, w, h).Contains(Game1.oldMouseState.X, Game1.oldMouseState.Y));
            this.DrawResearch(b, rx, by, 8, "General Research", new[] { this.Items[0], this.Items[1], this.Items[2] }, new[] { this.Items[6], this.Items[7], this.Items[8], this.Items[9], this.Items[10], this.Items[11], this.Items[11] }, new[] { this.Items[3], this.Items[4], this.Items[5]}, new Rectangle(rx, by, w, h).Contains(Game1.oldMouseState.X, Game1.oldMouseState.Y));
            base.draw(b);
            this.drawMouse(b);
        }
    }
}
