using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using Newtonsoft.Json;

namespace Entoarox.CustomBooks
{
    class TitlePage : Page
    {
        private string Title;
        private string Subtitle;
        private string Introduction;
        public TitlePage(string title, string subtitle, string introduction)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Introduction = introduction;
        }
        public TitlePage(string[] data)
        {
            this.Title = data[0];
            this.Subtitle = data[1];
            this.Introduction = data[2];
        }
        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            string introduction = Game1.parseText(this.Introduction, Game1.smallFont, region.Width);
            var titleSize = Game1.dialogueFont.MeasureString(this.Title);
            var subtitleSize = Game1.dialogueFont.MeasureString(this.Subtitle) * 0.8f;
            float titlePos = region.X + (region.Width - titleSize.X) / 2;
            float subtitlePos = region.X + (region.Width - subtitleSize.X) / 2;
            Utility.drawTextWithShadow(batch, this.Title, Game1.dialogueFont, new Vector2(titlePos, region.Y + 32), Game1.textColor, 1);
            Utility.drawTextWithShadow(batch, this.Subtitle, Game1.dialogueFont, new Vector2(subtitlePos, region.Y + titleSize.Y + 32), Game1.textColor, 0.8f);
            Utility.drawTextWithShadow(batch, introduction, Game1.smallFont, new Vector2(region.X, region.Y + titleSize.Y + subtitleSize.Y + subtitleSize.Y + 32), Game1.textColor);
        }
        public override Bookshelf.Book.Page Serialize()
        {
            var page = new Bookshelf.Book.Page
            {
                Type = Bookshelf.Book.PageType.Text,
                Content = JsonConvert.ToString(new[] { this.Title, this.Subtitle, this.Introduction })
            };
            return page;
        }

        public override void Click(Rectangle region, int x, int y)
        {
            
        }

        public override void Release(Rectangle region, int x, int y)
        {

        }

        public override void Hover(Rectangle region, int x, int y)
        {
            
        }
    }
}
