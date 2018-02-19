using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Entoarox.CustomBooks
{
    class ConfigPage : Page
    {
        protected readonly static Rectangle ButtonNormal = new Rectangle(256, 256, 10, 10);
        protected readonly static Rectangle ButtonHover = new Rectangle(267, 256, 10, 10);
        private static Texture2D Quil;
        private static Texture2D Brush;
        private static Texture2D Edit;
        private static Texture2D Delete;
        private bool Add;
        static ConfigPage()
        {
            Quil = CustomBooksMod.SHelper.Content.Load<Texture2D>("quil.png");
            Brush = CustomBooksMod.SHelper.Content.Load<Texture2D>("brush.png");
            Edit = CustomBooksMod.SHelper.Content.Load<Texture2D>("edit.png");
            Delete = CustomBooksMod.SHelper.Content.Load<Texture2D>("delete.png");
        }

        public ConfigPage(bool add = false)
        {
            this.Add = add;
        }
        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            if (this.Add)
            {
                Utility.drawTextWithShadow(batch, "Add Page", Game1.dialogueFont, new Vector2(region.X, region.Y + 16), Game1.textColor, 1);
                this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 100, region.Width, "Add Title Page");
                this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 172, region.Width, "Add Text Page");
                this.DrawOption(batch, Game1.staminaRect, region.X, region.Y + 244, region.Width, "Add Image Page");
            }
            else
            {
                Utility.drawTextWithShadow(batch, "Customise Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 16), Game1.textColor, 1);
                Utility.drawTextWithShadow(batch, "Name:", Game1.smallFont, new Vector2(region.X, region.Y + 100), Game1.textColor, 1);
                this.DrawTextBox(batch, Quil, region.X + 100, region.Y + 100, region.Width - 100);
                Utility.drawTextWithShadow(batch, "Color:", Game1.smallFont, new Vector2(region.X, region.Y + 140), Game1.textColor, 1);
                this.DrawTextBox(batch, Brush, region.X + 100, region.Y + 140, region.Width - 100);
                Utility.drawTextWithShadow(batch, "Manage Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 250), Game1.textColor, 1);
                this.DrawButton(batch, Edit, region.X + 25, region.Y + 320, region.Width - 50, "Mode: Reading");
                this.DrawButton(batch, Delete, region.X + 25, region.Y + 370, region.Width - 50, "Destroy Book");
            }
        }
        private void DrawButton(SpriteBatch b, Texture2D icon, int x, int y, int width, string text)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), x, y, width, 44, new Rectangle(x, y, width, 44).Contains(Game1.getMousePosition()) ? Color.Wheat : Color.White, 2, false);
            b.Draw(icon, new Rectangle(x + 8, y + 8, 24, 24), null, Color.White);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(x + icon.Width * 2 + 14, y + 6), Game1.textColor);
        }
        private void DrawTextBox(SpriteBatch b, Texture2D icon, int x, int y, int width)
        {
            b.Draw(icon, new Rectangle(x + width - 24, y, 24, 24), Color.White * 0.75f);
            b.Draw(Game1.staminaRect, new Rectangle(x, y + 28, width - 24, 2), Game1.textColor*0.75f);
        }
        private void DrawOption(SpriteBatch b, Texture2D icon, int x, int y, int width, string text)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), x, y, width, 64, new Rectangle(x, y, width, 64).Contains(Game1.getMousePosition()) ? Color.Wheat : Color.White, 2, false);
            b.Draw(icon, new Rectangle(x + 8, y + 8, 48, 48), null, Color.White);
            Vector2 size = Game1.smallFont.MeasureString(text);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(x + 60, y + 32 - (size.Y / 2)), Game1.textColor);
        }

        public override Bookshelf.Book.Page Serialize()
        {
            return null;
        }
    }
}
