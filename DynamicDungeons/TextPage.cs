using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.DynamicDungeons
{
    internal class TextPage : Page
    {
        /*********
        ** Fields
        *********/
        private readonly string Text;


        /*********
        ** Public methods
        *********/
        public TextPage(string text)
        {
            this.Text = text;
        }

        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            string text = Game1.parseText(this.Text, Game1.smallFont, region.Width);
            Utility.drawTextWithShadow(batch, text, Game1.smallFont, new Vector2(region.X, region.Y), Game1.textColor);
        }
    }
}
