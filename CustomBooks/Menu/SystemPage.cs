using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Entoarox.CustomBooks.Menu
{
    internal abstract class SystemPage : Page
    {
        /*********
        ** Fields
        *********/
        protected static readonly Rectangle ButtonHover = new Rectangle(267, 256, 10, 10);
        protected static readonly Rectangle ButtonNormal = new Rectangle(256, 256, 10, 10);


        /*********
        ** Public methods
        *********/
        public override void Hover(Rectangle region, int x, int y) { }

        public override void Release(Rectangle region, int x, int y) { }


        /*********
        ** Protected methods
        *********/
        protected void DrawButton(SpriteBatch b, Texture2D icon, int x, int y, int width, string text)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), x, y, width, 44, new Rectangle(x, y, width, 44).Contains(Game1.getMousePosition()) ? Color.Wheat : Color.White, 2, false);
            b.Draw(icon, new Rectangle(x + 8, y + 8, 24, 24), null, Color.White);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(x + icon.Width * 2 + 14, y + 6), Game1.textColor);
        }

        protected void DrawTextBox(SpriteBatch b, Texture2D icon, int x, int y, int width)
        {
            b.Draw(icon, new Rectangle(x + width - 24, y, 24, 24), Color.White * 0.75f);
            b.Draw(Game1.staminaRect, new Rectangle(x, y + 28, width - 24, 2), Game1.textColor * 0.75f);
        }

        protected void DrawOption(SpriteBatch b, Texture2D icon, int x, int y, int width, string text)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), x, y, width, 64, new Rectangle(x, y, width, 64).Contains(Game1.getMousePosition()) ? Color.Wheat : Color.White, 2, false);
            b.Draw(icon, new Rectangle(x + 8, y + 8, 48, 48), null, Color.White);
            Vector2 size = Game1.smallFont.MeasureString(text);
            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(x + 60, y + 32 - size.Y / 2), Game1.textColor);
        }
    }
}
