using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Entoarox.CustomBooks
{
    class ConfigPage : SystemPage
    {
        private static Texture2D Quil;
        private static Texture2D Brush;
        private static Texture2D Edit;
        private static Texture2D Delete;
        static ConfigPage()
        {
            Quil = CustomBooksMod.SHelper.Content.Load<Texture2D>("quil.png");
            Brush = CustomBooksMod.SHelper.Content.Load<Texture2D>("brush.png");
            Edit = CustomBooksMod.SHelper.Content.Load<Texture2D>("edit.png");
            Delete = CustomBooksMod.SHelper.Content.Load<Texture2D>("delete.png");
        }

        private BookMenu Menu;
        private bool Deleting = false;

        public ConfigPage(BookMenu menu)
        {
            this.Menu = menu;
        }
        public override void Click(Rectangle region, int x, int y)
        {
            if(new Rectangle(region.X + 25, region.Y + 320, region.Width - 50,44).Contains(x,y))
            {
                Game1.playSound("bigDeSelect");
                foreach (Page page in this.Menu.Pages)
                    page.Editable = !page.Editable;
            }
            else if (new Rectangle(region.X + 25, region.Y + 370, region.Width - 50, 44).Contains(x, y))
            {
                if (this.Deleting)
                {
                    Game1.player.items.Filter(a => (a as Book)?.Id.Equals(this.Menu.Id) != true);
                    CustomBooksMod.Shelf.Books.Remove(this.Menu.Id);
                    Game1.playSound("trashcan");
                    this.Menu.exitThisMenu(false);
                }
                else
                {
                    Game1.playSound("bigDeSelect");
                    this.Deleting = true;
                }
            }
        }
        public override void Hover(Rectangle region, int x, int y)
        {
            if (this.Deleting && !new Rectangle(region.X + 25, region.Y + 370, region.Width - 50, 44).Contains(x, y))
                this.Deleting = false;
        }
        public override void Draw(SpriteBatch batch, Rectangle region)
        {
            Utility.drawTextWithShadow(batch, "Customise Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 16), Game1.textColor, 1);
            Utility.drawTextWithShadow(batch, "Name:", Game1.smallFont, new Vector2(region.X, region.Y + 100), Game1.textColor, 1);
            this.DrawTextBox(batch, Quil, region.X + 100, region.Y + 100, region.Width - 100);
            Utility.drawTextWithShadow(batch, "Color:", Game1.smallFont, new Vector2(region.X, region.Y + 140), Game1.textColor, 1);
            this.DrawTextBox(batch, Brush, region.X + 100, region.Y + 140, region.Width - 100);
            Utility.drawTextWithShadow(batch, "Manage Book", Game1.dialogueFont, new Vector2(region.X, region.Y + 250), Game1.textColor, 1);
            this.DrawButton(batch, Edit, region.X + 25, region.Y + 320, region.Width - 50, "Mode: " + (this.Editable ? "Editing" : "Reading"));
            this.DrawButton(batch, Delete, region.X + 25, region.Y + 370, region.Width - 50, this.Deleting ? "Click to Confirm" : "Destroy Book");
        }

        public override Bookshelf.Book.Page Serialize()
        {
            return null;
        }
    }
}
