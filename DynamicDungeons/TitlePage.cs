using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.DynamicDungeons
{
    internal class TitlePage : Page
    {
        /*********
        ** Fields
        *********/
        private readonly string Introduction;
        private readonly string Subtitle;
        private readonly string Title;


        /*********
        ** Public methods
        *********/
        public TitlePage(string title, string subtitle, string introduction)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Introduction = introduction;
        }

        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            string introduction = Game1.parseText(this.Introduction, Game1.smallFont, region.Width);
            Vector2 titleSize = Game1.dialogueFont.MeasureString(this.Title);
            Vector2 subtitleSize = Game1.dialogueFont.MeasureString(this.Subtitle) * 0.8f;
            float titlePos = region.X + (region.Width - titleSize.X) / 2;
            float subtitlePos = region.X + (region.Width - subtitleSize.X) / 2;
            Utility.drawTextWithShadow(batch, this.Title, Game1.dialogueFont, new Vector2(titlePos, region.Y + 32), Game1.textColor, 1);
            Utility.drawTextWithShadow(batch, this.Subtitle, Game1.dialogueFont, new Vector2(subtitlePos, region.Y + titleSize.Y + 32), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(batch, introduction, Game1.smallFont, new Vector2(region.X, region.Y + titleSize.Y + subtitleSize.Y + subtitleSize.Y + 32), Game1.textColor);
        }
    }
}
